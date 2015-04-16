<%@ Page Language="C#" Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=16.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" meta:progid="SharePoint.WebPartPage.Document" meta:webpartpageexpansion="full" %>

<%@ Register TagPrefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="PublishingNavigation" Namespace="Microsoft.SharePoint.Publishing.Navigation" Assembly="Microsoft.SharePoint.Publishing, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:content contentplaceholderid="PlaceHolderPageTitle" runat="server">
	<SharePointWebControls:FieldValue id="PageTitle" FieldName="Title" runat="server"/>
</asp:content>
<asp:content contentplaceholderid="PlaceHolderMain" runat="server">
<script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.1.min.js"></script>
<script type="text/javascript">
    function getQuerystring(key) {
        var query;
        var result = location.search.match(new RegExp("[\?\&]" + key + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            query = "";
        }
        else {
            query = result[1];
        }
        return query;
    }

    $(function () {
        $('.cdsm_left a[href*="' + window.location.pathname + '?"]').addClass("current");
        $(".collapse_arrow").click(function () {
            $(this).siblings('ul').slideToggle(500);
            $(this).toggleClass('collapsed');
        });
    });
</script>
<style type="text/css">
    .cdsm_suportcase_page {
        font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
        font-size: 13px;
        min-width:1223px;
        overflow:auto;
    }

    #sideNavBox {
        display: none;
    }

    #contentBox {
        margin-left: 20px;
    }

    .cdsm_left {
        padding: 15px 15px 300px 30px;
        width: 242px;
        box-sizing: border-box;
        border: 1px solid #999;
        border-radius: 5px;
        float: left;
        font-size: 14px;
    }

    .cdsm_right {
        float: left;
        padding: 15px 15px;
        margin-left: 10px;
        width:900px;
    }

    .cdsm-button {
        padding: 5px 5px;
        border: 1px solid #000;
    }

    menu, ol, ul {
        list-style: none;
        padding: 0;
        margin: 0;
    }

    a, a:active, a:visited {
        color: #000;
        text-decoration: none;
    }

    .cdsm_mainmenu ul li > ul {
        margin-left: 16px;
    }

    .collapse_arrow {
        content: '';
        display: inline-block;
        width: 0;
        height: 0;
        border: 6px solid #808080;
        vertical-align: middle;
        border-left-color: transparent;
        border-bottom-color: transparent;
        border-right-color: transparent;
        border-bottom-width: 3px;
        margin-left: -16px;
        margin-top: 2px;
    }

    .collapsed {
        border-left-color: #808080;
        border-top-color: transparent;
        border-bottom-width: 6px;
        border-right-width: 1px;
        margin-left: -11px;
        margin-top: -2px;
    }

    .cdsm_left ul li a {
        background-color: transparent;
        padding: 2px 2px;
        line-height: 20px;
    }

        .cdsm_left ul li a.current {
            background-color: #ff8284;
            padding: 2px 2px;
        }

    .cdsm_suportcase_page .cdsm_top {
        padding: 9px 12px 9px 24px;
        overflow: hidden;
        border: 1px solid #999;
        border-radius: 5px;
    }

        .cdsm_suportcase_page .cdsm_top .cdsm_title {
            font-size: 15px;
            font-weight: bold;
            float: left;
        }

        .cdsm_suportcase_page .cdsm_top .cdsm_dplist {
            float: left;
            margin-left: 12px;
        }

            .cdsm_suportcase_page .cdsm_top .cdsm_dplist select {
                width: 570px;
                font-size: 12px;
                font-weight: bold;
                padding-left: 7px;
                border: 1px solid black;
            }

        .cdsm_suportcase_page .cdsm_top .ms-webpartzone-cell {
            margin: 0px 0px;
        }

    .cdsm_common_display {
        border: 1px solid #999;
        border-radius: 6px;
        margin-top: 20px;
        width: 522px;
    }

        .cdsm_common_display .cdsm_title {
            border-radius: 6px 6px 0 0;
            background-color: #EBEBEB;
            padding: 10px 10px;
            font-size: 15px;
            font-weight: bold;
        }

        .cdsm_common_display .cdsm_content {
            padding: 10px 10px;
            overflow: hidden;
        }

    .cdsm_suportcase_page .cdsm_common_display {
        margin-top: 20px;
        width: 520px;
    }

    .cdsm_common_display ul {
        margin: 10px 10px;
        border: 1px solid #000;
        height: 120px;
        overflow: auto;
    }

        .cdsm_common_display ul > li {
            float: none;
            clear: both;
            border-top: 1px solid #000;
            height: 30px;
            box-sizing: border-box;
        }

            .cdsm_common_display ul > li:first-child {
                border-top: none;
            }

            .cdsm_common_display ul > li > div {
                float: left;
                padding-left: 5px;
                padding-top: 5px;
                padding-bottom: 5px;
                box-sizing: border-box;
                display: table-cell;
            }

            .cdsm_common_display ul > li .id {
                width: 10%;
            }

            .cdsm_common_display ul > li .status {
                width: 15%;
            }

            .cdsm_common_display ul > li .title {
                width: 50%;
            }

            .cdsm_common_display ul > li .csr {
                width: 25%;
            }

            .cdsm_common_display ul > li:nth-child(even) {
                background: #fff;
            }

            .cdsm_common_display ul > li:nth-child(odd) {
                background: #f0f0f0;
            }

    .cdsm-list-link {
        font-size: 15px;
        font-weight: bold;
    }

.Open_status
{
    color:red;
}
.Resolve_status
{
    color:green;
}
</style>

<WebPartPages:SPProxyWebPartManager runat="server" id="spproxywebpartmanager"></WebPartPages:SPProxyWebPartManager>
<div class="cdsm_suportcase_page">
    <div class="cdsm_left">
        
        <WebPartPages:WebPartZone id="SupportCasesZoneLeft" runat="server" title="Zone Left"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
        
    </div>
    <div class="cdsm_right">
        <div class="cdsm_top">
            
        <WebPartPages:WebPartZone id="SupportCasesZoneTop" runat="server" title="Zone Top"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
            
        </div>
        <div class="cdsm_middle">
        
        <WebPartPages:WebPartZone id="SupportCasesZoneMiddle" runat="server" title="Zone Middle"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
        
        </div>
        <div class="cdsm_bottom">
        
        <WebPartPages:WebPartZone id="SupportCasesZoneBottom" runat="server" title="Zone Bottom"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
        
        </div>
    
    </div>
</div>
</asp:content>
