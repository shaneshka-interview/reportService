using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ReportService.Common;

namespace Domain
{
    public class ReportBuilder : IReportBuilder
    {
        private StringBuilder sb = new StringBuilder();
        private const string delimeter = "--------------------------------------------";
        private const string separate = "         ";
        private const string currency = "р";
        private const string error = "ошибка";
        private const string newLine = "\r\n"; //Environment.NewLine

        public string Create(DateTime date, IEnumerable<Department> departments,
            Dictionary<string, IList<Employee>> departmentEmployees, HashSet<string> employeeErrors)
        {
            var totalSum = 0;
            AddLine(date.ToString("MMMM yyyy", CultureInfo.CreateSpecificCulture("ru-RU")))
                .AddDelimeter();
            if (departments != null)
                foreach (var department in departments)
                {
                    AddLine(department.Name);
                    var sum = 0;
                    if (departmentEmployees != null &&
                        departmentEmployees.TryGetValue(department.Id, out var employees))
                        foreach (var employee in employees)
                        {
                            AddEmployee(employee, employeeErrors?.Contains(employee.Id) == true);
                            sum += employee.Salary;
                        }

                    AddLine($"Всего по отделу {sum}");
                    AddDelimeter();

                    totalSum += sum;
                }

            AddLine($"Всего по предприятию {totalSum}");
            return ToString();
        }

        private ReportBuilder AddLine(string line)
        {
            sb.Append(line);
            sb.Append(newLine);
            return this;
        }

        private ReportBuilder AddEmployee(Employee employee, bool hasError)
        {
            sb.Append(employee.Name);
            sb.Append(separate);
            if (hasError)
                sb.Append(error);
            else
            {
                sb.Append(employee.Salary);
                sb.Append(currency);
            }

            sb.Append(newLine);

            return this;
        }

        private ReportBuilder AddDelimeter()
        {
            sb.Append(delimeter);
            sb.Append(newLine);
            return this;
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}