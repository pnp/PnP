using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPOGraphConsumer.Models
{
    public class ListInfoViewModel
    {
        public String Description { get; set; }

        public String DisplayName { get; set; }

        public String ETag { get; set; }

        public Guid Id { get; set; }

        public String Name { get; set; }

        public String WebUrl { get; set; }
    }
}