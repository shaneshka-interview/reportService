using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportService.Common
{
    public interface IStorage
    {
        Task<IEnumerable<Department>> GetDepartmentsAsync();
        IAsyncEnumerable<Employee> GetEmployeesAsync(string departmentId);
        IAsyncEnumerable<Employee> GetEmployeesAsync();
    }
}