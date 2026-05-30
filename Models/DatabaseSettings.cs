namespace ContactKeeper.Models
{
    public class DatabaseSettings
    {
        public string? Devops { get; set; }
        public string? Development { get; set; }
        public string? Production { get; set; }
        public string? ConnectionString { get; set; }
    }

    public class EnvironmentUrls
    {
        public string? Devops { get; set; }
        public string? Development { get; set; }
        public string? Production { get; set; }
        public string? IIS { get; set; }
    }

    public class EnvironmentBuild
    {
        public string? Development { get; set; }
        public string? Production { get; set; }
    }
}