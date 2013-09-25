<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Src="Controls/Description.ascx" TagName="Description" TagPrefix="uc1" %>
<%@ Register Src="Controls/HighLights.ascx" TagName="HighLights" TagPrefix="uc1" %>
<%@ Register Src="Controls/Statistics.ascx" TagName="Statistics" TagPrefix="uc1" %>
<%@ Register Src="Controls/Documents.ascx" TagName="Documents" TagPrefix="uc1" %>
<%@ Register Src="Controls/UserManual.ascx" TagName="UserManual" TagPrefix="uc1" %>
<%@ Register Src="Controls/References.ascx" TagName="References" TagPrefix="uc1" %>
<%@ Register Src="Controls/AboutUs.ascx" TagName="AboutUs" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <ajax:ScriptManager runat="Server" ID="ScriptManager1" />
    <div class="siteBranding" nowrap>
        <font size="6pt" family="Verdana" color="white"><b><i>APEX Lab.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</i></b></font><font
            size="4pt" color="white"><i>Shanghai JiaoTong University</i></font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img
                alt="S.J.T.U" src="images/sjtulogo.gif" />
    </div>
    <div id="main" style="height: 300px;">
        <div id="masterNav" align="left" nowrap>
            <div class="png" id="masterNavTitle">
                &nbsp;</div>
            <div class="png" id="masterNavSearch">
                <form id="searchForm">
                    <div class="png" id="searchTextDiv">
                        <asp:TextBox runat="server" ID="txtbox_Search" CssClass="unwatermarked" autocomplete="off" SkinID="None" />
                        <ajax:TextBoxWatermarkExtender ID="TextBoxWatermarkExtenderSearch" runat="server"
                            TargetControlID="txtbox_Search" WatermarkText="Powered by APEX Lab" WatermarkCssClass="watermarked" />
                        <ajax:AutoCompleteExtender runat="server" ID="autoCompleteSearch" TargetControlID="txtbox_Search"
                            ServicePath="QueryAutoCompleteSrv.asmx" ServiceMethod="GetCompletionList" MinimumPrefixLength="2"
                            CompletionInterval="1000" EnableCaching="true" CompletionSetCount="4" />
                        <qajax:QueryTypeIdentifierExtender runat="server" ID="autoQueryTypeIdentifier" TargetControlID="txtbox_Search"
                            ServicePath="QueryTypeIdentificationSrv.asmx" ServiceMethod="GetQueryType" MinimumPrefixLength="2"
                            CompletionInterval="1500" EnableCaching="true" CompletionInformationalID="radio_Informational"
                            CompletionNavigationalID="radio_Navigational" CompletionTransactionalID="radio_Transactional"
                            ProgressBarID="img_Loading" />
                    </div>
                    <input id="submit" onclick="javascript:chkstr();" type="image" alt="Go" src="Images/search_go1.gif"
                        onmouseover="javascript:src='images/search_go2.gif';" onmouseout="javascript:src='images/search_go1.gif'" />
                    <div id="mnsRad">
                        <br />
                        <input id="apexSearch" type="radio" checked value="microsoft" name="searchTarget" /><img
                            title="Apex Search" onclick="apexSearch.checked=true" alt="Apex Search" src="Images/lablogo.gif" /><label
                                for="apexSearch">Apex.com</label>&nbsp;&nbsp;&nbsp;
                        <input id="liveSearch" type="radio" value="msn" name="searchTarget" /><label for="liveSearch">
                            <img title="Live Search" onclick="liveSearch.checked=true" alt="Live Search" src="Images/ls.gif" />Live
                            Search</label></div>
                    <div id="searchPopularity">
                        <br />
                        <ajax:Rating ID="ThaiRatingPopularity" runat="server" CurrentRating="3" MaxRating="9"
                            StarCssClass="ratingStar" WaitingStarCssClass="savedRatingStar" FilledStarCssClass="filledRatingStar"
                            EmptyStarCssClass="emptyRatingStar" RatingAlign="Horizontal" RatingDirection="leftToRightTopToBottom"
                            ToolTip="The popularity of the given query" ReadOnly="true" Style="float: left;"
                            Wrap="false" />
                        &nbsp;<font color="#727272" face="Verdana">(Popularity)</font>
                    </div>
                </form>
            </div>
            <div id="masterNavBody">
                <asp:Panel runat="server" ID="panel_QueryTypeTitle">
                    <h1 class="active">
                        <div class="topNavText" style="vertical-align: bottom">
                            Query Type Identification&nbsp;<asp:Image runat="server" ID="img_Arrow" ImageUrl="~/App_Themes/Microsoft/images/arrow_rt.png" /></div>
                    </h1>
                </asp:Panel>
                <asp:Panel runat="server" ID="panel_QueryType">
                    <asp:Image runat="server" ID="img_Loading" ImageUrl="images/free.gif" Width="398px"
                        Height="10px" /><br />
                    <asp:RadioButton GroupName="queryTypes" runat="server" ID="radio_Navigational" Text="Navigational" />
                    <asp:RadioButton GroupName="queryTypes" runat="server" ID="radio_Informational" Text="Informational" />
                    <asp:RadioButton GroupName="queryTypes" runat="server" ID="radio_Transactional" Text="Transactional" /><br />
                </asp:Panel>
            </div>
            <div class="png" id="masterNavFooter">
                &nbsp;</div>
            <ajax:CollapsiblePanelExtender ID="CollapsiblePanelQueryType" runat="server" ExpandControlID="panel_QueryTypeTitle"
                TargetControlID="panel_QueryType" CollapseControlID="panel_QueryTypeTitle" ImageControlID="img_Arrow" />
        </div>
    </div>
    <div class="newsHeadline">
        <div class="container">
            <span class="title">ABOUT</span><a href="Default.aspx">Shanghai JiaoTong University,
                Apex Lab.</a></div>
    </div>
    <div class="verticalTabs">
        <div class="tabs ulTabs">
            <ul>
                <li class="tab tabActive" id="tbh0_0" onkeydown="return KP(event);" onmouseover="activateTab(0,0)"
                    onfocus="KPF='activateTab(0,0)';" tabindex="0" onmouseout="mouseOutTab(0,0)">General
                    Description</li>
                <li class="tab" id="tbh0_1" onkeydown="return KP(event);" onmouseover="activateTab(0,1)"
                    onfocus="KPF='activateTab(0,1)';" tabindex="0" onmouseout="mouseOutTab(0,1)">Highlights</li>
                <li class="tab" id="tbh0_2" onkeydown="return KP(event);" onmouseover="activateTab(0,2)"
                    onfocus="KPF='activateTab(0,2)';" tabindex="0" onmouseout="mouseOutTab(0,2)">Tests
                    and Trainings</li>
                <li class="tab" id="tbh0_3" onkeydown="return KP(event);" onmouseover="activateTab(0,3)"
                    onfocus="KPF='activateTab(0,3)';" tabindex="0" onmouseout="mouseOutTab(0,3)">System
                    Documents</li>
                <li class="tab" id="tbh0_4" onkeydown="return KP(event);" onmouseover="activateTab(0,4)"
                    onfocus="KPF='activateTab(0,4)';" tabindex="0" onmouseout="mouseOutTab(0,4)">User
                    Manual</li>
                <li class="tab" id="tbh0_5" onkeydown="return KP(event);" onmouseover="activateTab(0,5)"
                    onfocus="KPF='activateTab(0,5)';" tabindex="0" onmouseout="mouseOutTab(0,5)">References</li>
                <li class="tab" id="tbh0_6" onkeydown="return KP(event);" onmouseover="activateTab(0,6)"
                    onfocus="KPF='activateTab(0,6)';" tabindex="0" onmouseout="mouseOutTab(0,6)">About
                    Us</li>
                <li></li>
            </ul>
        </div>
    </div>
    <div class="tabs dlTabs">
        <h5>
            On This Page</h5>
        <p>
            <a href="default.aspx#General+Description">General Description</a></p>
        <p>
            <a href="default.aspx#Highlights">Highlights</a></p>
        <p>
            <a href="default.aspx#Tests+and+Trainings">Tests and Trainings</a></p>
        <p>
            <a href="default.aspx#System+Documents">System Documents</a></p>
        <p>
            <a href="default.aspx#User+Manual">User Manual</a></p>
        <p>
            <a href="default.aspx#References">References</a></p>
        <p>
            <a href="default.aspx#About+Us">About Us</a></p>
    </div>
    <div class="tabContentPanel dl">
        <div class="tbc">
            <h3 class="dlh">
                General&nbsp;Description<a name="General+Description">&nbsp;</a></h3>
            <div class="tabPanel dlTabP" id="tbc0_0">
                <uc1:Description ID="DescriptionContent" runat="server" />
            </div>
            <div class="dlTabSpacer">
                <a href="default.aspx#top">^ Top of page</a></div>
        </div>
        <div class="tbc">
            <h3 class="dlh">
                Highlights<a name="Highlights">&nbsp;</a></h3>
            <div class="tabPanel dlTabP" id="tbc0_1">
                <uc1:HighLights ID="HighLightsContent" runat="server" />
            </div>
            <div class="dlTabSpacer">
                <a href="default.aspx#top">^ Top of page</a></div>
        </div>
        <div class="tbc">
            <h3 class="dlh">
                Tests&nbsp;and&nbsp;Trainings<a name="Tests+and+Trainings">&nbsp;</a></h3>
            <div class="tabPanel dlTabP" id="tbc0_2">
                <uc1:Statistics ID="StatisticsContent" runat="server" />
            </div>
            <div class="dlTabSpacer">
                <a href="default.aspx#top">^ Top of page</a></div>
        </div>
        <div class="tbc">
            <h3 class="dlh">
                System&nbsp;Documents<a name="System+Documents">&nbsp;</a></h3>
            <div class="tabPanel dlTabP" id="tbc0_3">
                <uc1:Documents ID="DocumentsContent" runat="server" />
            </div>
            <div class="dlTabSpacer">
                <a href="default.aspx#top">^ Top of page</a></div>
        </div>
        <div class="tbc">
            <h3 class="dlh">
                User&nbsp;Manual<a name="User+Manual">&nbsp;</a></h3>
            <div class="tabPanel dlTabP" id="tbc0_4">
                <uc1:UserManual ID="UserManualContent" runat="server" />
            </div>
            <div class="dlTabSpacer">
                <a href="default.aspx#top">^ Top of page</a></div>
        </div>
        <div class="tbc">
            <h3 class="dlh">
                References<a name="References">&nbsp;</a></h3>
            <div class="tabPanel dlTabP" id="tbc0_5">
                <uc1:References ID="ReferencesContent" runat="server" />
            </div>
            <div class="dlTabSpacer">
                <a href="default.aspx#top">^ Top of page</a></div>
        </div>
        <div class="tbc">
            <h3 class="dlh">
                About&nbsp;Us<a name="About+Us">&nbsp;</a></h3>
            <div class="tabPanel dlTabP" id="tbc0_6">
                <uc1:AboutUs ID="AboutUsContent" runat="server" />
            </div>
            <div class="dlTabSpacer">
                <a href="default.aspx#top">^ Top of page</a></div>
        </div>
    </div>
</asp:Content>
