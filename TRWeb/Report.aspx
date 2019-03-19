<%@ Page Title="Report" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Report.aspx.cs" Inherits="TRWeb.Report" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        REPORT</h2>
    <p>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:DropDownList ID="ddlReportName" runat="server" AutoPostBack="true"
            OnSelectedIndexChanged="ddlReportName_OnSelectedIndexChange" >
        </asp:DropDownList>
        <br />
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" 
        Font-Names="Verdana" Font-Size="8pt" InteractiveDeviceInfos="(Collection)" 
        ProcessingMode="Remote" WaitMessageFont-Names="Verdana" 
        WaitMessageFont-Size="14pt" Height="1250px" Width="923px">
<%-- Moved to code behind dynamic
            <ServerReport ReportServerUrl="http://sqlentdb1/ReportServer" 
            reportpath="/ERP Middleware/TestERPReports/ProcessingSummary"></ServerReport>--%>
    </rsweb:ReportViewer>
    </p>
    <asp:Panel ID="pnlImport" runat="server" Visible="false" >
        <asp:Label ID="lblStartDate" runat="server" Text="Date of Import:"></asp:Label>
        <asp:TextBox ID="tbStartDateImport" runat="server" TextMode="Date"></asp:TextBox>
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" OnClick="btnRefreshImport_Click" />
        <br />
        <asp:Label ID="lblImportStatus" runat="server" Text=""></asp:Label>
        <br />
        <table style="border:solid;">
            <tr>
                <th colspan="5" style="font-size:medium; border-bottom:solid;">Load Packages</th>
            </tr>
            <tr>
                <th><asp:Button ID="btnSendImport" runat="server" Text="Send Selected" OnClick="btnSendImport_Click"  /></th>
                <th>Created Date</th>
                <th>Source Type</th>
                <th>Batch ID</th>
                <th>Report Date</th>
            </tr>
        <asp:Repeater ID="RptrImport" runat="server">
            <ItemTemplate>
                <tr>
                    <td style="text-align:center; border:solid;">
                        <asp:CheckBox ID="cbSelected" runat="server" />
                    </td>
                    <td style="border:solid;">
                        <asp:Label ID="lblCreatedDate" runat="server" Text='<%# Eval("CreatedDate") %>'></asp:Label>
                    </td>
                    <td style="border:solid;">
                         <asp:Label ID="lblSourceType" runat="server" Text='<%# Eval("SourceTypeDescription") %>' />
                    </td>
                    <td style="border:solid;">
                         <asp:Label ID="lblBatchId" runat="server" Text='<%# Eval("BatchId") %>' />
                    </td>
                    <td style="border:solid;">
                         <asp:Label ID="lblReportDate" runat="server" Text='<%# String.Format("{0:MM/dd/yyyy}", Eval("ReportDate")) %>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        </table>
        <br />

 
    </asp:Panel>
    <asp:Panel ID="pnlExport" runat="server" Visible="false" >
        <asp:Label ID="Label1" runat="server" Text="Date of Export:"></asp:Label>
        <asp:TextBox ID="tbStartDateExport" runat="server" TextMode="Date"></asp:TextBox>
        <asp:Button ID="btnRefreshExport" runat="server" Text="Refresh" OnClick="btnRefreshExport_Click" />
        <br />
        <asp:Label ID="lblExportStatus" runat="server" Text=""></asp:Label>
        <br />
        <table style="border:solid;">
            <tr>
                <th colspan="5" style="font-size:medium; border-bottom:solid;">Export Packages</th>
            </tr>
            <tr>
                <th><asp:Button ID="btnSendExport" runat="server" Text="Send Selected" OnClick="btnSendExport_Click"  /></th>
                <th>Created Date</th>
                <th>Source Type</th>                
                <th>Batch ID</th>
                <th>Report Date</th>
            </tr>
            <asp:Repeater ID="RptrExport" runat="server">
            <ItemTemplate>
                <tr>
                    <td style="text-align:center; border:solid;">
                        <asp:CheckBox ID="cbSelected" runat="server" />
                    </td>
                    <td style="border:solid;">
                         <asp:Label ID="lblSourceType" runat="server" Text='<%# Eval("SourceTypeDescription") %>' />
                    </td>
                    <td style="border:solid;">
                         <asp:Label ID="Label2" runat="server" Text='<%# Eval("SourceTypeDescription") %>' />
                    </td>
                    <td style="border:solid;">
                         <asp:Label ID="lblBatchId" runat="server" Text='<%# Eval("BatchId") %>' />
                    </td>
                    <td style="border:solid;">
                         <asp:Label ID="lblReportDate" runat="server" Text='<%# String.Format("{0:MM/dd/yyyy}", Eval("ReportDate")) %>' />
                    </td>
                </tr>
            </ItemTemplate>
            </asp:Repeater>
        </table>
    </asp:Panel>

</asp:Content>
