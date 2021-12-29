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
                    // a work session is in progress
                    // create a virtual stop session at now
                    stopSession = new WorkSession(WorkSessionType.STOP);
                    _workSessionRepository.Save(stopSession);
                }

                if (startSession.Type != WorkSessionType.START || stopSession.Type != WorkSessionType.STOP)
                {
                    throw new ApplicationException($"illegal session type in sessions list => start => index {i} - {startSession.Type}, stop => index {i+1} - {stopSession.Type}");
                }

                totalTimeSpan = totalTimeSpan.Add(stopSession.Date - startSession.Date);
            }
            return totalTimeSpan;
        }
    }
}
