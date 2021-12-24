using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeClock.business.useCase.exportData
{
    public class ExportDataCommand : ICommand
    {
        public DateTime Date;

        public ExportDataCommand(DateTime date)
        {
            Date = date;
        }
    }
}
