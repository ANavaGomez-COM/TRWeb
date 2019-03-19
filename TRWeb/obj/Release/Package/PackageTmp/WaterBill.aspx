<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="WaterBill.aspx.cs" Inherits="TRWeb.WaterBill" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2 id="welcomeHeader" runat="server">
    </h2>
    <p>
        <asp:Label ID="lblFunction" runat="server" Text="Select Function"></asp:Label>
        <asp:DropDownList ID="cmbFunction" runat="server" AutoPostBack="True" 
            onselectedindexchanged="cmbFunction_SelectedIndexChanged" Height="25px" Width="400px" ></asp:DropDownList> 
    </p>
    <p>
        <asp:Label ID="lblDirection" runat="server" Text=""></asp:Label>
    </p>
    <div>
    <asp:Table ID="tblWater" runat="server" CellPadding="5" 
        HorizontalAlign="Left" Width="530px">
        <asp:TableRow>
            <asp:TableCell><asp:Label ID="lblFirst" runat="server" AssociatedControlID="txtFirst" Width="150px"></asp:Label></asp:TableCell>
            <asp:TableCell><asp:TextBox ID="txtFirst" runat="server" Width="350px"></asp:TextBox></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell><asp:Label ID="lblSecond" runat="server" AssociatedControlID="txtSecond" 
                Width="150px"></asp:Label></asp:TableCell>
            <asp:TableCell><asp:TextBox ID="txtSecond" runat="server" Width="350px"></asp:TextBox></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell><asp:Label ID="lblUnit" runat="server" AssociatedControlID="txtUnit" 
                Text="Unit" Width="150px"></asp:Label></asp:TableCell>
            <asp:TableCell><asp:TextBox ID="txtUnit" runat="server" Width="350px"></asp:TextBox></asp:TableCell>
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