using System.Threading.Tasks;

namespace ReportService.Common
{
    public interface IEmployeeCodeApi
    {
        //todo нужен метод на колекцию
        Task<string> GetAsync(string inn);
    }
}