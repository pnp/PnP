using System;
using Microsoft.SharePoint.Client;


namespace OfficeDevPnP.PowerShell.Commands.Entities
{
    public class ViewEntity : EntityContextObject<View>
    {
        public string Title { get; set; }
        public Guid Id { get; set; }
        public bool DefaultView { get; set; }
        public bool PersonalView { get; set; }
        public string ViewType { get; set; }
        public uint RowLimit { get; set; }
        public string Query { get; set; }
        public ViewFieldCollection ViewFields { get; set; }

    }

}
