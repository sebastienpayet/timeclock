using System;
using System.Collections.Generic;
using System.Linq;
using TimeClock.business.model.workSession;
using TimeClock.business.port.repository;
using TimeClock.infrastructure.util;

namespace TimeClock.infrastructure.repository.sqlLite.workSession
{

    public class SqliteWorkSessionRepository : IWorkSessionRepository
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly TimeClockContext timeClockContext;

        public SqliteWorkSessionRepository(TimeClockContext timeClockContext)
        {
            this.timeClockContext = timeClockContext;
        }

        public List<WorkSession> FindAll()
        {
            return timeClockContext.WorkSessions.ToList();
        }

        public List<WorkSession> FindAllOfTheDay(DateTime date)
        {
            Logger.Debug("SQLite Worksession FindAllOfTheDay");
            return timeClockContext.WorkSessions.Where(session =>
            session.Date.Year == date.Year && session.Date.DayOfYear == date.DayOfYear
            ).ToList();
        }

        public List<WorkSession> FindAllOfTheWeek(DateTime refDate)
        {
            Logger.Debug("SQLite Worksession FindAllOfTheWeek");
            return timeClockContext.WorkSessions.Where(session =>
            DateUtils.IsInTheSameWeek(session.Date, refDate)
            ).ToList();
        }

        public WorkSession FindById(string id)
        {
            return timeClockContext.WorkSessions.Where(session => session.Id == id).Single();
        }

        public List<WorkSession> FindDistinctOneByMonth(int numberOfMonthesInThePast)
        {
            Logger.Debug("SQLite Worksession FindDistinctOneByMonth");

            DateTime minDate = DateTime.Now.AddMonths(-numberOfMonthesInThePast);
            minDate = new DateTime(minDate.Year, minDate.Month, 1, 0, 0, 0);

            return timeClockContext.WorkSessions.Where(session => session.Date >= minDate)
                .ToList()
                .GroupBy(session => session.Date.Year + "" + session.Date.Month)
                .Select(group => group.First())
                .OrderByDescending(session => session.Date).ToList();
        }

        public WorkSession Save(WorkSession entity)
        {
            Logger.Debug("SQLite Worksession saving");

            _ = timeClockContext.WorkSessions.Add(entity);
            _ = timeClockContext.SaveChanges();
            return entity;
        }
    }
}
