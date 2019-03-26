<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="TRWeb._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<%--  <script type="text/javascript">
    function lnkEPay_Click()
    {
        myWindow = window.open('https://webapp.cityofmadison.com/epayment/epayfileprocessing/extract/processremittance.cfm', '_blank');
    }
  </script>--%>
    <h2 id="welcomeHeader" runat="server">
    </h2>
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
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnProcess" runat="server" Text="Process" onclick="btnProcess_Click" />
        <asp:LinkButton ID="lnkEPay" runat="server" OnClientClick="lnkEPay_Click()" >Epay</asp:LinkButton>
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" onclick="btnRefresh_Click" Visible="false" />
    </p>
    <p>
        <asp:Label ID="lblDirection" runat="server" Text=""></asp:Label>
        <asp:Table ID="tblFileNames" runat="server" CellPadding="5" HorizontalAlign="Left" Width="500px">
             <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblSelectDate" runat="server" Text="Select a Date to Process" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell><asp:Calendar ID="cldFileDate" Runat="server" OnSelectionChanged="cldFileDate_SelectionChanged" Width="350px">
                    <TodayDayStyle ForeColor="Black" BackColor="Gray"></TodayDayStyle>
                    <DayStyle Font-Bold="True" 
                            HorizontalAlign="Left" 
                            BorderWidth="1px" 
                            BorderStyle="Solid" 
                            BorderColor="Black" 
                            VerticalAlign="Top" 
                            BackColor="White"></DayStyle>
                    <NextPrevStyle ForeColor="Blue"/>
                    <DayHeaderStyle Font-Size="Large" Font-Bold="True" BorderWidth="1px" ForeColor="White" BorderStyle="Solid" BorderColor="Black" Width="100px" BackColor="#666666"></DayHeaderStyle>
                    <TitleStyle Font-Size="Large" Font-Bold="True" BorderWidth="1px" BorderStyle="Solid" ForeColor="White" BorderColor="Black" BackColor="#4b6c9e"></TitleStyle>
                    <WeekendDayStyle BackColor="#C0C0C0"></WeekendDayStyle></asp:Calendar></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblFiles" runat="server" Text="Files to Process" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell><asp:ListBox ID="lstFiles" runat="server" Width="350px" Height="140px" ></asp:ListBox></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblDCoPayDate" runat="server" Text="Dane Co. Payment Date" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtDCoPayDate" runat="server" Width="350px"></asp:TextBox>
                    <asp:RegularExpressionValidator  Runat="server" ID="valDateOnly" ControlToValidate="txtDCoPayDate" 
                        Display="Dynamic" ErrorMessage="&nbsp&nbspEnter a value in the format MM/DD/YYYY."  
                        ValidationExpression="^([0]\d|[1][0-2])\/([0-2]\d|[3][0-1])\/([2][01]|[1][6-9])\d{2}(\s([0-1]\d|[2][0-3])(\:[0-5]\d){1,2})?$" >
                    </asp:RegularExpressionValidator>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblDCoPenDate" runat="server" Text="Dane Co. Pen Date" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtDCoPenDate" runat="server" Width="350px"></asp:TextBox>
                    <asp:RegularExpressionValidator  Runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtDCoPenDate" 
                        Display="Dynamic" ErrorMessage="&nbsp&nbspEnter a value in the format MM/DD/YYYY."  
                        ValidationExpression="^([0]\d|[1][0-2])\/([0-2]\d|[3][0-1])\/([2][01]|[1][6-9])\d{2}(\s([0-1]\d|[2][0-3])(\:[0-5]\d){1,2})?$" >
                    </asp:RegularExpressionValidator>                    
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblDCoTaxYear" runat="server" Text="Dane Co. Tax Year" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtDCoTaxYear" runat="server" Width="350px"></asp:TextBox>
                    <asp:RegularExpressionValidator  Runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtDCoTaxYear" 
                        Display="Dynamic" ErrorMessage="&nbsp&nbspEnter a value in the format YYYY."  
                        ValidationExpression="^\d{4}$" >
                    </asp:RegularExpressionValidator> 
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblCashCode" runat="server" Text="CashCode" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtCashCode" runat="server" Width="350px"></asp:TextBox>
                    <asp:RegularExpressionValidator  Runat="server" ID="RegularExpressionValidator3" ControlToValidate="txtCashCode" 
                        Display="Dynamic" ErrorMessage="&nbsp&nbspEnter a value from 01 to 99."  
                        ValidationExpression="^\d{2}$" >
                    </asp:RegularExpressionValidator>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblPostDate" runat="server" Text="Post Date" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtPostDate" runat="server" Width="350px"></asp:TextBox>
                    <asp:RegularExpressionValidator  Runat="server" ID="RegularExpressionValidator4" ControlToValidate="txtPostDate" 
                        Display="Dynamic" ErrorMessage="&nbsp&nbspEnter a value in the format MM/DD/YYYY."  
                        ValidationExpression="^([0]\d|[1][0-2])\/([0-2]\d|[3][0-1])\/([2][01]|[1][6-9])\d{2}(\s([0-1]\d|[2][0-3])(\:[0-5]\d){1,2})?$" >
                    </asp:RegularExpressionValidator> 
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblBeginSeq" runat="server" Text="Sequence Number Begin" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtBeginSeq" runat="server" Width="350px"></asp:TextBox>
                    <asp:RegularExpressionValidator  Runat="server" ID="RegularExpressionValidator5" ControlToValidate="txtBeginSeq" 
                        Display="Dynamic" ErrorMessage="&nbsp&nbspEnter a value from 000000 to 999999."  
                        ValidationExpression="^\d{6}$" >
                    </asp:RegularExpressionValidator>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:Label ID="lblEndSeq" runat="server" Text="Sequence Number End" Width="150px"></asp:Label></asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtEndSeq" runat="server" Width="350px"></asp:TextBox>
                    <asp:RegularExpressionValidator  Runat="server" ID="RegularExpressionValidator6" ControlToValidate="txtEndSeq" 
                        Display="Dynamic" ErrorMessage="&nbsp&nbspEnter a value from 000000 to 999999."   
                        ValidationExpression="^\d{6}$" >
                    </asp:RegularExpressionValidator>
                </asp:TableCell>
            </asp:TableRow>
         </asp:Table>
         <asp:Label ID="lblResult" runat="server" Text="Message Window" Height="20px" ></asp:Label>
         <br />
         <asp:TextBox ID="txtResult" runat="server" Height="350px" TextMode="MultiLine" ReadOnly="True" Width="350px"></asp:TextBox>
    </p>
</asp:Content>