<%@ Page Title="Water Admin Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="WaterAdmin.aspx.cs" Inherits="TRWeb.WaterAdmin" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
     <script type = "text/javascript">
        function confirm_upload() {
            document.getElementById("MainContent_UploadButton").disabled = false;
         }
    </script>
     <style type="text/css">

ol
	{margin-bottom:0in;}
         .auto-style1 {
             font-size: medium;
         }
     </style>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p class="auto-style1">
        Please use this screen to update the Water Customer and Account Number data. This data is used when the Treasurer&#39;s imports the Bill Consolidator file and acts as a lookup.</p>
    <p class="auto-style1">
        &nbsp;</p>
    <p>
        This is an import into Middleware, so the file has very specific requirements.<o:p></o:p></p>
    <ol start="1" type="1">
        <li class="MsoNormal">It must be an excel file<o:p> and start with &quot;<strong>Cust</strong>&quot;, i.e CustAcct_20180108.xlsx</o:p></li>
        <li class="MsoNormal">It must contain all the customer and account numbers (i.e., this process neither adds nor keeps previous records).</li>
        <li class="MsoNormal">The excel file should have two columns, column A should be &quot;<strong>Customer #</strong>&quot; and B &quot;<strong>Account #</strong>&quot;<o:p></o:p></li>
    </ol>
    <p>
        To import the file, please follow these steps<o:p></o:p></p>
    <ol start="1" type="1">
        <li class="MsoNormal">Click the Browse button<o:p></o:p></li>
        <li class="MsoNormal">Select the Excel file<o:p></o:p></li>
        <li class="MsoNormal">Click the Upload File Button - there should be some information displayed<o:p></o:p></li>
        <li class="MsoNormal">Now click the Run Import button<o:p></o:p></li>
    </ol>
    <p>
        You should receive an email with the results of the import<o:p></o:p></p>
    <br />
    <asp:FileUpload ID="Upload" runat="server" Width="473px"   onclick="confirm_upload()" ToolTip=".xls or .xlsx files only" />
    <%--    <asp:RequiredFieldValidator ErrorMessage="Required" ControlToValidate="Upload"
      runat="server" Display="Dynamic" ForeColor="Red" />
     <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.xls|.xlsx)$"
    ControlToValidate="Upload" runat="server" ForeColor="Red" ErrorMessage="Please select a valid Excel file."
    Display="Dynamic" />--%>
    <br />
    <br />
    <asp:Button ID="UploadButton" runat="server" OnClick="UploadButton_Click" Text="Upload File" Enabled="False" />
    <asp:Label ID="lblUpload" runat="server" Font-Bold="True" ForeColor="Red" ></asp:Label>
    <br />
    <asp:Label ID="UploadDetails" runat="server"> </asp:Label>
    <br />
    <asp:Button ID="RunImport" runat="server" Text="Run Import" Width="123px" OnClick="RunImport_Click" Enabled="False" />
    <asp:Label ID="lblSSIS" runat="server"></asp:Label>
    <br />
    

    </asp:Content>


