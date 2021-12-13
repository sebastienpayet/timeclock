using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeClock.business.useCase.startAWorkSession
{
    class StartAWorkSessionCommand : ICommand
    {
        public DateTime Date { get; private set; }

        public StartAWorkSessionCommand(DateTime date)
        {
            Date = date;
        }
    }
}
