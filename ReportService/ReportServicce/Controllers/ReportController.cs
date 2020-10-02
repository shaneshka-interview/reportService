using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using ReportService.Common;

namespace ReportServicce.Controllers
{
    //todo permition?
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
        public async Task<FileContentResult> DownloadAsync(int year, int month)
        {
            try
            {
                var employeeErrors = new HashSet<string>();
                var departments = _storage.GetDepartmentsAsync();

                var departmentEmployees = new Dictionary<string, IList<Employee>>();
                var fillTasks = new List<Task>();
                await foreach (var employee in _storage.GetEmployeesAsync())
                {
                    fillTasks.Add(FillSalaryAsync(employee, employeeErrors));

                    if (!departmentEmployees.ContainsKey(employee.DepartmentId))
                        departmentEmployees.Add(employee.DepartmentId, new List<Employee>());
                    departmentEmployees[employee.DepartmentId].Add(employee);
                }
                await Task.WhenAll(fillTasks);

                var date = new DateTime(year, month, 1);
                var report = _reportBuilder.Create(new DateTime(year, month, 1), await departments, departmentEmployees,
                    employeeErrors);

                return new FileContentResult(Encoding.UTF8.GetBytes(report), "application/octet-stream")
                {
                    FileDownloadName = $"report_{date:yyyy-MM}.txt",
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"error for {year}-{month}");
                throw;
            }
        }

        private async Task FillSalaryAsync(Employee employee, HashSet<string> employeeErrors)
        {
            try
            {
                var buhCode =  await _employeeCodeApi.GetAsync(employee.Inn);
                employee.Salary = await _employeeSalaryApi.GetAsync(buhCode);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"employeeName=[{employee.Id}]");
                employeeErrors.Add(employee.Id);
            }
        }
    }
}