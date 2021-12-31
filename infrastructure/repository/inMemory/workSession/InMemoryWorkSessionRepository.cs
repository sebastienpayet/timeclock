using System;
using System.Collections.Generic;
using System.Linq;
using TimeClock.business.model.workSession;
using TimeClock.business.port.repository;
using TimeClock.infrastructure.util;

namespace TimeClock.infrastructure.repository.inMemory.workSession
{
    public class InMemoryWorkSessionRepository : IWorkSessionRepository
    {
        private readonly List<WorkSession> _workSessions = new List<WorkSession>();

        public List<WorkSession> FindAll() => _workSessions;

        public List<WorkSession> FindAllOfTheDay(DateTime date)
        {
            return _workSessions.Where(session =>
            session.Date.Year == date.Year && session.Date.DayOfYear == date.DayOfYear
            ).ToList();
        }

        public List<WorkSession> FindAllOfTheWeek(DateTime refDate)
        {
            return _workSessions.Where(session => DateUtils.IsInTheSameWeek(session.Date, refDate)).ToList();
        }

        public WorkSession FindById(string id)
        {
            throw new NotImplementedException();
        }

        public List<WorkSession> FindDistinctOneByMonth(int numberOfMonthesInThePast)
        {
            throw new NotImplementedException();
        }

        public WorkSession FindLastOfTheDay(DateTime date)
        {
            throw new NotImplementedException();
        }

        public WorkSession Save(WorkSession entity)
        {
            _workSessions.Add(entity);
            return entity;
        }
    }
}
