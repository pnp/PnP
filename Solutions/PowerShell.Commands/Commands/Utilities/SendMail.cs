using System.Management.Automation;
using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommunications.Send, "SPOMail")]
    [CmdletHelp("Sends an email using the Office 365 SMTP Service", Category = "Utilities")]
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
