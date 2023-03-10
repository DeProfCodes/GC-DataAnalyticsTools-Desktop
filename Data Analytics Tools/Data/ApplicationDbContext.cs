using Data_Analytics_Tools.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data_Analytics_Tools.Data
{
    public class ApplicationDbContext : DbContext
    {
        private string connectionString = "Server=SYNERGY-7U24F9O\\GCWENSASERVER;Database=apacheLogsToMySqlMemory;User Id=gcwensaUser;Password=Gcwensa123;Trusted_Connection=True;MultipleActiveResultSets=true";
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<ProcessedApacheFile> ApacheFilesImportProgress { get; set; }

    }
}
