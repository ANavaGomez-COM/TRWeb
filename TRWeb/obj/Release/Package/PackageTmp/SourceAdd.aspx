<%@ Page Title="Administration" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="SourceAdd.aspx.cs" Inherits="TRWeb.SourceAdd" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
        
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>ADD SOURCE</h2>
    <div>Enter all information below then click Add Source.</div>
    <br />
    <br />
    <div class="addfloater">
        <asp:Label ID="lblSource"  AssociatedControlID="txtSource" runat="server" Text="Source Code" ></asp:Label>
        <asp:TextBox CssClass="myTextBox" ID="txtSource" runat="server" AutoPostBack="true" ></asp:TextBox>
        <br />
        <asp:Label CssClass="labelError" ID="lblSourceError" runat="server" ></asp:Label>
        <br />
        <br />
        <asp:Label ID="lblSourceDescription"  AssociatedControlID="txtSourceDescription" runat="server" 
            Text="Source Description" ></asp:Label>
        <asp:TextBox CssClass="myTextBox" ID="txtSourceDescription" runat="server" ></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="lblSourceAttament"  AssociatedControlID="txtSourceAttachment" runat="server" 
            Text="Source Attachment Name" ></asp:Label>
        <asp:TextBox CssClass="myTextBox" ID="txtSourceAttachment" runat="server" ></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="lblBillerProductCode"  AssociatedControlID="txtBillerProductCode" runat="server" 
            Text="Biller Product Code" ></asp:Label>
        <asp:TextBox CssClass="myTextBox" ID="txtBillerProductCode" runat="server" ></asp:TextBox>
    </div>
    <br />
    <div class="addfloater">
        <asp:Label ID="lblNotifEmail"  AssociatedControlID="txtNotifEmail" runat="server" Text="Notification Email" ></asp:Label>
        <asp:TextBox CssClass="myTextBox" ID="txtNotifEmail" runat="server" ></asp:TextBox>
        <br />
        <asp:Label CssClass="labelError" ID="lblNotifEmailError" runat="server" ></asp:Label>
        <br />
        <br />
        <asp:Label ID="lblReceiveRemittance"  AssociatedControlID="chkReceiveRemittance" runat="server" Text="Receive Remittance Email" ></asp:Label>
        <asp:CheckBox ID="chkReceiveRemittance" runat="server" />
        <br />
        <br />
        <asp:Label ID="lblReceiveOnline"  AssociatedControlID="chkReceiveOnline" runat="server" Text="Receive Online Email" ></asp:Label>
        <asp:CheckBox ID="chkReceiveOnline" runat="server" />
        <br />
        <br />
        <asp:Label ID="lblActiveNotif"  AssociatedControlID="chkActiveNotif" runat="server" Text="Active" ></asp:Label>
        <asp:CheckBox ID="chkActiveNotif" runat="server" />
    </div>
    <br />
    <br />
    <asp:Button ID="btnAdd" runat="server" Text="Add Source" onclick="btnAdd_Click" />
</asp:Content>
