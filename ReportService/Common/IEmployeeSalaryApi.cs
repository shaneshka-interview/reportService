using System.Threading.Tasks;

namespace ReportService.Common
{
    public interface IEmployeeSalaryApi
    {
        //todo нужен метод на колекцию
        Task<int> GetAsync(string code);
    }
}