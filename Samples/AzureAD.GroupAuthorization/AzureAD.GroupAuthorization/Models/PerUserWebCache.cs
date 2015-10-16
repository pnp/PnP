using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Office365AddIn.GroupAuthorization.Models
{
   public class PerUserWebCache
    {
        [Key]
        public int EntryId { get; set; }
        public string WebUserUniqueId { get; set; }
        public byte[] CacheBits { get; set; }
        public DateTime LastWrite { get; set; }
    }
}
