namespace BHSystem.Web.Features.Client
{
    public partial class IndexClient
    {
        public string binding { get; set; } = "";
        public List<string> ListData = new List<string>() { "Quận 1", "Quận 2", "Quận 3", "Quận 4", "Quận 5" };
        public int Page { get; set; } = 3;
        public int PageSize { get; set; } = 4;
        public int TotalCount { get; set; } = 50;
    }
}
