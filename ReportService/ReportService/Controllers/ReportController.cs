using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReportService.Common;
using ReportService.ReportService.Domain;

namespace ReportService.ReportService.Controllers
{
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly IEmployeeCodeApi _employeeCodeApi;
        private readonly IEmployeeSalaryApi _employeeSalaryApi;
        private readonly IStorage _storage;
        private readonly IReportBuilder _reportBuilder;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IEmployeeCodeApi employeeCodeApi, IEmployeeSalaryApi employeeSalaryApi,
            IStorage storage, ILogger<ReportController> logger, IReportBuilder reportBuilder)
        {
            _employeeCodeApi = employeeCodeApi;
            _employeeSalaryApi = employeeSalaryApi;
            _storage = storage;
            _logger = logger;
            _reportBuilder = reportBuilder;
        }

        [HttpGet]
        [Route("{year}/{month}")]
        public async Task<IActionResult> Download2(int year, int month)
        {
            try
            {
                var employeeErrors = new List<string>();
                var departmentEmployees = (await _storage.GetEmployeesAsync()).ToLookup(x => x.Department, async x =>
                {
                    try
                    {
                        x.BuhCode = await _employeeCodeApi.Get(x.Inn);
                        x.Salary = await _employeeSalaryApi.Get(x.BuhCode);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"employeeName=[{x.Name}]");
                        employeeErrors.Add(x.Name);
                    }

                    return x;
                });

                var report = await _reportBuilder.Create(new DateTime(year, month, 1), departmentEmployees, employeeErrors);

                var response = File(Encoding.ASCII.GetBytes(report), "application/octet-stream", "report.txt");
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, null);
                throw;
            }
        }
    }
}