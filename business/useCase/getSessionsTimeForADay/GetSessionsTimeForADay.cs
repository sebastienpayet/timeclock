using System;
using System.Collections.Generic;
using System.Linq;
using TimeClock.business.model.workSession;
using TimeClock.business.port.repository;

namespace TimeClock.business.useCase.getSessionsTimeForADay
{
    public class GetSessionsTimeForADay : IUseCase<TimeSpan, GetSessionsTimeForADayCommand>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IWorkSessionRepository _workSessionRepository;

        public GetSessionsTimeForADay(IWorkSessionRepository workSessionRepository)
        {
            _workSessionRepository = workSessionRepository;
        }

        public TimeSpan Handle(GetSessionsTimeForADayCommand command)
        {
            Logger.Info($"GetSessionsTimeForADay use case triggered with refDate {command.Date}");
            List<WorkSession> workSessions = _workSessionRepository.FindAllOfTheDay(command.Date).OrderBy(session => session.Date).ToList();

            ManageStartAndStopOverTwoDays(command.Date, workSessions);

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
                    string message = $"illegal session type in sessions list => start => index {i} - {startSession.Type}, stop => index {i + 1} - {stopSession.Type}";
                    Logger.Fatal(message);
                    throw new ApplicationException(message);
                }

                totalTimeSpan = totalTimeSpan.Add(stopSession.Date - startSession.Date);
            }
            return totalTimeSpan;
        }

        private void ManageStartAndStopOverTwoDays(DateTime date, List<WorkSession> workSessions)
        {
            Logger.Debug("check data consistency");
            // if the first of the day is a stop, just delete it
            if (workSessions.Count > 0)
            {
                if (workSessions.First().Type == WorkSessionType.STOP)
                {
                    Logger.Warn("first session of the day is a stop, cleaning it");
                    _ = workSessions.Remove(workSessions[0]);
                }

                // if the last session of the day is a start, load the next session of the next day
                if (workSessions.Last().Type == WorkSessionType.START)
                {
                    Logger.Warn("last session of the day is a start");
                    List<WorkSession> nextDayworkSessions = _workSessionRepository.FindAllOfTheDay(date.AddDays(1)).OrderBy(session => session.Date).ToList();
                    Logger.Warn("loading first session of the next day");
                    // first session of the next day MUST be a STOP
                    if (nextDayworkSessions.Count > 0 && nextDayworkSessions.First().Type == WorkSessionType.STOP)
                    {
                        workSessions.Add(nextDayworkSessions.First());
                    }
                    // log error
                    Logger.Error($"Worksession database inconsistency between {date} and the day after");
                };
            }
        }
    }
}
