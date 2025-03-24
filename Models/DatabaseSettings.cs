namespace ContactKeeper.Models
{
    public class DatabaseSettings
    {   
        public string? ConnectionString { get; set; }
        public string? DevelopmentConnection { get; set; }
        public string? ProductionConnection { get; set; }
 
    }   
}