// ICTCLAS.h

#pragma once
#include "Utility\\Utility.h"
#include "Utility\\Dictionary.h"
#include "Segment\\Segment.h"
#include "Tag\\Span.h"
#include "Unknown\\UnknowWord.h"

using namespace System;

public enum class eOperateType : System::SByte  { OnlySegment, FirstTag, SecondTag };
public enum class eOutputFormat : System::SByte { PKU, _973, XML };

public ref class NICTCLAS
{
public:
	NICTCLAS(String^ dictDir);
	~NICTCLAS();
protected:
	!NICTCLAS();

private:
	char* m_dictDir;
	char* m_bigramDictPath;
	char* m_coreDictPath;
	char* m_lexicalPath;
	char* m_nrPath;
	char* m_nrDctPath;
	char* m_nrCtxPath;
	char* m_nsPath;
	char* m_nsDctPath;
	char* m_nsCtxPath;
	char* m_trPath;
	char* m_trDctPath;
	char* m_trCtxPath;
	bool disposed;
private:
	int m_nOperateType, m_nOutputFormat;
	double m_dSmoothingPara;

	CSegment *m_Seg;//Seg class
	CDictionary *m_dictCore, *m_dictBigram;//Core dictionary,bigram dictionary
	CSpan *m_POSTagger;//POS tagger
	CUnknowWord *m_uPerson, *m_uTransPerson, *m_uPlace;//Person recognition

	int m_nResultCount;
	PWORD_RESULT *m_pResult;
	//The buffer which store the segment and POS result
	//and They stored order by its possibility
	ELEMENT_TYPE *m_dResultPossibility;
public:
	bool ParagraphProcessing(String^ source, String^% result);
public:
	/**////
	///0:Only Segment;1: First Tag; 2:Second Tag
	///
	property eOperateType OperateType
	{
		void set(eOperateType v) { m_nOperateType = (int)v; }
		eOperateType get() { return (eOperateType)m_nOperateType; } 
	}
	/**////
	///0:PKU criterion;1:973 criterion; 2: XML criterion
	///
	property eOutputFormat OutputFormat
	{
		void set(eOutputFormat v) { m_nOutputFormat = (int)v; }
		eOutputFormat get() { return (eOutputFormat)m_nOutputFormat; }
	}
	property double SmoothingPara
	{
		void set(double v) { m_dSmoothingPara = v; }
		double get() { return m_dSmoothingPara; }
	}

private:
	bool IsDataExists();
	bool Processing(char *sSentence,unsigned int nCount);
	bool ParagraphProcessing(char *sParagraph,char *sResult);
	bool Output(PWORD_RESULT pItem, char *sResult, bool bFirstWordIgnore);
	bool ChineseNameSplit(char *sPersonName, char *sSurname, char *sSurname2, char *sGivenName, CDictionary &personDict);
	bool PKU2973POS(int nHandle, char *sPOS973);
	bool Adjust(PWORD_RESULT pItem, PWORD_RESULT pItemRet);
	ELEMENT_TYPE ComputePossibility(PWORD_RESULT pItem);
	bool Sort();
	char* Refstr2Charptr(String^ src);
};

