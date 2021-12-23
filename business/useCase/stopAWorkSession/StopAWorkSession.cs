using TimeClock.business.model.workSession;
using TimeClock.business.port.repository;
using TimeClock.business.useCase.startAWorkSession;

namespace TimeClock.business.useCase.stopAWorkSession
{
    public class StopAWorkSession : IUseCase<WorkSession, StopAWorkSessionCommand>
    {
        private IWorkSessionRepository _workSessionRepository;

        public StopAWorkSession(IWorkSessionRepository workSessionRepository)
        {
            _workSessionRepository = workSessionRepository;
        }

        public WorkSession Handle(StopAWorkSessionCommand command)
        {
            WorkSession newSession = new WorkSession(WorkSessionType.STOP);
            return _workSessionRepository.Save(newSession);
        }
    }
}
