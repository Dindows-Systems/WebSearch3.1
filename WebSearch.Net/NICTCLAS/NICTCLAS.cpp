// This is the main DLL file.

#include "stdafx.h"

#include <string>
#include <math.h>
#include <io.h>
#include <vcclr.h>
#include <windows.h>

#include "NICTCLAS.h"

char* cpystr(char* src)
{
	char* result = new char[strlen(src) + 1];
	strcpy(result, src);
	return result;
}

NICTCLAS::NICTCLAS(String^ dictDir)
{
	m_dictDir = this->Refstr2Charptr(dictDir);
	m_bigramDictPath = this->Refstr2Charptr(dictDir + "\\BigramDict.dct");
	m_coreDictPath = this->Refstr2Charptr(dictDir + "\\coreDict.dct");
	m_lexicalPath = this->Refstr2Charptr(dictDir + "\\lexical.ctx");
	m_nrPath = this->Refstr2Charptr(dictDir + "\\nr");
	m_nrDctPath = this->Refstr2Charptr(dictDir + "\\nr.dct");
	m_nrCtxPath = this->Refstr2Charptr(dictDir + "\\nr.ctx");
	m_nsPath = this->Refstr2Charptr(dictDir + "\\ns");
	m_nsDctPath = this->Refstr2Charptr(dictDir + "\\ns.dct");
	m_nsCtxPath = this->Refstr2Charptr(dictDir + "\\ns.ctx");
	m_trPath = this->Refstr2Charptr(dictDir + "\\tr");
	m_trDctPath = this->Refstr2Charptr(dictDir + "\\tr.dct");
	m_trCtxPath = this->Refstr2Charptr(dictDir + "\\tr.ctx");

	if(!IsDataExists())
		throw gcnew System::Exception("The data is not available.Please check path of data.");

	disposed = false;
	m_Seg = new CSegment();
	m_dictCore = new CDictionary();
	m_dictBigram = new CDictionary();
	m_POSTagger = new CSpan();
	m_uPerson = new CUnknowWord();
	m_uTransPerson = new CUnknowWord();
	m_uPlace = new CUnknowWord();

	m_pResult = new PWORD_RESULT[MAX_SEGMENT_NUM];
	for(int i = 0; i < MAX_SEGMENT_NUM; i++)
	{
		m_pResult[i] = new WORD_RESULT[MAX_WORDS];
	}
	m_dResultPossibility = new ELEMENT_TYPE[MAX_SEGMENT_NUM];

	m_dictCore->Load(m_coreDictPath);
	m_POSTagger->LoadContext(m_lexicalPath);
/*
	m_dictCore.Load(m_dictDir + "\\Dict.dct");
	m_POSTagger.LoadContext(m_dictDir + "\\trainTest.ctx");
*/
/*
	m_dictCore.AddItem("十分",'d'*256,+500);
	m_dictCore.AddItem("十分",'m'*256,-500);
	m_dictCore.AddItem("我国",'n'*256,-2000);
	m_dictCore.AddItem("我国",'r'*256,+2000);
	m_dictCore.AddItem("千年",'t'*256,200);

    m_dictCore.Optimum();
	m_dictCore.Save(m_dictDir + "\\coreDictOptimum.dct");
*/

	
	m_POSTagger->SetTagType();

	m_uPerson->Configure(m_nrPath, TT_PERSON);
	//Set the person recognition configure
	m_uPlace->Configure(m_nsPath, TT_PLACE);
	//Set the place recognition configure
	m_uTransPerson->Configure(m_trPath, TT_TRANS_PERSON);
	//Set the transliteration person recognition configure
	
	m_nOperateType=0;//0:Only Segment;1: First Tag; 2:Second Type
	m_nOutputFormat=0;//0:PKU criterion;1:973 criterion; 2: XML criterion
	m_dSmoothingPara=0.1;//Smoothing parameter

	m_dictBigram->Load(m_bigramDictPath);
}

NICTCLAS::~NICTCLAS() //c++/cli的实现中Destructor自动调用了System.GC::SuppressFinalize
{
	if(disposed)//Destructor实现的Dispose模式不完整，需要控制多次清理的标记位
		return;
	// dispose managed resources

	this->!NICTCLAS();
	disposed = true;
}

NICTCLAS::!NICTCLAS()
{
	// dispose unmanaged resources
	delete m_Seg;
	delete m_dictCore;
	delete m_dictBigram;
	delete m_POSTagger;
	delete m_uPerson;
	delete m_uTransPerson;
	delete m_uPlace;

	delete [] m_pResult;
	delete [] m_dResultPossibility;
}

bool NICTCLAS::ParagraphProcessing(String^ source, String^% result)
{
	char* sParagraph = this->Refstr2Charptr(source);
	char* sResult;
	if(m_nOutputFormat!=2)
		sResult=new char [(strlen(sParagraph)+13)*3];
	else
		sResult=new char [(strlen(sParagraph)+13)*50];
	bool returnVal = NICTCLAS::ParagraphProcessing(sParagraph, sResult);
	result = gcnew String(sResult);
	delete [] sParagraph;
	delete [] sResult;
	return returnVal;
}

bool NICTCLAS::IsDataExists()
{
	char* sDataFiles[] = {	m_bigramDictPath, m_coreDictPath, m_lexicalPath,
		                    m_nrDctPath, m_nrCtxPath, m_nsDctPath, m_nsCtxPath,
		                    m_trDctPath, m_trCtxPath, ""};
	int i = 0;
	while(strlen(sDataFiles[i]) != 0)
	{
		if ((_access(sDataFiles[i], 0)) == -1)
			return false;
		i++;
	}
	return true;
}

bool NICTCLAS::Output(PWORD_RESULT pItem, char *sResult, bool bFirstWordIgnore)
{
	int i=0;
	char sTempBuffer[WORD_MAXLENGTH],sPOS[3];
	sPOS[2]=0;
	sResult[0]=0;
	if(bFirstWordIgnore)//Ignore first valid
		i=1;
	while(pItem[i].sWord[0]!=0&&pItem[i].nHandle!=CT_SENTENCE_END)//Not sentence ending flag
	{
		//Get the POS string
		if(m_nOutputFormat!=0)//Not PKU format
			PKU2973POS(pItem[i].nHandle,sPOS);
		else//PKU format
		{
			sPOS[0]=pItem[i].nHandle/256;
			sPOS[1]=pItem[i].nHandle%256;
		}
		sPOS[m_nOperateType]=0;//Set the sPOS with operate type
		
		if(m_nOutputFormat==0)//PKU format
		{
			sprintf(sTempBuffer,"%s",pItem[i].sWord);
		    strcat(sResult,sTempBuffer);
			if(sPOS[0]!=0)//need POS 
			{
				sprintf(sTempBuffer,"/%s",sPOS);
				strcat(sResult,sTempBuffer);
			}
		    strcat(sResult,"  ");
		}
		else if(m_nOutputFormat==1)//973 format
		{
			sprintf(sTempBuffer,"%s\\",pItem[i].sWord);
		    strcat(sResult,sTempBuffer);
			if(sPOS[0]!=0)//need POS 
			{
				sprintf(sTempBuffer,"[%s]",sPOS);
				strcat(sResult,sTempBuffer);
			}
		}
		else if(m_nOutputFormat==2)//XML format
		{
			if(sPOS[0]!=0)//POS
			{
				sprintf(sTempBuffer,"<any type=\042%s\042>",sPOS);
				strcat(sResult,sTempBuffer);
			}
			sprintf(sTempBuffer,"<src>%s</src>",pItem[i].sWord);
			strcat(sResult,sTempBuffer);
			if(sPOS[0]!=0)
			{
				strcat(sResult,"</any>");
			}
		}
		i++;
	}
	return true;
}

bool NICTCLAS::ChineseNameSplit(char *sPersonName, char *sSurname, char *sSurname2, char *sGivenName, CDictionary &personDict)
{
	int nSurNameLen=4,nLen=strlen(sPersonName),nFreq,i=0,nCharType,nFreqGiven;
	char sTemp[3];
	if(nLen<3||nLen>8)//Not a traditional Chinese person name
		return false;
	while(i<nLen)//No Including non-CHinese char
	{
		nCharType=charType((unsigned char*)sPersonName+i);
		if(nCharType!=CT_CHINESE&&nCharType!=CT_OTHER)
			return false;
		i+=2;
	}
	sSurname2[0]=0;//init 
	strncpy(sSurname,sPersonName,nSurNameLen);	
	sSurname[nSurNameLen]=0;
	if(!personDict.IsExist(sSurname,1))
	{
		nSurNameLen=2;
		sSurname[nSurNameLen]=0;
		if(!personDict.IsExist(sSurname,1))
		{
			nSurNameLen=0;
			sSurname[nSurNameLen]=0;
		}
	}
	strcpy(sGivenName,sPersonName+nSurNameLen);
	if(nLen>6)
	{
		strncpy(sTemp,sPersonName+nSurNameLen,2);
		sTemp[2]=0;//Get the second possible surname
		if(personDict.IsExist(sTemp,1))
		{//Hongkong women's name: Surname+surname+given name
			strcpy(sSurname2,sTemp);
			strcpy(sGivenName,sPersonName+nSurNameLen+2);
		}
	}
	nFreq=personDict.GetFrequency(sSurname,1);
	strncpy(sTemp,sGivenName,2);
	sTemp[2]=0;
	nFreqGiven=personDict.GetFrequency(sTemp,2);
	if(nSurNameLen!=4&&((nSurNameLen==0&&nLen>4)||strlen(sGivenName)>4||(GetForeignCharCount(sPersonName)>=3&&nFreq<personDict.GetFrequency("张",1)/40&&nFreqGiven<personDict.GetFrequency("华",2)/20)||(nFreq<10&&GetForeignCharCount(sGivenName)==(nLen-nSurNameLen)/2)))
		return false;
	if(nLen==4&&m_uPerson->IsGivenName(sPersonName))
	{//Single Surname+given name
		return false;
	}
	return true;
}


bool NICTCLAS::PKU2973POS(int nHandle, char *sPOS973)
{
	int nHandleSet[46]={24832,24932,24935,24942,25088,25344,25600,25703,25856,26112,26368,26624,26880,27136,27392,27648,27904,28160,28263,28274,28275,28276,28280,28282,28416,28672,28928,29184,29440,29696,29799,29952,30052,30055,30058,30060,30070,30074,30208,30308,30311,30318,30464,30720,30976,31232};
						//   "a", "ad","ag","an","b", "c", "d", "dg","e", "f","g", "h", "i", "j", "k", "l", "m", "n", "ng","nr","ns","nt","nx","nz","o", "p", "q", "r", "s", "t", "tg","u", "ud","ug","uj","ul","uv","uz","v", "vd","vg","vn","w", "x", "y", "z"
	char sPOSRelated[46][3]={"a", "ad","ga","an","f", "c", "d", "d", "e","nd","g", "h", "i", "j", "k", "l", "m", "n", "gn","nh","ns","ni","ws", "nz","o", "p", "q", "r", "nl","nt","gt","u", "ud","ug","uj","ul","uv","uz","v", "vd","gv","vn","w", "x", "u", "a"};
/* 
 "Bg","gf",
 "Rg","gr",
 "Mg","gm",
 "Yg","u",
 "Ug","u",
 "Qg","q",
*/

	int nIndex=BinarySearch(nHandle,nHandleSet,46);
	if(nIndex==-1)
		strcpy(sPOS973,"@");
	else
		strcpy(sPOS973,sPOSRelated[nIndex]);
	return true;
}

//Adjust the result with some rules
bool NICTCLAS::Adjust(PWORD_RESULT pItem,PWORD_RESULT pItemRet)
{
	int i=0,j=0;
	unsigned int nLen;
	char sSurName[10],sSurName2[10],sGivenName[10];
	bool bProcessed=false;//Have been processed
	while(pItem[i].sWord[0]!=0)
	{
		nLen=strlen(pItem[i].sWord);
		bProcessed=false;
		
		//Rule1: adjust person name
		if(pItem[i].nHandle==28274&&ChineseNameSplit(pItem[i].sWord,sSurName,sSurName2,sGivenName,m_uPerson->m_dict)&&strcmp(pItem[i].sWord,"叶利钦")!=0)//'nr'
		{//Divide name into surname and given name
			
			if(sSurName[0])
			{
				strcpy(pItemRet[j].sWord,sSurName);
				pItemRet[j++].nHandle=28274;
			}
			if(sSurName2[0])
			{
				strcpy(pItemRet[j].sWord,sSurName2);
				pItemRet[j++].nHandle=28274;
			}
			if(sGivenName[0])
			{
				strcpy(pItemRet[j].sWord,sGivenName);
				pItemRet[j++].nHandle=28274;
			}
			bProcessed=true;
		}
		//Rule2 for overlap words ABB 一段段、一片片
		else if(pItem[i].nHandle==27904&&strlen(pItem[i+1].sWord)==2&&strcmp(pItem[i+1].sWord,pItem[i+2].sWord)==0)
		{//(pItem[i+1].nHandle/256=='q'||pItem[i+1].nHandle/256=='a')&&
			strcpy(pItemRet[j].sWord,pItem[i].sWord);
			strcat(pItemRet[j].sWord,pItem[i+1].sWord);
			strcat(pItemRet[j].sWord,pItem[i+2].sWord);
			pItemRet[j].nHandle=27904;
			j+=1;
			i+=2;
			bProcessed=true;
		}
		//Rule3 for overlap words AA
		else if(nLen==2&&strcmp(pItem[i].sWord,pItem[i+1].sWord)==0)
		{
			strcpy(pItemRet[j].sWord,pItem[i].sWord);
			strcat(pItemRet[j].sWord,pItem[i+1].sWord);
		     //24832=='a'*256
			pItemRet[j].nHandle=24832;//a
			if(pItem[i].nHandle/256=='v'||pItem[i+1].nHandle/256=='v')//30208='v'8256
			{
				pItemRet[j].nHandle=30208;
			}
			if(pItem[i].nHandle/256=='n'||pItem[i+1].nHandle/256=='n')//30208='v'8256
			{
				pItemRet[j].nHandle='n'*256;
			}			
			i+=1;
			if(strlen(pItem[i+1].sWord)==2)
			{//AAB:洗/洗/脸、蒙蒙亮
				if((pItemRet[j].nHandle==30208&&pItem[i+1].nHandle/256=='n')||
				   (pItemRet[j].nHandle==24832&&pItem[i+1].nHandle/256=='a')
				   )
				{
					strcat(pItemRet[j].sWord,pItem[i+1].sWord);
					i+=1;
				}
			}
			j+=1;
			bProcessed=true;
		}

		//Rule 4: AAB 洗/洗澡
		else if(nLen==2&&strncmp(pItem[i].sWord,pItem[i+1].sWord,2)==0&&strlen(pItem[i+1].sWord)==4&&(pItem[i].nHandle/256=='v'||pItem[i].nHandle==24832))//v,a
		{
			strcpy(pItemRet[j].sWord,pItem[i].sWord);
			strcat(pItemRet[j].sWord,pItem[i+1].sWord);
		     //24832=='a'*256
			pItemRet[j].nHandle=24832;//'a'
			if(pItem[i].nHandle/256=='v'||pItem[i+1].nHandle/256=='v')//30208='v'8256
			{
				pItemRet[j].nHandle=30208;
			}

			i+=1;
			j+=1;
			bProcessed=true;
		}
		else if(pItem[i].nHandle/256=='u'&&pItem[i].nHandle%256)//uj,ud,uv,uz,ul,ug->u
			pItem[i].nHandle='u'*256;
		else if(nLen==2&&strncmp(pItem[i].sWord,pItem[i+1].sWord,2)==0&&strlen(pItem[i+1].sWord)==4&&strncmp(pItem[i+1].sWord+2,pItem[i+2].sWord,2)==0)
		{//AABB 朴朴素素 枝枝叶叶
				strcpy(pItemRet[j].sWord,pItem[i].sWord);
				strcat(pItemRet[j].sWord,pItem[i+1].sWord);
				strcat(pItemRet[j].sWord,pItem[i+2].sWord);
				pItemRet[j].nHandle=pItem[i+1].nHandle;
				i+=2;
				j+=1;
				bProcessed=true;
		}
		else if(pItem[i].nHandle==28275)//PostFix
		{
			if(m_uPlace->m_dict.IsExist(pItem[i+1].sWord,4))
			{
				strcpy(pItemRet[j].sWord,pItem[i].sWord);
				strcat(pItemRet[j].sWord,pItem[i+1].sWord);
				pItemRet[j].nHandle=28275;
				i+=1;
				j+=1;
				bProcessed=true;
			}
			else if(strlen(pItem[i+1].sWord)==2&&CC_Find("队",pItem[i+1].sWord))
			{
				strcpy(pItemRet[j].sWord,pItem[i].sWord);
				strcat(pItemRet[j].sWord,pItem[i+1].sWord);
				pItemRet[j].nHandle=28276;
				i+=1;
				j+=1;
				bProcessed=true;
			}
			else if(strlen(pItem[i+1].sWord)==2&&CC_Find("语文字杯",pItem[i+1].sWord))
			{
				strcpy(pItemRet[j].sWord,pItem[i].sWord);
				strcat(pItemRet[j].sWord,pItem[i+1].sWord);
				pItemRet[j].nHandle=28282;
				i+=1;
				j+=1;
				bProcessed=true;
			}
			else if(strlen(pItem[i+1].sWord)==2&&CC_Find("裔",pItem[i+1].sWord))
			{
				strcpy(pItemRet[j].sWord,pItem[i].sWord);
				strcat(pItemRet[j].sWord,pItem[i+1].sWord);
				pItemRet[j].nHandle=28160;
				i+=1;
				j+=1;
				bProcessed=true;
			}
		}
		else if(pItem[i].nHandle==30208||pItem[i].nHandle==28160)//v
		{
			if(strlen(pItem[i+1].sWord)==2&&CC_Find("员",pItem[i+1].sWord))
			{
				strcpy(pItemRet[j].sWord,pItem[i].sWord);
				strcat(pItemRet[j].sWord,pItem[i+1].sWord);
				pItemRet[j].nHandle=28160;
				i+=1;
				j+=1;
				bProcessed=true;
			}
		}
		else if(pItem[i].nHandle==28280)
		{//www/nx ./w sina/nx; ＥＩＭ/nx  -６０１/m 
			strcpy(pItemRet[j].sWord,pItem[i].sWord);
			pItemRet[j].nHandle=28280;
			while(pItem[i+1].nHandle==28280||strstr(".．",pItem[i+1].sWord)||(pItem[i+1].nHandle==27904&&IsAllNum((unsigned char *)pItem[i+1].sWord)))
			{
				strcat(pItemRet[j].sWord,pItem[i+1].sWord);
				i+=1;
			}
			j+=1;
			bProcessed=true;
		}

		if(!bProcessed)
		{//If not processed,that's mean: not need to adjust;
		 //just copy to the final result
			strcpy(pItemRet[j].sWord,pItem[i].sWord);
			pItemRet[j++].nHandle=pItem[i].nHandle;
		}
		i++;
	}
	pItemRet[j].sWord[0]=0;//Set ending
	return true;
}

//Compute the possibility of current segmentation and POS result
ELEMENT_TYPE NICTCLAS::ComputePossibility(PWORD_RESULT pItem)
{
	int i=0;
	ELEMENT_TYPE dResultPossibility=0;
	while(pItem[i].sWord[0]!=0)
	{
		dResultPossibility+=pItem[i].dValue;
		//Compute the possibility of logP(Wi|Ti)
		if(pItem[i+1].sWord[0]!=0)//Not the last one
		{//Compute the possibility of logP(Ti|Ti-1)
			dResultPossibility+=log((double)(m_POSTagger->m_context.GetContextPossibility(0,pItem[i].nHandle,pItem[i+1].nHandle)+1));
			dResultPossibility-=log((double)(m_POSTagger->m_context.GetFrequency(0,pItem[i].nHandle)+1));
		}
		i++;
	}
	return dResultPossibility;
}

bool NICTCLAS::Sort()
{
	ELEMENT_TYPE dPossibility[MAX_SEGMENT_NUM],dTemp;
	int nIndex[MAX_SEGMENT_NUM],nTemp;//Index

	memset(dPossibility,0,sizeof(dPossibility));
	//Init the possibility
	
	for(int i=0;i<m_Seg->m_nSegmentCount;i++)
	{//Computing the possibility
		dPossibility[i]=ComputePossibility(m_Seg->m_pWordSeg[i]);
		nIndex[i]=i;//Record the index
	}
	
	//Sort with Bubble sort algorithm
	for(int i=0;i<m_Seg->m_nSegmentCount;i++)
		for(int j=i+1;j<m_Seg->m_nSegmentCount;j++)
		{
			if(dPossibility[i]<dPossibility[j])
			{//Swap the possition and value
				nTemp=nIndex[i];
				dTemp=dPossibility[i];
				nIndex[i]=nIndex[j];
				dPossibility[i]=dPossibility[j];
				nIndex[j]=nTemp;
				dPossibility[j]=dTemp;
			}
		}
	
	for(int i=0;i<m_Seg->m_nSegmentCount;i++)
	{//Adjust the segmentation and POS result and store them in the final result array
	 //Store them according their possibility ascendly
		Adjust(m_Seg->m_pWordSeg[nIndex[i]],m_pResult[i]);	
		m_dResultPossibility[i]=dPossibility[i];	
	}
	return true;
}

//Paragraph Segment and POS Tagging
bool NICTCLAS::ParagraphProcessing(char *sParagraph,char *sResult)
{
	char *sSentence,sChar[3];
	char *sSentenceResult;
	unsigned int nLen=strlen(sParagraph)+13;
	sSentence=new char[nLen];//malloc buffer
	sSentenceResult=new char[nLen*3];//malloc buffer
	sSentence[0]=0;
	unsigned int nPosIndex=0,nParagraphLen=strlen(sParagraph),nSentenceIndex=0;
	sChar[2]=0;
	sResult[0]=0;//Init the result
	bool bFirstIgnore=true;
	strcpy(sSentence,SENTENCE_BEGIN);//Add a sentence begin flag
	while(nPosIndex<nParagraphLen)
	{//Find a whole sentence which separated by ! . \n \r
		sChar[0]=sParagraph[nPosIndex];//Get a char
		sChar[1]=0;
		if(sParagraph[nPosIndex]<0)
		{//double byte char
			nPosIndex+=1;
			sChar[1]=sParagraph[nPosIndex];
		}
		nPosIndex+=1;

#define  SEPERATOR_C_SENTENCE "。！？：；…"
#define  SEPERATOR_C_SUB_SENTENCE "、，（）“”‘’"
#define  SEPERATOR_E_SENTENCE "!?:;"
#define  SEPERATOR_E_SUB_SENTENCE ",()\042'"
#define  SEPERATOR_LINK "\n\r 　"

		if(CC_Find(SEPERATOR_C_SENTENCE,sChar)||CC_Find(SEPERATOR_C_SUB_SENTENCE,sChar)||strstr(SEPERATOR_E_SENTENCE,sChar)||strstr(SEPERATOR_E_SUB_SENTENCE,sChar)||strstr(SEPERATOR_LINK,sChar))
		{//Reach end of a sentence.Get a whole sentence
			if(!strstr(SEPERATOR_LINK,sChar))//Not link seperator
			{
				strcat(sSentence,sChar);
			}
			if(sSentence[0]!=0&&strcmp(sSentence,SENTENCE_BEGIN)!=0)
			{
				if(!strstr(SEPERATOR_C_SUB_SENTENCE,sChar)&&!strstr(SEPERATOR_E_SUB_SENTENCE,sChar))
					strcat(sSentence,SENTENCE_END);//Add sentence ending flag

				Processing(sSentence,1);//Processing and output the result of current sentence.
				Output(m_pResult[0],sSentenceResult,bFirstIgnore);//Output to the imediate result
				//bFirstIgnore=true;
				strcat(sResult,sSentenceResult);//Store in the result buffer
			}
			if(strstr(SEPERATOR_LINK,sChar))//Link the result with the SEPERATOR_LINK
			{
				strcat(sResult,sChar);
				strcpy(sSentence,SENTENCE_BEGIN);//Add a sentence begin flag

				//sSentence[0]=0;//New sentence, and begin new segmentation
				//bFirstIgnore=false;
			}
			else if(strstr(SEPERATOR_C_SENTENCE,sChar)||strstr(SEPERATOR_E_SENTENCE,sChar))
			{
				strcpy(sSentence,SENTENCE_BEGIN);//Add a sentence begin flag
				//sSentence[0]=0;//New sentence, and begin new segmentation
				//bFirstIgnore=false;
			}
			else
			{
				strcpy(sSentence,sChar);//reset current sentence, and add the previous end at begin position
			}
		}
		else //Other chars and store in the sentence buffer
			strcat(sSentence,sChar);
	}
	if(sSentence[0]!=0&&strcmp(sSentence,SENTENCE_BEGIN)!=0)
	{
		strcat(sSentence,SENTENCE_END);//Add sentence ending flag
		Processing(sSentence,1);//Processing and output the result of current sentence.
		Output(m_pResult[0],sSentenceResult,bFirstIgnore);//Output to the imediate result
		strcat(sResult,sSentenceResult);//Store in the result buffer
	}
	delete []  sSentence;//FREE sentence buffer 	
	delete []  sSentenceResult;//free buffer
	return true;
}

bool NICTCLAS::Processing(char *sSentence,unsigned int nCount)
{
	int nIndex;
#if _ICT_DEBUG
	char *sSegment;
	sSegment=new char[MAX_SENTENCE_LEN*2];
#endif
	//Unigram segment
	//m_Seg->Segment(sSentence,m_dictCore,nCount);
	//Bigram segment
	m_Seg->BiSegment(sSentence, m_dSmoothingPara,*m_dictCore,*m_dictBigram,nCount);
	
	m_nResultCount=m_Seg->m_nSegmentCount;
	//Record the number of result
	for(nIndex=0;nIndex<m_Seg->m_nSegmentCount;nIndex++)
	{
#if _ICT_DEBUG
			m_POSTagger.POSTagging(m_Seg->m_pWordSeg[nIndex],m_dictCore,m_dictCore);
			Output(m_Seg->m_pWordSeg[nIndex],sSegment);
			printf("POS Tag%d:%s\n",nIndex+1,sSegment);
#endif
		m_uPerson->Recognition(m_Seg->m_pWordSeg[nIndex],m_Seg->m_graphOptimum,m_Seg->m_graphSeg,*m_dictCore);
		m_uTransPerson->Recognition(m_Seg->m_pWordSeg[nIndex],m_Seg->m_graphOptimum,m_Seg->m_graphSeg,*m_dictCore);
		m_uPlace->Recognition(m_Seg->m_pWordSeg[nIndex],m_Seg->m_graphOptimum,m_Seg->m_graphSeg,*m_dictCore);
	}		
	//m_uPerson.Recognition(m_Seg->m_WordSeg[0],m_Seg->m_graphOptimum,m_Seg->m_graphSeg,m_dictCore);
	//Person Recognition
#if _ICT_DEBUG
	printf("After person recognition.\n");
#endif
	//Unigram
	//m_Seg->OptimumSegmet(nCount);
	//Bigram
	m_Seg->BiOptimumSegment(nCount,m_dSmoothingPara,*m_dictBigram,*m_dictCore);

	for(nIndex=0;nIndex<m_Seg->m_nSegmentCount;nIndex++)
	{
		m_POSTagger->POSTagging(m_Seg->m_pWordSeg[nIndex],*m_dictCore,*m_dictCore);
#if _ICT_DEBUG
		Output(m_Seg->m_pWordSeg[nIndex],sSegment);
		printf("POS Tag%d:%s\n",nIndex+1,sSegment);
#endif
	}
#if _ICT_DEBUG
	printf("After Sorting.\n");
#endif
	Sort();//Sort the ending 
#if _ICT_DEBUG
	for(nIndex=0;nIndex<m_Seg->m_nSegmentCount;nIndex++)
	{
		Output(m_pResult[nIndex],sSegment);
		printf("POS Tag%d(P=Exp(%f)):%s\n",nIndex+1,m_dResultPossibility[nIndex],sSegment);
	}
	delete [] sSegment;
#endif
	return true;
}

char* NICTCLAS::Refstr2Charptr(String^ src)
{
	pin_ptr<const wchar_t> wch = PtrToStringChars(src);
	int len = WideCharToMultiByte(CP_ACP, 0, wch, -1, NULL, 0, NULL, NULL) ; 
	/*std::string buf;
	buf.resize(len);*/
	char* buf = new char[len];
	WideCharToMultiByte(CP_ACP, 0, wch, -1, buf, len ,NULL ,NULL);
	return buf;
}