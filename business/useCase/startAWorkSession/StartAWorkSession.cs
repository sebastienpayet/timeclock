using TimeClock.business.model.workSession;
using TimeClock.business.port.repository;

namespace TimeClock.business.useCase.startAWorkSession
{
    public class StartAWorkSession : IUseCase<WorkSession, StartAWorkSessionCommand>
    {
        private readonly IWorkSessionRepository _workSessionRepository;

        public StartAWorkSession(IWorkSessionRepository workSessionRepository)
        {
            _workSessionRepository = workSessionRepository;
        }

        public WorkSession Handle(StartAWorkSessionCommand command)
        {
            WorkSession newSession = new WorkSession(WorkSessionType.START);
            return _workSessionRepository.Save(newSession);
        }
    }
}
