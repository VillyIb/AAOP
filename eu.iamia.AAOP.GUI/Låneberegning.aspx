<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Låneberegning.aspx.cs" Inherits="eu.iamia.AAOP.GUI.Låneberegning" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Låneberegning</h1>
            <table>
                <thead>

                    <tr>
                        <th>Emne</th>
                        <th>Værdi</th>
                    </tr>
                </thead>
                <tbody>

                    <tr>
                        <td>Hovedstol:</td>
                        <td>
                            <asp:TextBox runat="server" ID="XuHovedstol"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Omkostning.</td>
                        <td>
                            <asp:TextBox runat="server" ID="XuOmkostning"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Pålydende rente:</td>
                        <td>
                            <asp:TextBox runat="server" ID="XuRente"></asp:TextBox><br />
                        </td>
                    </tr>
                    <tr>
                        <td>Løbetid i md.</td>
                        <td>
                            <asp:TextBox runat="server" ID="XuNumberOfPeriods"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:LinkButton runat="server" ID="XuCalculate" OnClick="XuCalculate_Click">Beregn</asp:LinkButton></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>Ydelse:</td>
                        <td>
                            <asp:Label runat="server" ID="XuYdelse"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>Debitor rente:</td>
                        <td>
                            <asp:Label runat="server" ID="XuDebRente"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>ÅOP</td>
                        <td>
                            <asp:Label runat="server" ID="XuAAOP"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:LinkButton runat="server" ID="XuExport" OnClick="XuExport_Click">Hent betalingsforløb</asp:LinkButton></td>
                    </tr>

                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <th colspan="2">Arkiv</th>
                    </tr>

                    <tr>
                        <td>Filnavn (uden extension)</td>
                        <td>
                            <asp:TextBox runat="server" ID="XuFilename"></asp:TextBox></td>
                    </tr>

                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:LinkButton runat="server" ID="XuSave" OnClick="XuSave_Click">Gem låneberegning</asp:LinkButton></td>
                    </tr>

                    <tr>
                        <td>Låneberegninger</td>
                        <td>
                            <asp:DropDownList runat="server" ID="XuArchiveList" OnSelectedIndexChanged="XuArchiveList_SelectedIndexChanged" AutoPostBack="True" /></td>
                    </tr>

                </tbody>
            </table>

        </div>

    </form>
</body>
</html>
