<%@ Page Title="User Administration" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    MaintainScrollPositionOnPostback="true"
    CodeBehind="AdminUser.aspx.cs" Inherits="TRWeb.AdminUser" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type = "text/javascript" language = "javascript">
        function confirm_proceed(message) {
            var chkbox = document.getElementById("<%=chkHidden.ClientID %>");
            chkbox.checked = confirm(message);
            return true; 
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>USER ADMINISTRATION</h2>
    <asp:CheckBox ID = "chkHidden" runat="server" Visible="false" ></asp:CheckBox>
    <br />
    <br />
    <div class="caseBottom">
        <div class="floater">
            <asp:Label ID="lblUserName"  AssociatedControlID="txtUserName" runat="server" Text="User Name" ></asp:Label>
            <asp:TextBox CssClass="myTextBox" ID="txtUserName" runat="server" ></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lblUserEmail"  AssociatedControlID="txtUserEmail" runat="server" Text="User Email" ></asp:Label>
            <asp:TextBox CssClass="myTextBox" ID="txtUserEmail" runat="server" ></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lblUserID"  AssociatedControlID="txtUserID" runat="server" Text="User ID" ></asp:Label>
            <br />
            <asp:TextBox CssClass="myTextBox" ID="txtUserID" runat="server" ></asp:TextBox>
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
            <asp:Label ID="lblTRAdministrator" AssociatedControlID="chkAdministrator" runat="server" Text="TR Administrator" ></asp:Label>
            <asp:CheckBox ID="chkAdministrator" runat="server" Enabled="false" />
        </div>
        <asp:Button ID="btnAddUser" runat="server" Text="Add User" onclick="btnAddUser_Click" />
        <br />
        <br />
        <asp:Button ID="btnUpdateUser" runat="server" Text="Update User" Enabled="false" onclick="btnUpdateUser_Click" />
    </div>
    <br />
    <br />
    <div>
        <asp:GridView ID="grdResult" runat="server" Width="915px" ShowFooter="True"
            onselectedindexchanged="grdResult_SelectedIndexChanged" OnPageIndexChanging="grdResult_PageIndexChanging"
            OnSorting="grdResult_Sorting" AutoGenerateSelectButton="True" AllowPaging="True" AllowSorting="True">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
        </asp:GridView>
    </div>
</asp:Content>
