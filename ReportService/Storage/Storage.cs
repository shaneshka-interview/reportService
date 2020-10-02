using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using ReportService.Common;

namespace ReportService.Storage
{
    public class Storage : IStorage
    {
        private readonly string _connection;

        public Storage(string connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        {
            await using var conn = await GetConnectAsync();
            var cmd = new NpgsqlCommand("SELECT d.id, d.name from deps d where d.active = true", conn);
            var reader = await cmd.ExecuteReaderAsync();
            var res = new List<Department>();
            while (await reader.ReadAsync())
            {
                res.Add(new Department
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                });
            }

            return res;
        }

        public async IAsyncEnumerable<Employee> GetEmployeesAsync(string departmentId)
        {
            await using var conn = await GetConnectAsync();
            var cmd = new NpgsqlCommand(
                $"SELECT e.name, e.inn from emps e where e.departmentid={departmentId}",
                conn);
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                yield return new Employee
                    {Name = reader.GetString(0), Inn = reader.GetString(1), DepartmentId = departmentId};
            }
        }

        public async IAsyncEnumerable<Employee> GetEmployeesAsync()
        {
            await using var conn = await GetConnectAsync();
            var cmd = new NpgsqlCommand(
                $"SELECT e.name, e.inn, e.departmentid from emps e left join deps d on e.departmentid = d.id where d.active = true order by d.name, e.name",
                conn);
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                yield return new Employee
                    {Name = reader.GetString(0), Inn = reader.GetString(1), DepartmentId = reader.GetString(2)};
            }
        }

        private async Task<NpgsqlConnection> GetConnectAsync()
        {
            var conn = new NpgsqlConnection(_connection);
            await conn.OpenAsync();

            return conn;
        }
    }
}