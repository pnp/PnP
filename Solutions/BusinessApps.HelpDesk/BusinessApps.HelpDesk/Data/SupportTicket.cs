namespace BusinessApps.HelpDesk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SupportTicket")]
    public partial class SupportTicket
    {
        public int ID { get; set; }

        [Required]
        [StringLength(300)]
        public string MessageID { get; set; }

        [Required]
        [StringLength(100)]
        public string AssignedTo { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
