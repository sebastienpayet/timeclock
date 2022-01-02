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
            return timeClockContext.WorkSessions.Where(session =>
            session.Date.Year == date.Year && session.Date.DayOfYear == date.DayOfYear
            ).ToList();
        }

        public List<WorkSession> FindAllOfTheWeek(DateTime refDate)
        {
            return timeClockContext.WorkSessions.Where(session =>
            DateUtils.IsInTheSameWeek(session.Date, refDate)
            ).ToList();
        }

        public WorkSession FindById(string id)
        {
            throw new NotImplementedException();
        }

        public List<WorkSession> FindDistinctOneByMonth(int numberOfMonthesInThePast)
        {
            DateTime minDate = DateTime.Now.AddMonths(-numberOfMonthesInThePast);
            minDate = new DateTime(minDate.Year, minDate.Month, 1, 0, 0, 0);

            return timeClockContext.WorkSessions.Where(session => session.Date >= minDate)
                .ToList()
                .GroupBy(session => session.Date.Year + "" + session.Date.Month)
                .Select(group => group.First())
                .OrderByDescending(session => session.Date).ToList();
        }

        public WorkSession FindLastOfTheDay(DateTime date)
        {
            throw new NotImplementedException();
        }

        public WorkSession Save(WorkSession entity)
        {
            _ = timeClockContext.WorkSessions.Add(entity);
            _ = timeClockContext.SaveChanges();
            return entity;
        }
    }
}
