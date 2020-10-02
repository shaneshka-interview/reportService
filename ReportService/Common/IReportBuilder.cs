using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportService.Common
{
    public interface IReportBuilder
    {
        string Create(DateTime date, IEnumerable<Department> departments,
            Dictionary<string, IList<Employee>> departmentEmployees, HashSet<string> employeeErrors);
    }
}