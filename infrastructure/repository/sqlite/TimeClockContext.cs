using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using TimeClock.business.model.workSession;

namespace TimeClock.infrastructure.repository.sqlLite
{

    public class TimeClockContext : DbContext
    {
        public DbSet<WorkSession> WorkSessions { get; set; }

        public string DbPath { get; private set; }

        public TimeClockContext()
        {
            Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
            string path = Environment.GetFolderPath(folder);
            DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}\\timeclock\\timeclock.db";
            Database.EnsureCreated();
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            IEnumerable<IMutableProperty> keysProperties = builder.Model.GetEntityTypes().Select(x => x.FindPrimaryKey()).SelectMany(x => x.Properties);
            foreach (IMutableProperty property in keysProperties)
            {
                property.ValueGenerated = ValueGenerated.OnAdd;
            }

            _ = builder.Entity<WorkSession>()
                .HasIndex(b => b.Date).IsUnique();
        }
    }
}
