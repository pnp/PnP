using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Workflow.CallServiceUpdateSPViaProxyWeb.Models
{
    public class UpdatePartSupplierModel
    {
        public int Id { get; set; }
        public ISet<Supplier> Suppliers { get; set; }
    }
}