<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Default" Title="Query Survey" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <ajax:ScriptManager runat="Server" ID="ScriptManager1" />
    <div id="mainFrame" style="height: 900px;">
        <div class="siteBranding" nowrap>
            <font size="5pt" family="Verdana" color="white">Query Survey</font>
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
                            <asp:Panel runat="server" ID="panel_Specific" Height="30px">
                                <font color="#636363" size="2pt" family="Verdana"><b>Query&nbsp;Collection:&nbsp;</b></font>
                                <asp:DropDownList runat="server" ID="dl_QueryCollections" Width="369px" />&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:ImageButton runat="server" ID="btn_Load" AlternateText="Load" src="Images/loadout1.gif"
                                    onmouseover="javascript:src='images/loadover1.gif';" onmouseout="javascript:src='images/loadout1.gif'"
                                    onmousedown="javascript:src='images/loaddown1.gif'" onmouseup="javascript:src='images/loadover1.gif'"
                                    OnClick="btn_Load_Click" />&nbsp;&nbsp;
                                <asp:ImageButton runat="server" ID="btn_New" AlternateText="New" ImageUrl="Images/newout1.gif"
                                    onmouseover="javascript:src='images/newover1.gif';" onmouseout="javascript:src='images/newout1.gif'"
                                    onmousedown="javascript:src='images/newdown1.gif'" onmouseup="javascript:src='images/newover1.gif'" />&nbsp;&nbsp;
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
                            <asp:GridView runat="server" ID="gdv_QueryCollection" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" PageSize="20" ShowFooter="True" OnDataBound="gdv_QueryCollecion_DataBound"
                                OnPageIndexChanging="gdv_QueryCollecion_PageIndexChanging" OnSorting="gdv_QueryCollecion_Sorting"
                                Width="100%" Font-Size="Small" RowStyle-Height="20px" RowStyle-VerticalAlign="middle"
                                OnRowCancelingEdit="gdv_QueryCollecion_RowCancelingEdit" OnRowEditing="gdv_QueryCollecion_RowEditing"
                                OnRowUpdating="gdv_QueryCollecion_RowUpdating" OnRowDataBound="gdv_QueryCollecion_RowDataBound"
                                OnRowDeleting="gdv_QueryCollection_RowDeleting">
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
                                        <ItemStyle HorizontalAlign="Center" Width="40px" />
                                    </asp:CommandField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton runat="server" CommandName="Delete" OnClientClick="return confirm('Are you sure to delete the query?');"
                                                ImageUrl="Images/delete.gif"></asp:ImageButton>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="center" Width="30px" />
                                    </asp:TemplateField>
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
