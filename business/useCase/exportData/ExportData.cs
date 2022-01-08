using TimeClock.business.port.exporter;

namespace TimeClock.business.useCase.exportData
{
    public class ExportData : IUseCase<string, ExportDataCommand>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IExporter _exporter;

        public ExportData(IExporter exporter)
        {
            _exporter = exporter;
        }

        public string Handle(ExportDataCommand command)
        {
            Logger.Info("ExportData use case triggered");
            _exporter.ExportFromAReferenceDate(command.Date);
            return "export started";
        }
    }
}
