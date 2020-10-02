using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace ReportService.BaseApi
{
    public abstract class BaseApi<T>
    {
        protected static readonly HttpClient _httpClient;
        protected readonly ILogger<T> _logger;

        static BaseApi()
        {
            _httpClient = new HttpClient();
        }

        protected BaseApi(string url, ILoggerFactory loggerFactory)
        {
            _httpClient.BaseAddress = new Uri(url);
            loggerFactory.CreateLogger(nameof(T));
        }
    }
}