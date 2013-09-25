<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default"
    Theme="Microsoft" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Search Demo</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="head" align="center" style="height: 72px; background: url(Images/bg.gif) repeat-x top #1586CD;">
            <table width="98%" style="height: 40px" border="0" align="center" cellpadding="0"
                cellspacing="0">
                <tr>
                    <td align="center">
                        <font face="Verdana" size="2">Powered by WebSearch.Net Platform Solution Team</font></td>
                </tr>
            </table>
            <asp:Panel DefaultButton="btn_Search" runat="server" ID="panel_Head">
                <table width="777" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td width="95">
                            <img src="Images\butterfly.gif" width="19" height="18" />&nbsp;&nbsp;&nbsp;<span
                                class="nhwhite13">网页库：</span></td>
                        <td width="75">
                            <asp:TextBox runat="server" ID="tb_WebCollection" Text="SogouIndex" Width="70px"></asp:TextBox>
                        </td>
                        <td width="310" align="right">
                            <asp:TextBox runat="server" ID="tb_UserQuery" Width="300"></asp:TextBox></td>
                        <td width="120">
                            <span class="nhwhite13">结果数：</span>
                            <asp:DropDownList runat="server" ID="dl_ResultNum">
                                <asp:ListItem Text="10" Value="10" />
                                <asp:ListItem Text="20" Value="20" Selected="true" />
                                <asp:ListItem Text="50" Value="50" />
                                <asp:ListItem Text="100" Value="100" />
                                <asp:ListItem Text="150" Value="150" />
                                <asp:ListItem Text="200" Value="200" />
                                <asp:ListItem Text="300" Value="300" />
                            </asp:DropDownList></td>
                        <td width="69">
                            <asp:ImageButton runat="server" ID="btn_Search" ImageUrl="Images/Search.gif" OnClick="btn_Search_Click" /></td>
                        <td width="70">
                            <span style="padding-top: 7px">
                                <asp:HyperLink NavigateUrl="~/compare.aspx" CssClass="nhwhite12" runat="server" ID="lnk_Compare">结果比较</asp:HyperLink></span></td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
        <div id="results">
            <table border="0">
                <tr>
                    <td style="height: 700px">
                        <p>
                            <ul>
                                <asp:DataList runat="server" ID="dtl_SearchResults" OnItemDataBound="dtl_SearchResults_ItemDataBound">
                                    <ItemTemplate>
                                        <li>
                                            <h3 style="width: 640px">
                                                <%#((WebSearch.Model.Net.SearchResult)Container.DataItem).Rank%>
                                                &nbsp;
                                                <asp:HyperLink Target="_blank" NavigateUrl="<%#((WebSearch.Model.Net.SearchResult)Container.DataItem).Url%>"
                                                    Text="<%#((WebSearch.Model.Net.SearchResult)Container.DataItem).Anchor%>" runat="server"
                                                    ID="lnk_Title" />
                                                &nbsp;<asp:Label runat="server" ID="lb_SogouRank" Font-Names="Microsoft Sans Serif"
                                                    Font-Size="8" BackColor="#FCF4B5" />
                                            </h3>
                                            <p style="margin-left: 15px">
                                                <asp:Label runat="server" ID="lb_Snippets" Width="650px"></asp:Label>
                                            </p>
                                            <ul class="metaData">
                                                <li class="dispUrl">&nbsp;&nbsp;&nbsp;<%#((WebSearch.Model.Net.SearchResult)Container.DataItem).Url%>
                                                </li>
                                                <li class="cached">缓存页面</li></ul>
                                        </li>
                                    </ItemTemplate>
                                </asp:DataList>
                            </ul>
                            <br />
                            <br />
                        </p>
                    </td>
                </tr>
            </table>
        </div>
        <div id="foot" align="center" style="height: 72px; background: url(Images/bg.gif) repeat-x top #1586CD;">
            <br />
            <asp:Panel DefaultButton="btn_Search2" runat="server" ID="panel_Foot">
                <table width="777" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td width="95">
                            <img src="Images\butterfly.gif" width="20" height="19" />&nbsp;&nbsp;&nbsp;<span
                                class="nhwhite13">网页库：</span></td>
                        <td width="75">
                            <asp:TextBox runat="server" ID="tb_WebCollection2" Text="SogouIndex" Width="70px"></asp:TextBox>
                        </td>
                        <td width="310" align="right">
                            <asp:TextBox runat="server" ID="tb_UserQuery2" Width="300"></asp:TextBox></td>
                        <td width="120">
                            <span class="nhwhite13">结果数：</span>
                            <asp:DropDownList runat="server" ID="dl_ResultNum2">
                                <asp:ListItem Text="10" Value="10" />
                                <asp:ListItem Text="20" Value="20" Selected="true" />
                                <asp:ListItem Text="50" Value="50" />
                                <asp:ListItem Text="100" Value="100" />
                                <asp:ListItem Text="150" Value="150" />
                                <asp:ListItem Text="200" Value="200" />
                                <asp:ListItem Text="300" Value="300" />
                            </asp:DropDownList></td>
                        <td width="69">
                            <asp:ImageButton runat="server" ID="btn_Search2" ImageUrl="Images/Search.gif" OnClick="btn_Search2_Click" /></td>
                        <td width="70">
                            <span style="padding-top: 7px">
                                <asp:HyperLink NavigateUrl="~/compare.aspx" CssClass="nhwhite12" runat="server" ID="lnk_Compare2">结果比较</asp:HyperLink></span></td>
                    </tr>
                </table>
            </asp:Panel>
            <table width="98%" style="height: 40px" border="0" align="center" cellpadding="0"
                cellspacing="0">
                <tr>
                    <td align="center">
                        <font face="Verdana" size="2">Powered by WebSearch.Net Platform Solution Team</font></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
