using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Governance.TimerJobs.Data
{
    /// <summary>
    /// GovernanceDbContext represents the O/RM context object of the governance solution database repository
    /// </summary>
    public class GovernanceDbContext : DbContext
    {
        /// <summary>
        /// Construct a new instance of GovernanceDbContext object
        /// </summary>
        public GovernanceDbContext() : base()
        {
        }

        /// <summary>
        /// Construct a new instance of GovernanceDbContext object with specific settings
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="disableProxy">Disable proxy entity generation for serialization</param>
        public GovernanceDbContext(String connectionString, bool disableProxy = false)
            : base(connectionString)
        {
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.ProxyCreationEnabled = !disableProxy;
        }

        /// <summary>
        /// Setup code first DB model relationship
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SiteInformation>().HasMany(site => site.Administrators).WithMany().Map(
              m =>
              {
                  m.MapLeftKey("SiteInformation_Id");
                  m.MapRightKey("User_Id");
                  m.ToTable("SiteInformationAdministrators");
              });

            modelBuilder.Entity<SiteInformation>().HasMany(site => site.ExternalUsers).WithMany().Map(
              m =>
              {
                  m.MapLeftKey("SiteInformation_Id");
                  m.MapRightKey("User_Id");
                  m.ToTable("SiteInformationExternalUsers");
              });

            modelBuilder.Entity<SiteInformation>().HasMany(site =>
                site.SiteMetadata).WithRequired(m => m.TargetSite).HasForeignKey(m => m.TargetSiteId).WillCascadeOnDelete(true);

            modelBuilder.Entity<SiteInformation>().HasMany(site => site.RemediationHistory).WithRequired(h => h.TargetSite).HasForeignKey(h => h.TargetSiteId).WillCascadeOnDelete(true);            
        }

        /// <summary>
        /// All web records
        /// </summary>
        public DbSet<WebInformation> Webs { get; set; }

        /// <summary>
        /// All site collection records
        /// </summary>
        public DbSet<SiteInformation> Sites { get; set; }

        /// <summary>
        /// All SharePoint user records
        /// </summary>
        public DbSet<SiteUser> Users { get; set; }

        /// <summary>
        /// All site collection metadata
        /// </summary>
        public DbSet<SiteMetadata> Metadata { get; set; }

        /// <summary>
        /// All remediation history records
        /// </summary>
        public DbSet<RemediationHistory> RemediationHistory { get; set; }
        
        /// <summary>
        /// Replace the duplicated user entities with those DB existed / attached user entities
        /// </summary>
        /// <param name="users"></param>
        public void ReplaceExistedUsers(ICollection<SiteUser> users)
        {
            if (users == null)
                return;
            var removeList = new LinkedList<SiteUser>();
            var addList = new LinkedList<SiteUser>();
            foreach (var user in users)
            {
                var existedUser = Users.FirstOrDefault(u => u.LoginName == user.LoginName);
                if (existedUser != null)
                {
                    removeList.AddLast(user);
                    addList.AddLast(existedUser);
                }
            }
            foreach(var user in removeList)
                users.Remove(user);
            foreach(var user in addList)
                users.Add(user);
        }

        /// <summary>
        /// Get site collection record by URL
        /// </summary>
        /// <param name="url">The site collection URL</param>
        /// <returns>Site information entity</returns>
        public SiteInformation GetSite(string url)
        {
            var tmp = new SiteInformation()
            {
                Url = url,
            };
            var existed = Sites.FirstOrDefault(s => 
                s.Name == tmp.Name &&
                s.UrlDomain == tmp.UrlDomain &&
                s.UrlPath == tmp.UrlPath);
            return existed;
        }

        /// <summary>
        /// Get web record by URL
        /// </summary>
        /// <param name="url">The web URL</param>
        /// <returns>Web information entity</returns>
        public WebInformation GetWeb(string url)
        {
            var tmp = new WebInformation()
            {
                Url = url,
            };
            var existed = Webs.FirstOrDefault(w =>
                w.Name == tmp.Name &&
                w.UrlDomain == tmp.UrlDomain &&
                w.UrlPath == tmp.UrlPath);
            return existed;
        }

        /// <summary>
        /// Insert a new site collection record in database if the URL is not existing, or otherwise update the existing site collection record with the property values of the argument
        /// </summary>
        /// <param name="site">The site information entity to be added or updated</param>
        public virtual void SaveSite(SiteInformation site)
        {
            var existed = GetSite(site.Url);
            if (existed != null)
                Sites.Remove(existed);
            ReplaceExistedUsers(site.Administrators);
            ReplaceExistedUsers(site.ExternalUsers);
            Sites.Add(site);
            SaveChanges();
        }

        /// <summary>
        /// Get all site information records in a paginated way
        /// </summary>
        /// <param name="page">The page number starts from 1</param>
        /// <param name="pageSize">The number of records in each page</param>
        /// <param name="maxPageIndex">Output the max page number</param>
        /// <param name="includes">An string array contains the relationship collection names to be included in the query</param>
        /// <param name="criteria">An params array contains the Linq to Entity expressions to be used in the where clause to union all the matching records</param>s
        /// <returns>Returns the enumerable of the current page records</returns>
        public IEnumerable<SiteInformation> GetAllSites(int page, int pageSize, out int maxPageIndex, string[] includes = null, params Expression<Func<SiteInformation, bool>>[] criteria)
        {
            if (page < 0)
                throw new ArgumentOutOfRangeException("page");
            else if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize");
            int totalCount = Sites.Count();
            maxPageIndex = totalCount / pageSize;
            if (totalCount % pageSize != 0)
                maxPageIndex++;
            Expression<Func<SiteInformation, int>> sort = s => s.Id;
            IQueryable<SiteInformation> sites = null;
            foreach (var c in criteria)
            {
                var matches = Sites.Where(c);
                sites = sites == null ? matches : sites.Union(matches);
            }
            foreach (var i in includes ?? new string[] {})
            {
                sites = (sites ?? Sites).Include(i);
            }            
            var result = (sites ?? Sites).OrderBy(sort).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }

        /// <summary>
        /// Get all web information records in a paginated way
        /// </summary>
        /// <param name="page">The page number starts from 1</param>
        /// <param name="pageSize">The number of records in each page</param>
        /// <param name="maxPageIndex">Output the max page number</param>
        /// <param name="criteria">An params array contains the Linq to Entity expressions to be used in the where clause to union all the matching records</param>s
        /// <returns>Returns the enumerable of the current page records</returns>
        public IEnumerable<WebInformation> GetAllWebs(int page, int pageSize, out int maxPageIndex, params Expression<Func<WebInformation, bool>>[] criteria)
        {
            if (page < 0)
                throw new ArgumentOutOfRangeException("page");
            else if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize");
            int totalCount = Sites.Count();
            maxPageIndex = totalCount / pageSize;
            if (totalCount % pageSize != 0)
                maxPageIndex++;
            Expression<Func<WebInformation, int>> sort = s => s.Id;
            IQueryable<WebInformation> webs = null;
            foreach (var c in criteria)
            {
                var matches = Webs.Where(c);
                webs = webs == null ? matches : webs.Union(matches);
            }
            var result = (webs ?? Webs).OrderBy(sort).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }
    }
}
