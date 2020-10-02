using System;
using System.Collections.Generic;
using Domain;
using NUnit.Framework;
using ReportService.Common;

namespace ReportService.Tests
{
    public class ReporBuilderTest
    {
        private IReportBuilder _reportBuilder;

        [SetUp]
        public void Setup()
        {
            _reportBuilder = new ReportBuilder();
        }

        [TestCaseSource(nameof(TestCase))]
        public void Simple((DateTime date, IEnumerable<Department> departments,
            Dictionary<string, IList<Employee>> departmentEmployees, HashSet<string> employeeErrors, string res) item)
        {
            var report = _reportBuilder.Create(item.date, item.departments, item.departmentEmployees,
                item.employeeErrors);
            Assert.That(report, Is.EqualTo(item.res));
        }

        private static IEnumerable<(DateTime date, IEnumerable<Department> departments,
                Dictionary<string, IList<Employee>> departmentEmployees, HashSet<string> employeeErrors, string res)>
            TestCase()
        {
            const string report = @"Январь 2020
--------------------------------------------
ФинОтдел
Андрей Сергеевич Бубнов         150000р
Григорий Евсеевич Зиновьев         100000р
Всего по отделу 250000
--------------------------------------------
Бухгалтерия
Дмитрий Степанович Полянски         200000р
Всего по отделу 200000
--------------------------------------------
Всего по предприятию 450000
";
            var depatments = new[]
            {
                new Department
                {
                    Id = "1",
                    Name = "ФинОтдел"
                },
                new Department
                {
                    Id = "2",
                    Name = "Бухгалтерия"
                }
            };
            var departmentEmployees = new Dictionary<string, IList<Employee>>
            {
                {
                    "1", new List<Employee>
                    {
                        new Employee
                        {
                            Id = "1",
                            Name = "Андрей Сергеевич Бубнов",
                            Salary = 150000,
                            DepartmentId = "1"
                        },
                        new Employee
                        {
                            Id = "2",
                            Salary = 100000,
                            Name = "Григорий Евсеевич Зиновьев",
                            DepartmentId = "1"
                        }
                    }
                },
                {
                    "2", new List<Employee>
                    {
                        new Employee
                        {
                            Id = "3",
                            Salary = 200000,
                            Name = "Дмитрий Степанович Полянски",
                            DepartmentId = "2"
                        }
                    }
                }
            };
            var date = new DateTime(2020, 01, 01);
            yield return (date, depatments, departmentEmployees, null, report);
            yield return (date, null, departmentEmployees, null, @"Январь 2020
--------------------------------------------
Всего по предприятию 0
");
            yield return (date, null, null, null, @"Январь 2020
--------------------------------------------
Всего по предприятию 0
");
            yield return (date, depatments, null, null, @"Январь 2020
--------------------------------------------
ФинОтдел
Всего по отделу 0
--------------------------------------------
Бухгалтерия
Всего по отделу 0
--------------------------------------------
Всего по предприятию 0
");
            //...
        }
    }
}