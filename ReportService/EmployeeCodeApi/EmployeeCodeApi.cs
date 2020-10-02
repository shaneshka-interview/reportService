using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReportService.Common;

namespace ReportService.EmployeeCodeApi
{
    public class EmployeeCodeApi : BaseApi.BaseApi<EmployeeCodeApi>, IEmployeeCodeApi
    {
        public EmployeeCodeApi(string url, ILoggerFactory loggerFactory) : base(url, loggerFactory)
        {
        }

        public async Task<string> GetAsync(string inn)
        {
            var response= await _httpClient.GetAsync(inn);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}