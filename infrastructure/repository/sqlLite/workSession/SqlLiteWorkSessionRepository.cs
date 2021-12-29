using System;
using System.Collections.Generic;
using System.Linq;
using TimeClock.business.model.workSession;
using TimeClock.business.port.repository;
using TimeClock.infrastructure.util;

namespace TimeClock.infrastructure.repository.sqlLite.workSession
{

    public class SqlLiteWorkSessionRepository : IWorkSessionRepository
    {
        private readonly TimeClockContext timeClockContext;

        public SqlLiteWorkSessionRepository(TimeClockContext timeClockContext)
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

        public WorkSession FindLastOfTheDay(DateTime date)
        {
            throw new NotImplementedException();
        }

        public WorkSession Save(WorkSession entity)
        {
            WorkSession savedSession = timeClockContext.WorkSessions.Update(entity).Entity;
            timeClockContext.WorkSessions.Add(savedSession);
            timeClockContext.SaveChanges();
            return savedSession;
        }
    }
}
