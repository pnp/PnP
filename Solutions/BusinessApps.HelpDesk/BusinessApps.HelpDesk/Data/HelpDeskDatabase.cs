namespace BusinessApps.HelpDesk.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class HelpDeskDatabase : DbContext
    {
        public HelpDeskDatabase()
            : base("name=HelpDeskDatabase")
        {
        }

        public virtual DbSet<SupportTicket> SupportTickets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
