using System;
using System.Collections.Generic;
using TimeClock.business.model.workSession;

namespace TimeClock.business.port.repository
{
    internal interface IWorkSessionRepository : IRepository<WorkSession>
    {
        WorkSession FindLastOfTheDay(DateTime date);
        List<WorkSession> FindAllOfTheDay(DateTime date);
        List<WorkSession> FindAllOfTheWeek(DateTime refDate);
    }
}
