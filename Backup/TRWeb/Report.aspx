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
        <ServerReport ReportServerUrl="http://sqlentdb1/ReportServer" 
                reportpath="/ERP Middleware/ERPReports/ProcessingSummary"></ServerReport>
    </rsweb:ReportViewer>
    </p>
</asp:Content>
