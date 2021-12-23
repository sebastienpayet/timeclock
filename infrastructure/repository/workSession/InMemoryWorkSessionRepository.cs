using System;
using System.Collections.Generic;
using System.Linq;
using TimeClock.business.model.workSession;
using TimeClock.business.port.repository;

namespace TimeClock.infrastructure.repository.workSession
{

    class InMemoryWorkSessionRepository : IWorkSessionRepository
    {
        private List<WorkSession> _workSessions = new List<WorkSession>();

        public List<WorkSession> FindAll()
        {
            return _workSessions;
        }

        public List<WorkSession> FindAllOfTheDay(DateTime date)
        {
            return _workSessions.Where(session => session.Date.Day == date.Day).ToList();
        }

        public List<WorkSession> FindAllOfTheWeek(DateTime refDate)
        {
            throw new NotImplementedException();
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
            _workSessions.Add(entity);
            return entity;
        }
    }
}
