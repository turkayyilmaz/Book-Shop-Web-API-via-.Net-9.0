using NLog;
using Services.Abstracts;

namespace Services.Concretes
{
    public class LoggerService : ILoggerService
    {
        // Logger Nlog kütüphanesinden geliyor dikkat et
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public void LogDebug(string message) => logger.Debug(message);
        public void LogError(string message) => logger.Error(message);
        public void LogInfo(string message) => logger.Info(message);
        public void LogWarning(string message) => logger.Warn(message);
    }
}
