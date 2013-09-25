<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Training.aspx.cs" Inherits="Training" Title="Automatic Query Type Identification System - Training Data" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <ajax:ScriptManager runat="Server" ID="ScriptManager1" />
    <div id="mainFrame" style="height: 900px;">
        <div class="siteBranding" nowrap>
            <font size="5pt" family="Verdana" color="white">Training Data</font>
        </div>
        <table border="0">
            <tr height="75">
                <td colspan="3">
                </td>
            </tr>
            <tr>
                <td rowspan="2" width="77">
                </td>
                <td>
                    <ajax:UpdatePanel runat="server" ID="UpdatePanel1">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="panel_QueryLogs" Height="30px">
                                <font color="#636363" size="2pt" family="Verdana"><b>Query&nbsp;Log&nbsp;DB:&nbsp;</b></font>
                                <asp:DropDownList runat="server" ID="dl_QueryLogDBs" OnSelectedIndexChanged="dl_QueryLogDBs_SelectedIndexChanged"
                                    AutoPostBack="True" Width="120px" />
                                <font color="#636363" size="2pt" family="Verdana"><b>&nbsp;&nbsp;Training&nbsp;Set:&nbsp;</b></font>
                                <asp:DropDownList runat="server" ID="dl_TrainingSets" Width="150px" />&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:ImageButton runat="server" ID="btn_Load" AlternateText="Load" src="Images/loadout1.gif"
                                    onmouseover="javascript:src='images/loadover1.gif';" onmouseout="javascript:src='images/loadout1.gif'"
                                    onmousedown="javascript:src='images/loaddown1.gif'" onmouseup="javascript:src='images/loadover1.gif'"
                                    OnClick="btn_Load_Click" />&nbsp;&nbsp;
                                <asp:ImageButton runat="server" ID="btn_Train" AlternateText="Train" ImageUrl="Images/trainout1.gif"
                                    onmouseover="javascript:src='images/trainover1.gif';" onmouseout="javascript:src='images/trainout1.gif'"
                                    onmousedown="javascript:src='images/traindown1.gif'" onmouseup="javascript:src='images/trainover1.gif'" />&nbsp;&nbsp;
                                <asp:ImageButton runat="server" ID="btn_New" AlternateText="New" src="Images/newout1.gif"
                                    onmouseover="javascript:src='images/newover1.gif';" onmouseout="javascript:src='images/newout1.gif'"
                                    onmousedown="javascript:src='images/newdown1.gif'" onmouseup="javascript:src='images/newover1.gif'" />
                                <p />
                                <asp:Panel runat="server" ID="panel_ModalPopup" Style="display: none" CssClass="modalPopup">
                                    <asp:Panel ID="panel_ModalPopupTitle" runat="server" Style="cursor: move; background-color: #DDDDDD;
                                        border: solid 1px Gray; color: Black" Height="25px">
                                        <div>
                                            <p align="center">
                                                <b>New a Training Set</b></p>
                                        </div>
                                    </asp:Panel>
                                    <div>
                                        <table border="0">
                                            <tr>
                                                <td>
                                                    Training&nbsp;Set&nbsp;Name:</td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="tb_TrainSetName" Width="150px" Height="16px" /></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Query&nbsp;Log&nbsp;DB:</td>
                                                <td>
                                                    <asp:DropDownList runat="server" ID="dl_ChooseQueryLogDBs" Width="150px" /></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Query&nbsp;Log&nbsp;Table:</td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="tb_QueryLogTable" Width="150px" Height="16px" /></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Count&nbsp;of&nbsp;Logs:</td>
                                                <td>
                                                    <ew:NumericBox runat="server" MaxLength="3" ID="tb_QueryLogCount" Width="150px" Height="16px" />
                                                </td>
                                            </tr>
                                        </table>
                                        <br />
                                        <p style="text-align: center;">
                                            <asp:Button ID="btn_NewOK" runat="server" Text="OK" OnClick="btn_New_Click" />
                                            <asp:Button ID="btn_NewCancel" runat="server" Text="Cancel" />
                                        </p>
                                    </div>
                                </asp:Panel>
                                <qajax:ModalPopupExtender ID="ModalPopupExtender" runat="server" TargetControlID="btn_New"
                                    PopupControlID="panel_ModalPopup" BackgroundCssClass="modalBackground" CancelControlID="btn_NewCancel"
                                    DropShadow="true" X="300" Y="200" MaskWidth="934" MaskHeight="900" />
                            </asp:Panel>
                            <asp:Panel runat="server" ID="panel_Specific" Height="30px">
                                <font color="#636363" size="2pt" family="Verdana"><b>Spec&nbsp;Trainset:&nbsp;</b></font>
                                <asp:DropDownList runat="server" ID="dl_SpecificTrainsets" Width="369px" />&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:ImageButton runat="server" ID="btn_Load2" AlternateText="Load" src="Images/loadout1.gif"
                                    onmouseover="javascript:src='images/loadover1.gif';" onmouseout="javascript:src='images/loadout1.gif'"
                                    onmousedown="javascript:src='images/loaddown1.gif'" onmouseup="javascript:src='images/loadover1.gif'"
                                    OnClick="btn_Load2_Click" />&nbsp;&nbsp;
                                <asp:ImageButton runat="server" ID="btn_Train2" AlternateText="Test" ImageUrl="Images/trainout1.gif"
                                    onmouseover="javascript:src='images/trainover1.gif';" onmouseout="javascript:src='images/trainout1.gif'"
                                    onmousedown="javascript:src='images/traindown1.gif'" onmouseup="javascript:src='images/trainover1.gif'" />&nbsp;&nbsp;
                            </asp:Panel>
                        </ContentTemplate>
                    </ajax:UpdatePanel>
                </td>
                <td rowspan="2" width="77">
                </td>
            </tr>
            <tr>
                <td>
                    <ajax:UpdatePanel runat="server" ID="UpdatePanel2">
                        <ContentTemplate>
                            <p>
                                &nbsp;</p>
                            <asp:GridView runat="server" ID="gdv_TrainItems" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" PageSize="20" ShowFooter="True" OnDataBound="gdv_TrainItems_DataBound"
                                OnPageIndexChanging="gdv_TrainItems_PageIndexChanging" OnSorting="gdv_TrainItems_Sorting"
                                Width="100%" Font-Size="Small" RowStyle-Height="20px" RowStyle-VerticalAlign="middle"
                                OnRowCancelingEdit="gdv_TrainItems_RowCancelingEdit" OnRowEditing="gdv_TrainItems_RowEditing"
                                OnRowUpdating="gdv_TrainItems_RowUpdating" OnRowDataBound="gdv_TrainItems_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="queryID" HeaderText="ID" ReadOnly="True" SortExpression="queryID">
                                        <ItemStyle HorizontalAlign="center" Width="80px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="queryText" HeaderText="Query Text" SortExpression="queryText"
                                        ReadOnly="True" />
                                    <asp:BoundField DataField="queryType" HeaderText="Type" SortExpression="queryType"
                                        NullDisplayText="N/A" ReadOnly="True" HtmlEncode="true">
                                        <ItemStyle HorizontalAlign="center" Width="100px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dvariance" HeaderText="1/Variance" SortExpression="dvariance"
                                        NullDisplayText="N/A" ReadOnly="True" DataFormatString="{0:F3}" HtmlEncode="false">
                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:RadioButtonList ID="RadioButtonListView" runat="server" Height="26px" RepeatDirection="Horizontal"
                                                Width="151px" Enabled="false">
                                                <asp:ListItem Value="1">Info.</asp:ListItem>
                                                <asp:ListItem Value="2">Navi.</asp:ListItem>
                                                <asp:ListItem Value="3">Tran.</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:RadioButtonList ID="RadioButtonListEdit" runat="server" Height="26px" RepeatDirection="Horizontal"
                                                Width="151px">
                                                <asp:ListItem Value="1">Info.</asp:ListItem>
                                                <asp:ListItem Value="2">Navi.</asp:ListItem>
                                                <asp:ListItem Value="3">Tran.</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </EditItemTemplate>
                                        <ItemStyle Width="160px" />
                                    </asp:TemplateField>
                                    <asp:CommandField ShowEditButton="True">
                                        <ItemStyle HorizontalAlign="Center" Width="80px" />
                                    </asp:CommandField>
                                </Columns>
                                <RowStyle Height="20px" VerticalAlign="Middle" />
                            </asp:GridView>
                            <p>
                                &nbsp;</p>
                            <asp:HyperLink runat="server" ID="lnk_Charting" Visible="false" Font-Size="Small"
                                Font-Bold="true">
                    Charting&nbsp;the&nbsp;Query&nbsp;Type&nbsp;Distribution&nbsp;along&nbsp;the&nbsp;Survey&nbsp;Variance&nbsp;
                    &nbsp;&nbsp;&nbsp;<img alt="" src="images/charting.jpg" /></asp:HyperLink>
                        </ContentTemplate>
                    </ajax:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
