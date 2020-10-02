using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using ReportServicce.Controllers;
using ReportService.Common;

namespace ReportService.Tests
{
    public class ReportControllerTest
    {
        private Mock<IEmployeeCodeApi> _mockCodeApi;
        private Mock<IEmployeeSalaryApi> _mockSalaryApi;
        private Mock<IStorage> _mockStorage;
        private ReportController _controller;

        private const string Report = @"Январь 2020
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

        private const string ReportError = @"Январь 2020
--------------------------------------------
ФинОтдел
Андрей Сергеевич Бубнов         150000р
Григорий Евсеевич Зиновьев         ошибка
Всего по отделу 150000
--------------------------------------------
Бухгалтерия
Дмитрий Степанович Полянски         200000р
Всего по отделу 200000
--------------------------------------------
Всего по предприятию 350000
";

        [SetUp]
        public void Setup()
        {
            _mockCodeApi = new Mock<IEmployeeCodeApi>();
            _mockSalaryApi = new Mock<IEmployeeSalaryApi>();
            _mockStorage = new Mock<IStorage>();

            _mockCodeApi.Setup(x => x.GetAsync("1")).ReturnsAsync("1");
            _mockCodeApi.Setup(x => x.GetAsync("2")).ReturnsAsync("2");
            _mockCodeApi.Setup(x => x.GetAsync("3")).ReturnsAsync("3");

            _mockSalaryApi.Setup(x => x.GetAsync("1")).ReturnsAsync(150000);
            _mockSalaryApi.Setup(x => x.GetAsync("2")).ReturnsAsync(100000);
            _mockSalaryApi.Setup(x => x.GetAsync("3")).ReturnsAsync(200000);

            _mockStorage.Setup(x => x.GetEmployeesAsync()).Returns(new[]
            {
                new Employee
                {
                    Id = "1",
                    Inn="1",
                    Name = "Андрей Сергеевич Бубнов",
                    DepartmentId = "1"
                },
                new Employee
                {
                    Id = "2",
                    Inn="2",
                    Name = "Григорий Евсеевич Зиновьев",
                    DepartmentId = "1"
                },
                new Employee
                {
                    Id = "3",
                    Inn="3",
                    Name = "Дмитрий Степанович Полянски",
                    DepartmentId = "2"
                }
            }.ToAsyncEnumerable());
            _mockStorage.Setup(x => x.GetDepartmentsAsync()).ReturnsAsync(new[]
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
            });

            _controller = new ReportController(_mockCodeApi.Object, _mockSalaryApi.Object, _mockStorage.Object,
                new NullLogger<ReportController>(), new ReportBuilder());
        }

        [Test]
        public async Task ShouldReport()
        {
            var response = await _controller.DownloadAsync(2020, 1);
            Assert.That(System.Text.Encoding.Default.GetString(response.FileContents), Is.EqualTo(Report));
            Assert.That(response.FileDownloadName, Is.EqualTo("report_2020-01.txt"));
        }

        [Test]
        public async Task ShouldReport_IfThrowCodeApi()
        {
            _mockCodeApi.Setup(x => x.GetAsync("2")).ReturnsAsync(() => throw new Exception());

            var response = await _controller.DownloadAsync(2020, 1);

            Assert.That(System.Text.Encoding.Default.GetString(response.FileContents), Is.EqualTo(ReportError));
        }

        [Test]
        public async Task ShouldReport_IfThrowSalaryApi()
        {
            _mockSalaryApi.Setup(x => x.GetAsync("2")).ReturnsAsync(() => throw new Exception());

            var response = await _controller.DownloadAsync(2020, 1);

            Assert.That(System.Text.Encoding.Default.GetString(response.FileContents), Is.EqualTo(ReportError));
        }

        [Test]
        public void Throw_IfThrowGetDepartments()
        {
            _mockStorage.Setup(x => x.GetDepartmentsAsync()).ReturnsAsync(() => throw new Exception());

            Assert.ThrowsAsync<Exception>(async () => await _controller.DownloadAsync(2020, 1));
        }

        [Test]
        public void Throw_IfThrowGetEmployees()
        {
            async IAsyncEnumerable<Employee> StorageException()
            {
                await Task.FromException(new Exception());
                yield break;
            }

            _mockStorage.Setup(x => x.GetEmployeesAsync()).Returns(StorageException());

            Assert.ThrowsAsync<Exception>(async () => await _controller.DownloadAsync(2020, 1));
        }

        //...and more
    }
}