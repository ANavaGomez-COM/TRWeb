<%@ Page Title="Notification Administration" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    MaintainScrollPositionOnPostback="true"
    CodeBehind="AdminNotify.aspx.cs" Inherits="TRWeb.AdminNotify" %>

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
    <h2>NOTIFICATION ADMINISTRATION</h2>
    <asp:CheckBox ID = "chkHidden" runat="server" Visible="false" ></asp:CheckBox>

    <br />
    <div class="case" >
        <div class="floater">
            <asp:Label ID="lblSource"  AssociatedControlID="txtSource" runat="server" Text="Source Code" ></asp:Label>
            <asp:TextBox CssClass="myTextBox" ID="txtSource" runat="server" AutoPostBack="true" Enabled="false" ></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lblNotifEmail"  AssociatedControlID="txtNotifEmail" runat="server" Text="Notification Email" ></asp:Label>
            <asp:TextBox CssClass="myTextBox" ID="txtNotifEmail" runat="server" Enabled="false" ></asp:TextBox>
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
        <asp:Button ID="btnAddEmailNotif" runat="server" Text="Add Notification" onclick="btnAddEmailNotif_Click" />
        <br />
        <br />
        <asp:Button ID="btnUpdateEmailNotif" runat="server" Text="Update Notification" onclick="btnUpdateEmailNotif_Click" Enabled="false" />
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
