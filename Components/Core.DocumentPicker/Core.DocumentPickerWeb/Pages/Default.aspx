<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.DocumentPickerWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Core.Documentpicker</title>
    <link href="../Styles/documentpicker/documentpickercontrol.css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/MyCustomDocumentPickerDataSource.js"></script>
    <script type="text/javascript" src="../Scripts/documentpickerdatasource.js"></script>
    <script type="text/javascript" src="../Scripts/documentpickercontrol.js"></script>
    <script type="text/javascript" src="../Scripts/app.js"></script>
</head>
<body style="overflow:auto !important; display: none;">

    <form id="form1" runat="server">
        <asp:HiddenField ID="defaultDocumentUrl" runat="server" />
        <asp:HiddenField ID="defaultDocumentPath" runat="server" />
        <asp:HiddenField ID="DocList1Id" runat="server" />
        <asp:HiddenField ID="DocList2Id" runat="server" />


        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" />
        <div id="divSPChrome"></div>

        <div style="left: 50%; width: 500px; margin-left: -250px; position: absolute;">
         
           <div class="ms-core-form-line">
                <asp:Label runat="server" ID="OutputLabel"></asp:Label><br />
            </div>
            <div class="ms-core-form-line" >
                <h1>Basic document picker</h1> 
            </div>

            <%-- BasicDocumentPicker --%>
            <div class="ms-core-form-line" style="height:100px;">
                <span style="width:150px;float:left;">select documents:</span>
                <div id="BasicDocumentPicker" style="width:350px;height:100px;float:right"></div>  <%-- Div to host the control --%>
                <asp:HiddenField runat="server" ID="BasicDocumentPickerValue" /> <%-- Hiddenfield that will contain the output value of the control --%>
            </div>
            
            <div class="ms-core-form-line" style="height:50px;">
                <asp:Button runat="server" ID="GetValuesButton" OnClick="GetValuesButton_Click" Text="Get values by server" />
                <button id="GetValuesByJs">Get values by javascript</button>
                <button id="SetValuesByJs">Set values by javascript</button>
            </div>
            <div class="ms-core-form-line">
                <h1>Document picker with options</h1> 
                <span>Showing 2 document libraries. Smaller size by css styling. Only xlsx and docx documents are shown. Only 1 document can be selected. Folders collapsed.</span>
            </div>

            <%-- DocumentPickerWithOptions --%>
            <div class="ms-core-form-line" style="height:50px;">
                <span style="width:150px;float:left;">select documents:</span>
                <!--use width and heigt styling on the div to determine the size of the control-->
                <div id="DocumentPickerWithOptions" style="width:350px;height:25px;float:right"></div>  
                <asp:HiddenField runat="server" ID="DocumentPickerWithOptionsValue" />    
            </div>
            
            <div class="ms-core-form-line">
                <h1>Doc picker with custom datasource</h1> 
                <span>Get the data for the control any way you want.</span>
            </div>

            <%-- DocumentPickerWithCustomDatasource --%>
            <div class="ms-core-form-line" style="height:75px;">
                <span style="width:150px;float:left;">select documents:</span>
                <div id="DocumentPickerWithCustomDataSource" style="width:350px;height:50px;float:right"></div>  
                <asp:HiddenField runat="server" ID="DocumentPickerWithCustomDataSourceValue" />    
            </div>
        </div>
    </form>
</body>
</html>
