using TimeClock.business.model.workSession;
using TimeClock.business.port.repository;

namespace TimeClock.business.useCase.startAWorkSession
{
    public class StartAWorkSession : IUseCase<WorkSession, StartAWorkSessionCommand>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IWorkSessionRepository _workSessionRepository;

        public StartAWorkSession(IWorkSessionRepository workSessionRepository)
        {
            _workSessionRepository = workSessionRepository;
        }

        public WorkSession Handle(StartAWorkSessionCommand command)
        {
            Logger.Info("StartAWorkSession use case triggered");
            WorkSession newSession = _workSessionRepository.Save(new WorkSession(WorkSessionType.START));
            Logger.Info("Session started");
            return newSession;
        }
    }
}
