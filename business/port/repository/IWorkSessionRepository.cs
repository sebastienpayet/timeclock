using System;
using System.Collections.Generic;
using TimeClock.business.model.workSession;

namespace TimeClock.business.port.repository
{
    public interface IWorkSessionRepository : IRepository<WorkSession>
    {
        List<WorkSession> FindAllOfTheDay(DateTime date);
        List<WorkSession> FindAllOfTheWeek(DateTime refDate);
        List<WorkSession> FindDistinctOneByMonth(int numberOfMonthesInThePast);
    }
}
