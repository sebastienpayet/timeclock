using TimeClock.business.port.exporter;

namespace TimeClock.business.useCase.exportData
{
    public class ExportData : IUseCase<string, ExportDataCommand>
    {
        private readonly IExporter _exporter;

        public ExportData(IExporter exporter)
        {
            _exporter = exporter;
        }

        public string Handle(ExportDataCommand command)
        {
            _exporter.ExportFromAReferenceDate(command.Date);
            return "export started";
        }
    }
}
