namespace BusinessApps.RemoteCalendarAccessWeb.DataLayer.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DataModel : DbContext
    {
        public DataModel()
            : base("name=DataModel")
        {
        }

        public virtual DbSet<RemoteCalendarAccess> RemoteCalendarAccess { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RemoteCalendarAccess>()
                .Property(e => e.UserId)
                .IsUnicode(true);

            modelBuilder.Entity<RemoteCalendarAccess>()
                .Property(e => e.SiteAddress)
                .IsUnicode(true);
        }
    }
}
