using System;

namespace TimeClock.business.port.exporter
{
    public interface IExporter
    {
        void ExportFromAReferenceDate(DateTime date);
    }
}
