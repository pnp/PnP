
<%--
 Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Core.ODataBatchWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body bgcolor="#000000">
    <form id="form1" runat="server">
    <div>
        <h1 style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal; color:white">
            Using the $batch query option with the SharePoint REST APIs
        </h1>  
        <p style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal; color:white">
            This sample is most useful if you are using the <a href="http://www.telerik.com/fiddler">Fiddler tool</a> to examine the HTTP requests and responses.
          </p>      
        <h2 style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal; color:white">Batch Job 1: Two GETs</h2>

        <p style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal; color:white">
            Click the button to get the <b>Users</b> list and the <b>Composed Looks</b> list in a single call to SharePoint.<br />
            Then scroll down to try batch job #2.
         <asp:Literal ID="Literal1" runat="server"><br /></asp:Literal>
            <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" 
        Text="BATCH REQUEST 1" BackColor="#00FFFF" ForeColor="Black" Font-Size="Large" 
        Style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal" 
        Height="50px" Width="239px" />&nbsp;&nbsp;&nbsp;
        </p>

        <asp:Table ID="TwoLists" runat="server">
            <asp:TableRow VerticalAlign="Top" >
                <asp:TableCell>   
                    <asp:GridView ID="GridView1" runat="server"  BackColor="#808080" ForeColor="White"
                        BorderColor="#0033CC" BorderStyle="Solid" Caption="Users" 
                        CaptionAlign="Left" CellPadding="5" Style="font-family: 'Segoe UI'" GridLines="None" 
                        HorizontalAlign="Left">
                        <AlternatingRowStyle BackColor="White" ForeColor="Black" />
                    </asp:GridView>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:GridView ID="GridView2" runat="server" BackColor="#808080" ForeColor="White"
                        BorderColor="#0033CC" BorderStyle="Solid" Caption="Composed Looks" 
                        CaptionAlign="Left" CellPadding="5" Style="font-family: 'Segoe UI'" GridLines="None" 
                        HorizontalAlign="Left" Width="200">
                        <AlternatingRowStyle BackColor="White" ForeColor="Black" />
                    </asp:GridView>
               </asp:TableCell>
       </asp:TableRow>

        </asp:Table>
</div>
        <div>
          <asp:Table ID="Table1" runat="server">
            <asp:TableRow VerticalAlign="Top" >
                <asp:TableCell>  
        <asp:Literal ID="Literal8" runat="server"><br /><br /></asp:Literal>

        <h2 style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal; color:white">Batch job 2: POST and GET</h2>
        <p style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal; color:white">
            Enter a new list title in the box and then click the button to add a new list and get the <b>List of Lists</b> in a single call to SharePoint.<br />
            Then scroll down to try batch job #3.</p>
        <asp:Literal ID="Literal2" runat="server"><br /></asp:Literal>
        <asp:TextBox ID="NewList" runat="server">Enter new list title.</asp:TextBox>&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" 
        Text="BATCH REQUEST 2" BackColor="#00FFFF" ForeColor="Black" Font-Size="Large" 
        Style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal" 
        Height="50px" Width="239px" />&nbsp;&nbsp;&nbsp;
        
        <asp:Label ID="AddListResponse" runat="server" Style="font-family: 'Segoe UI'; color:white"></asp:Label>

        <asp:GridView ID="GridView3" runat="server" BackColor="#808080" ForeColor="Black"
        BorderColor="#0033CC" BorderStyle="Solid" Caption="List of Lists" 
        CaptionAlign="Left" CellPadding="5" Style="font-family: 'Segoe UI'" GridLines="None" 
        HorizontalAlign="Left" Width="200">
        <AlternatingRowStyle BackColor="White" ForeColor="Black" />
              </asp:GridView>
               </asp:TableCell>
           </asp:TableRow>
         </asp:Table>
       </div>

        <div>

         <asp:Literal ID="Literal7" runat="server"><br /><br /></asp:Literal>

        <h2 style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal; color:white">Batch Job 3: DELETE and GET</h2>
        <p style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal; color:white"> 
         Enter the name of a list to delete and then click the button delete it and get the updated <b>List of Lists</b> in a single call to SharePoint.<br />
        <i>Delete only lists that were created with this app. Deleting built-in SharePoint lists or lists created with other apps could have unpredictable effects.</i></p>
        <asp:Literal ID="Literal5" runat="server"><br /></asp:Literal>
        <asp:TextBox ID="OldList" runat="server">Enter existing list title.</asp:TextBox>&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button4" runat="server" OnClick="Button4_Click" 
        Text="BATCH REQUEST 3" BackColor="#00FFFF" ForeColor="Black" Font-Size="Large" 
        Style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal" 
        Height="50px" Width="239px" />&nbsp;&nbsp;&nbsp;
       
        <asp:Label ID="DeleteListResponse" runat="server" Style="font-family: 'Segoe UI'; color:white"></asp:Label>

        <asp:Literal ID="Literal6" runat="server"><br /><br /></asp:Literal>
        <asp:GridView ID="GridView4" runat="server" BackColor="#808080" ForeColor="White"
        BorderColor="#0033CC" BorderStyle="Solid" Caption="List of Lists" 
        CaptionAlign="Left" CellPadding="5" Style="font-family: 'Segoe UI'" GridLines="None" 
        HorizontalAlign="Left" Width="200">
        <AlternatingRowStyle BackColor="White" ForeColor="Black" />
              </asp:GridView>

        </div>
        

    </form>
</body>
</html>


