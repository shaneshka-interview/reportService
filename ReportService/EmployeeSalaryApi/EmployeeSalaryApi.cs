using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReportService.Common;

namespace ReportService.EmployeeSalaryApi
{
    public class EmployeeSalaryApi : BaseApi.BaseApi<EmployeeSalaryApi>, IEmployeeSalaryApi
    {
        public EmployeeSalaryApi(string url, ILoggerFactory loggerFactory) : base(url, loggerFactory)
        {
        }

        public async Task<int> GetAsync(string code)
        {
            var response = await _httpClient.PostAsync(code, new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return int.Parse(content);
        }
    }
}