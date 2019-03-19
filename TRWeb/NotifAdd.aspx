<%@ Page Title="Administration" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="NotifAdd.aspx.cs" Inherits="TRWeb.NotifAdd" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
        
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>ADD NOTIFICATION</h2>
<%--    <div>Enter all information below then click Add Notification.</div>--%>
    <div>Select a Notification List and an email to add a new Notification.</div>
    <br />
    <br />

    <div class="addfloater">
<%--        <asp:Label ID="lblSource"  AssociatedControlID="cmbSourceList" runat="server" Text="Source Code" ></asp:Label>--%>
        <asp:Label ID="lblSource"  AssociatedControlID="cmbSourceList" runat="server" Text="Notification List" />
        <br />
        <asp:DropDownList CssClass="headselection" ID="cmbSourceList" runat="server" ></asp:DropDownList>
        <br />
        <br />
<%--        <asp:Label ID="lblNotifEmailList"  AssociatedControlID="lstNotifEmailList" runat="server" Text="Notification Email" ></asp:Label>--%>
        <asp:Label ID="lblNotifEmailList"  AssociatedControlID="lstNotifEmailList" runat="server" Text="Available User Emails (or enter email below)" />
        <br />
        <asp:ListBox CssClass="myListBox" ID="lstNotifEmailList" runat="server" AutoPostBack="True" DataTextField="UserEmail" DataValueField="UserEmail" 
            OnSelectedIndexChanged="lstNotifEmailList_OnSelectedIndexChanged" />
        <br />
<%-- adding textbox to accept free-text email--itjas, 8/23/2018 --%>
        <asp:TextBox CssClass="myTextBox" ID="txtNotifEmail" runat="server" onkeypress="ClearText()" />
        <br />
        <asp:Label CssClass="labelError" ID="lblNotifEmailError" runat="server"></asp:Label>
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
        <asp:CheckBox ID="chkActiveNotif" runat="server" Checked="true" />
        <br />
        <br />
        <asp:Button ID="btnAdd" runat="server" Text="Add Notification" onclick="btnAdd_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click" />
    </div>
</asp:Content>
