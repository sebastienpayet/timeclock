using System;
using System.Collections.Generic;
using TimeClock.business.model.workSession;
using TimeClock.business.port.repository;

namespace TimeClock.infrastructure.repository.sqlLite.workSession
{

    class SqlLiteWorkSessionRepository : IWorkSessionRepository
    {
        public List<WorkSession> FindAll()
        {
            throw new NotImplementedException();
        }

        public List<WorkSession> FindAllOfTheDay(DateTime date)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
