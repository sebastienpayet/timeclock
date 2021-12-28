using Microsoft.EntityFrameworkCore;
using System;
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
            DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}timeclock.db";
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
