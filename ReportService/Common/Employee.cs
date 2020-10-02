namespace ReportService.Common
{
    public class Employee
    {
        public string Id { get; set; } //todo guid?
        public string Name { get; set; }
        public string DepartmentId { get; set; } //todo guid?
        public string  Inn { get; set; }
        public int Salary { get; set; }
        //public string BuhCode { get; set; }
    }
}
