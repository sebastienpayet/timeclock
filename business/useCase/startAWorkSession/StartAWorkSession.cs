using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeClock.business.model.workSession;

namespace TimeClock.business.useCase.startAWorkSession
{
    class StartAWorkSession : IUseCase<WorkSession, StartAWorkSessionCommand>
    {
        public WorkSession Handle(StartAWorkSessionCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
