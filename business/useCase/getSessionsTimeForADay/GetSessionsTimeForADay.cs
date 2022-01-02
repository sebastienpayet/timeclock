using System;
using System.Collections.Generic;
using System.Linq;
using TimeClock.business.model.workSession;
using TimeClock.business.port.repository;

namespace TimeClock.business.useCase.getSessionsTimeForADay
{
    public class GetSessionsTimeForADay : IUseCase<TimeSpan, GetSessionsTimeForADayCommand>
    {
        private readonly IWorkSessionRepository _workSessionRepository;

        public GetSessionsTimeForADay(IWorkSessionRepository workSessionRepository)
        {
            _workSessionRepository = workSessionRepository;
        }

        public TimeSpan Handle(GetSessionsTimeForADayCommand command)
        {
            List<WorkSession> workSessions = _workSessionRepository.FindAllOfTheDay(command.Date).OrderBy(session => session.Date).ToList();

            ChekWorkSessionsConsistency(command.Date, workSessions);

            TimeSpan totalTimeSpan = new TimeSpan();

            for (int i = 0; i < workSessions.Count; i += 2)
            {
                WorkSession startSession;
                WorkSession stopSession;

                // start session
                startSession = workSessions[i];

                try
                {
                    // stop session
                    stopSession = workSessions[i + 1];
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // a work session has been started but not stopped
                    // create a virtual stop session at start date + 1 second, session time is lost
                    stopSession = new WorkSession(WorkSessionType.STOP, workSessions[i].Date.AddSeconds(1));
                    _workSessionRepository.Save(stopSession);
                }

                if (startSession.Type != WorkSessionType.START || stopSession.Type != WorkSessionType.STOP)
                {
                    throw new ApplicationException($"illegal session type in sessions list => start => index {i} - {startSession.Type}, stop => index {i + 1} - {stopSession.Type}");
                }

                totalTimeSpan = totalTimeSpan.Add(stopSession.Date - startSession.Date);
            }
            return totalTimeSpan;
        }

        private void ChekWorkSessionsConsistency(DateTime date, List<WorkSession> workSessions)
        {
            // if the first of the day is a stop, just delete it
            if (workSessions.Count > 0)
            {
                if (workSessions.First().Type == WorkSessionType.STOP)
                {
                    _ = workSessions.Remove(workSessions[0]);
                }

                // if the last session of the day is a start, load the next session of the next day
                if (workSessions.Last().Type == WorkSessionType.START)
                {
                    List<WorkSession> nextDayworkSessions = _workSessionRepository.FindAllOfTheDay(date.AddDays(1)).OrderBy(session => session.Date).ToList();

                    // first session of the next day MUST be a STOP
                    if (nextDayworkSessions.Count > 0 && nextDayworkSessions.First().Type == WorkSessionType.STOP)
                    {
                        workSessions.Add(nextDayworkSessions.First());
                    }
                    // Todo else log error here
                };
            }
        }
    }
}
