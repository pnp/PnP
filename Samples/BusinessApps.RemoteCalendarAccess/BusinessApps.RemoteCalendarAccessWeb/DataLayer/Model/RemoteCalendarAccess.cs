namespace BusinessApps.RemoteCalendarAccessWeb.DataLayer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RemoteCalendarAccess")]
    public partial class RemoteCalendarAccess
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public Guid CalendarId { get; set; }

        [Required]
        [StringLength(500)]
        public string SiteAddress { get; set; }

        [Required]
        [StringLength(100)]
        public string UserId { get; set; }

        public DateTime LastAccess { get; set; }
    }
}
