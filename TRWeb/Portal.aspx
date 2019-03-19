<%@ Page Title="Portal Administration" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Portal.aspx.cs" Inherits="TRWeb.Portal" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>Portal</h2>
    <h3>Please select a Group to work with.  The Group that's selected defines the functions that can be performed.</h3>
    <p>
        <asp:Label ID="lblGroup" runat="server" Text="Select Group"></asp:Label>
        <asp:DropDownList ID="cmbGroup" runat="server" AutoPostBack="True"
            onselectedindexchanged="cmbGroup_SelectedIndexChanged" Height="25px" Width="200px" 
            style="margin-left: 14px"></asp:DropDownList>
    </p>
    <p>
        <asp:Label ID="lblFunction" runat="server" Text="Select Function"></asp:Label>
        <asp:DropDownList ID="cmbFunction" runat="server" AutoPostBack="True" 
            onselectedindexchanged="cmbFunction_SelectedIndexChanged" Height="25px" Width="400px" ></asp:DropDownList> 
    </p>
    <p>
        <asp:Label ID="lblDirection" runat="server" Text=""></asp:Label>
    </p>
    <div>
        <asp:Table ID="tblPortal" runat="server" CellPadding="5" 
            HorizontalAlign="Left" Width="530px">
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblOwnerName" runat="server" Text="Owner Name" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell><asp:TextBox ID="txtOwnerName" runat="server" Width="350px"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblOwnerEmail" runat="server" Text="Owner Email" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell><asp:TextBox ID="txtOwnerEmail" runat="server" Width="350px"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblEmail" runat="server" Text="Email" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell><asp:TextBox ID="txtEmail" runat="server" Width="350px"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblPortalID" runat="server" Text="Portal ID" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtPortalID" runat="server" Width="350px"></asp:TextBox>
                        <asp:RegularExpressionValidator  Runat="server" ID="valNumbersOnly" ControlToValidate="txtPortalID" 
                        Display="Dynamic" ErrorMessage="&nbsp&nbspPlease enter only numbers."  
                        ValidationExpression="^[0-9]+$" >
                    </asp:RegularExpressionValidator>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblActive" runat="server" Text="Active" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell><asp:TextBox ID="txtActive" runat="server" Width="350px"></asp:TextBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>

            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" /></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:Label ID="lblResult" runat="server" Text="Message Window"></asp:Label>
        <asp:TextBox ID="txtResult" runat="server" Height="210px" TextMode="MultiLine" Width="378px" ReadOnly="True"></asp:TextBox>
        <asp:Table ID="tblMethods" runat="server" CellPadding="5" HorizontalAlign="Left" >
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Label ID="lblAvailableMethods" runat="server" Text="Available Methods" Width="150px"></asp:Label>
                     <br />
                    <asp:ListBox ID="lstAvailableMethods" runat="server" Width="250px" SelectionMode="Multiple" ></asp:ListBox>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnAdd" runat="server" Text="Add >>" Width="100px" onclick="btnAdd_Click" />
                     <br />
                     <br />
                    <asp:Button ID="btnRemove" runat="server" Text="<< Remove" Width="100px" onclick="btnRemove_Click" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="lblMethods" runat="server" Text="Assigned Methods" Width="150px" ></asp:Label>
                    <br />
                    <asp:ListBox ID="lstMethods" runat="server" Width="250px" SelectionMode="Multiple" ></asp:ListBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Button ID="btnUpdate" runat="server" Text="Update" onclick="btnUpdate_Click" />&nbsp;&nbsp;
                    <asp:Button ID="btnNewKey" runat="server" Text="Request New Key" onclick="btnNewKey_Click" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
        <br />
        <br />
        <asp:GridView ID="grdResult" runat="server" Width="915px" ShowFooter="True"
            onselectedindexchanged="grdResult_SelectedIndexChanged" AutoGenerateSelectButton="True">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
        </asp:GridView>
    </div>
</asp:Content>
