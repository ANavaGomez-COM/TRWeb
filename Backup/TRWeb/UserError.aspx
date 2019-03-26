<%@ Page Title="Error" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="UserError.aspx.cs" Inherits="TRWeb.UserError" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>ERROR</h2>
    <p>
        An error has occured.&nbsp; The user does not exist or does not have rights to 
        use this application.
    </p>
</asp:Content>
