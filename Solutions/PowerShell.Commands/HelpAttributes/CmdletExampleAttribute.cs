using System;

namespace OfficeDevPnP.PowerShell.CmdletHelpAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
                      AllowMultiple = true)]
    public sealed class CmdletExampleAttribute : Attribute
    {
       
        public string Code { get; set; }
        public string Introduction { get; set; }
        public string Remarks { get; set; }
        public int SortOrder { get; set; }
        
        public CmdletExampleAttribute()
        {
           
        }
    }
}
