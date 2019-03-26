<%@ Page Title="Administration" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="AuthAdd.aspx.cs" Inherits="TRWeb.AuthAdd" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
        
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>ADD NOTIFICATION</h2>
    <div>Enter all information below then click Add Notification.</div>
    <br />
    <br />
    <div class="addfloater">
        <asp:Label ID="lblUserEmail"  AssociatedControlID="txtUserEmail" runat="server" Text="User Email" ></asp:Label>
        <br />
        <asp:TextBox CssClass="myTextBox" ID="txtUserEmail" runat="server" ></asp:TextBox>
        <br />
        <asp:Label CssClass="labelError" ID="lblUserEmailError" runat="server" ></asp:Label>
        <br />
        <br />
        <asp:Label ID="lblUserId"  AssociatedControlID="txtUserId" runat="server" Text="User ID" ></asp:Label>
        <br />
        <asp:TextBox CssClass="myTextBox" ID="txtUserId" runat="server" ></asp:TextBox>
        <br />
        <asp:Label CssClass="labelError" ID="lblUserIdError" runat="server" ></asp:Label>
        <br />
        <br />
        <asp:Label ID="lblFirstName"  AssociatedControlID="txtFirstName" runat="server" Text="First Name" ></asp:Label>
        <br />
        <asp:TextBox CssClass="myTextBox" ID="txtFirstName" runat="server" ></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="lblLastName"  AssociatedControlID="txtLastName" runat="server" Text="Last Name" ></asp:Label>
        <br />
        <asp:TextBox CssClass="myTextBox" ID="txtLastName" runat="server" ></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="lblUserGroup"  AssociatedControlID="cmbUserGroup" runat="server" Text="User Group" ></asp:Label>
        <asp:DropDownList CssClass="myTextBox" ID="cmbUserGroup" runat="server" ></asp:DropDownList>
        <br />
        <br />
        <asp:Label ID="lblActiveUser"  AssociatedControlID="chkActiveUser" runat="server" Text="Active" ></asp:Label>
        <asp:CheckBox ID="chkActiveUser" runat="server" />
        <br />
        <br />
        <asp:Button ID="btnAdd" runat="server" Text="Add User" onclick="btnAdd_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click" />
    </div>
</asp:Content>
