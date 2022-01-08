using TimeClock.business.model.workSession;
using TimeClock.business.port.repository;
using TimeClock.business.useCase.startAWorkSession;

namespace TimeClock.business.useCase.stopAWorkSession
{
    public class StopAWorkSession : IUseCase<WorkSession, StopAWorkSessionCommand>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IWorkSessionRepository _workSessionRepository;

        public StopAWorkSession(IWorkSessionRepository workSessionRepository)
        {
            _workSessionRepository = workSessionRepository;
        }

        public WorkSession Handle(StopAWorkSessionCommand command)
        {
            Logger.Info("StopAWorkSession use case triggered");
            WorkSession newSession = _workSessionRepository.Save(new WorkSession(WorkSessionType.STOP));
            Logger.Info("Session stopped");
            return newSession;
        }
    }
}
