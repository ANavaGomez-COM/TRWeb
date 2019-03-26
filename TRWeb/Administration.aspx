<%@ Page Title="Administration" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    MaintainScrollPositionOnPostback="true"
    CodeBehind="Administration.aspx.cs" Inherits="TRWeb.Administration" %>

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
    <h2>ADMINISTRATION</h2>
    <div>Use the boxes below to load related Source, Notification and User information.</div>
    <%--adding environmental settings comparison between URL and web.config--itjas, 9/26/2018--%>
    <div style="float:right">
        <asp:Label ID="lblWebConfig" runat="server" Text="Environment: "></asp:Label>
        <asp:Label ID="lblWebConfig_env" runat="server" ></asp:Label>
        <br />
        <asp:Button ID="btnShowHide" runat="server" OnClick="btnShowHide_Clicked" Text="Show Detail" />
    </div>
    <asp:TextBox ID="txtWebConfigDet" runat="server" Height="300" OnClick="copyToClipboard(this)" ReadOnly="true" TextMode="MultiLine" Width ="100%" Visible="false" />
    <script>
        function copyToClipboard(pControl) {
            const cScrollTop = $(window).scrollTop();               //save off the current scroll position
            const cElement = document.createElement('textarea');    //create new <textarea> element 
            cElement.value = pControl.value;                        //set its value to the string that you want copied 
            cElement.setAttribute('readonly', '');                  //make it readonly to be tamper-proof 
            cElement.style.position = 'absolute';                  
            cElement.style.left = '-9999px';                        //move outside the screen to make it invisible 
            document.body.appendChild(cElement);                    //append the <textarea> element to the HTML document 
            const selected = document.getSelection().rangeCount > 0 //check if there is any content selected previously 
                ? document.getSelection().getRangeAt(0)             //store selection if found 
                : false;                                            //mark as false to know no selection existed before 
            cElement.select();                                      //select the <textarea> content 
            document.execCommand('copy');                           //copy - only works as a result of a user action (e.g. click events) 
            document.body.removeChild(cElement);                    //remove the <textarea> element 
            if (selected) {                                         //if a selection existed before copying 
                document.getSelection().removeAllRanges();          //unselect everything on the HTML document 
                document.getSelection().addRange(selected);         //restore the original selection 
            }
            $(window).scrollTop(cScrollTop);                        //restore the scroll position
            pControl.select;
            var vTxtbox = document.getElementById("<%=txtWebConfigDet.ClientID%>");
            vTxtbox.style.backgroundColor = 'LightYellow';
        }
        
    </script>
    <br />
    <asp:CheckBox ID = "chkHidden" runat="server" Visible="false" ></asp:CheckBox>

    <div id="headcontent">
        <p>
            <asp:Label CssClass="mylabel" ID="lblSearchType" runat="server" Text="Select Search Type" ></asp:Label>
            <asp:DropDownList CssClass="headselection" ID="cmbSearchType" runat="server" AutoPostBack="True"
                onselectedindexchanged="cmbSearchType_SelectedIndexChanged" ></asp:DropDownList>
        </p>
        <div style="float:right">
            <asp:Button ID="btnRunTest" runat="server" Text="Run ERPLoadTest.dtsx" OnClick="btnRunTest_Click" />
            <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
        </div>
        <p>
            <asp:Label CssClass ="mylabel" ID="lblSearch" runat="server" Text="Source Type" ></asp:Label>
            <asp:DropDownList CssClass="headselection" ID="cmbSearchList" runat="server" AutoPostBack="True"
                onselectedindexchanged="cmbSearchList_SelectedIndexChanged" ></asp:DropDownList>
            <asp:TextBox CssClass="headselection" ID="txtSearchSingle" runat="server" ></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="btnSearch" runat="server" Text="Search" onclick="btnSearch_Click" />
        </p>
    </div>
    <br />
    <div class="case">
        <div class="floater">
            <asp:Label ID="lblSourceList"  AssociatedControlID="lstSourceList" runat="server" Text="Source List" ></asp:Label>
            <br />
            <asp:ListBox CssClass="myListBox" ID="lstSourceList" runat="server" AutoPostBack="True"
             OnSelectedIndexChanged="lstSourceList_OnSelectedIndexChanged" ></asp:ListBox>
        </div>
        <div class="floater">
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
        <asp:Button ID="btnAddSource" runat="server" Text="Add Source" onclick="btnAddSource_Click" />
        <br />
        <br />
        <asp:Button ID="btnUpdateSource" runat="server" Text="Update Source" onclick="btnUpdateSource_Click" />
        <br />
        <br />
        <asp:Button ID="btnDeleteSource" runat="server" Text="Delete Source" onclick="btnDeleteSource_Click" />
        <br />
        <br />
        <asp:Button ID="btnListSources" runat="server" Text="List All Sources" onclick="btnListSources_Click" />
    </div>
    <br />
    <div class="case">
        <div class="floater">
            <asp:Label ID="lblNotifEmailList"  AssociatedControlID="lstNotifEmailList" runat="server" Text="Notification Email List" ></asp:Label>
            <br />
            <asp:ListBox CssClass="myListBox" ID="lstNotifEmailList" runat="server" AutoPostBack="True" 
             OnSelectedIndexChanged="lstNotifEmailList_OnSelectedIndexChanged" ></asp:ListBox>
        </div>
        <div class="floater">
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
        <asp:Button ID="btnUpdateEmailNotif" runat="server" Text="Update Notification" onclick="btnUpdateEmailNotif_Click" />
        <br />
        <br />
        <asp:Button ID="btnListNotifications" runat="server" Text="List All Notifications" onclick="btnListNotifications_Click" />
    </div>
    <br />
    <div class="caseBottom">
        <div class="floater">
            <asp:Label ID="lblUserEmailList"  AssociatedControlID="lstUserEmailList" runat="server" 
                Text="User Email List" ></asp:Label>
            <br />
            <asp:ListBox CssClass="myListBox" ID="lstUserEmailList" runat="server" AutoPostBack="True"
             OnSelectedIndexChanged="lstUserEmailList_OnSelectedIndexChanged" ></asp:ListBox>
        </div>
        <div class="floater">
            <asp:Label ID="lblUserName"  AssociatedControlID="txtUserName" runat="server" Text="User Name" ></asp:Label>
            <asp:TextBox CssClass="myTextBox" ID="txtUserName" runat="server" ></asp:TextBox>
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
            <asp:CheckBox ID="chkAdministrator" runat="server" />
        </div>
        <asp:Button ID="btnAddUser" runat="server" Text="Add User" onclick="btnAddUser_Click" />
        <br />
        <br />
        <asp:Button ID="btnUpdateUser" runat="server" Text="Update User" onclick="btnUpdateUser_Click" />
        <br />
        <br />
        <asp:Button ID="btnListUsers" runat="server" Text="List All Users" onclick="btnListUsers_Click" />
    </div>
    <br />
<%--                        <asp:RegularExpressionValidator  Runat="server" ID="valNumbersOnly" ControlToValidate="txtPortalID" 
                        Display="Dynamic" ErrorMessage="&nbsp&nbspPlease enter only numbers."  
                        ValidationExpression="^[0-9]+$" >
                    </asp:RegularExpressionValidator>--%>
    <div>
        <asp:GridView ID="grdResult" runat="server" Width="915px" ShowFooter="True"
            onselectedindexchanged="grdResult_SelectedIndexChanged" OnPageIndexChanging="grdResult_PageIndexChanging"
            OnSorting="grdResult_Sorting" AutoGenerateSelectButton="True" AllowPaging="True" AllowSorting="True">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
        </asp:GridView>
    </div>
</asp:Content>
