using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Web.Configuration;
using NtTracker.Models;

namespace NtTracker.Data
{
    public class DataContext : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<NbrSurveillance> NbrSurveillances { get; set; }
        public DbSet<CnsExploration> CnsExplorations { get; set; }
        public DbSet<Hypothermia> Hypothermias { get; set; }
        public DbSet<Monitoring> Monitorings { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Analysis> Analyses { get; set; }

        public DataContext() : base("name=DataConnection")
        {
            Configuration.LazyLoadingEnabled = false;
#if DEBUG
            Database.Log = s => Debug.WriteLine(s);
#endif
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            // Get default schema from web.config
            var schema = WebConfigurationManager.AppSettings["DefaultSchema"];
            if (!string.IsNullOrEmpty(schema))
            {
                modelBuilder.HasDefaultSchema(schema.ToUpper());
            }
        }
    }
}