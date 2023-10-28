namespace BHSystem.Web.Core
{
    public class LoggerCore //ghi log
    {
        #region "Properties"

        IHttpClientFactory factory;
        HttpClient request;

        ILogger<ApiService> logger;
        private readonly string API_KEY_NAME = "ApiKey";
        private readonly string API_KEY_VALUE = "123"; // lấy từ settings

        #endregion "Properties"


        public LoggerCore(IHttpClientFactory factory, ILogger<ApiService> logger)
        {
            this.factory = factory;
            this.request = factory.CreateClient("api");
            this.request.DefaultRequestHeaders.Add(API_KEY_NAME, API_KEY_VALUE);
            this.logger = logger;
        }
        public void LogInformation(Exception exception, string message, params object[] args)
        {
            this.logger.LogInformation(exception, message, args);
        }

        public void LogWarning(Exception exception, string message, params object[] args)
        {
            this.logger.LogWarning(exception, message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            this.logger.LogError(exception, message, args);
        }
    }
}