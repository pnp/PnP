using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SPO = OfficeDevPnP.SPOnline.Core;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommunications.Send, "SPOMail")]
    public class SendMail : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public string Server = "smtp.office365.com";

        [Parameter(Mandatory = true)]
        public string From;

        [Parameter(Mandatory = true)]
        public string Password;

        [Parameter(Mandatory = true)]
        public string[] To;

        [Parameter(Mandatory = false)]
        public string[] Cc;

        [Parameter(Mandatory = true)]
        public string Subject;

        [Parameter(Mandatory = true)]
        public string Body;


        protected override void ExecuteCmdlet()
        {
            MailUtility.SendEmail(Server, From, Password, To, Cc, Subject, Body);
        }
    }

}
