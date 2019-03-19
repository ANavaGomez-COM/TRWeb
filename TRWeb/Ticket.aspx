<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Ticket.aspx.cs" Inherits="TRWeb.Ticket" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2 id="welcomeHeader" runat="server">
    </h2>
    <p>
        <asp:Label ID="lblFunction" runat="server" Text="Select Function"></asp:Label>
        <asp:DropDownList ID="cmbFunction" runat="server" AutoPostBack="True" 
            onselectedindexchanged="cmbFunction_SelectedIndexChanged" Height="25px" Width="400px" ></asp:DropDownList> 
        &nbsp;&nbsp;&nbsp;&nbsp;
    </p>
    <p>
        <asp:Label ID="lblDirection" runat="server" Text=""></asp:Label>
    </p>
    <div>
    <asp:Table ID="tblParkingTicket" runat="server" CellPadding="5" 
        HorizontalAlign="Left" Width="530px">
        <asp:TableRow>
            <asp:TableCell><asp:Label ID="lblTicketNumber" AssociatedControlID="txtTicketNumber" runat="server" 
                Text="Ticket Number" Width="150px"></asp:Label></asp:TableCell>
            <asp:TableCell><asp:TextBox ID="txtTicketNumber" runat="server" Width="350px"></asp:TextBox></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell><asp:Label ID="lblLicensePlateNum" runat="server" AssociatedControlID="txtLicensePlateNum" 
                Text="License Plate Number" Width="150px"></asp:Label></asp:TableCell>
            <asp:TableCell><asp:TextBox ID="txtLicensePlateNum" runat="server" Width="350px"></asp:TextBox></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell><asp:Label ID="lblLicensePlateSt" runat="server" AssociatedControlID="txtLicensePlateSt" 
                Text="License Plate State" Width="150px"></asp:Label></asp:TableCell>
            <asp:TableCell><asp:TextBox ID="txtLicensePlateSt" runat="server" Width="350px"></asp:TextBox></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell><asp:Label ID="lblLicensePlateTyp" runat="server" AssociatedControlID="txtLicensePlateTyp" 
                Text="License Plate Type" Width="150px"></asp:Label></asp:TableCell>
            <asp:TableCell><asp:TextBox ID="txtLicensePlateTyp" runat="server" Width="350px"></asp:TextBox></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell><asp:Label ID="lblDate" runat="server" Text="Violation Date" Width="150px"></asp:Label></asp:TableCell>
            <asp:TableCell><asp:TextBox ID="txtDate" runat="server" Width="350px" ontextchanged="txtDate_TextChanged"></asp:TextBox>
                <asp:RegularExpressionValidator  Runat="server" ID="valDateOnly" ControlToValidate="txtDate" 
                    Display="Dynamic" ErrorMessage="&nbsp&nbspPlease enter a value in the format MM/DD/YYYY."  
                    ValidationExpression="^([0]\d|[1][0-2])\/([0-2]\d|[3][0-1])\/([2][01]|[1][6-9])\d{2}(\s([0-1]\d|[2][0-3])(\:[0-5]\d){1,2})?$" >
                </asp:RegularExpressionValidator>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell><asp:Label ID="lblBalanceDue" runat="server" Text="Balance Due" Width="150px"></asp:Label></asp:TableCell>
            <asp:TableCell><asp:TextBox ID="txtBalanceDue" runat="server" Width="350px" ontextchanged="txtBalanceDue_TextChanged"></asp:TextBox>
                <asp:RegularExpressionValidator  Runat="server" ID="valNumbersOnly" ControlToValidate="txtBalanceDue" 
                    Display="Dynamic" ErrorMessage="&nbsp&nbspPlease enter a value in the format 0.00."  
                    ValidationExpression="^\s*\$?\s*\d{1,3}((,\d{3})*|\d*)(\.\d{2})?\s*$" >
                </asp:RegularExpressionValidator>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell><asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" /></asp:TableCell>
            <asp:TableCell>
                <asp:Button ID="btnMakePay" runat="server" Text="Make Payment" onclick="btnMakePay_Click" />&nbsp;&nbsp;
                <asp:Button ID="btnVoidPay" runat="server" Text="Void Payment" onclick="btnVoidPay_Click" />            
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Label ID="lblResult" runat="server" Text="Message Window"></asp:Label>
    <asp:TextBox ID="txtResult" runat="server" Height="210px" TextMode="MultiLine" 
            Width="378px" ReadOnly="True"></asp:TextBox>
        <br />
        <br />
        <br />
        <asp:GridView ID="grdResult" runat="server" Width="915px" ShowFooter="True"
            onselectedindexchanged="grdResult_SelectedIndexChanged" AutoGenerateSelectButton="True" >
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
        </asp:GridView>
    </div>
    <br />
</asp:Content>