using ContactKeeper.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactKeeper.Data
{
    /// <summary>
    /// <param name="DataContext">Config options for databse conn, on this case, successfully connection string </param>
    /// <param name="Users">Object List of user entity model</param>
    /// </summary>
    public class DataContext : DbContext
    {
        public DataContext (DbContextOptions<DataContext> options) : base(options){ }
        
        public DbSet<User> Users {get;set;}
    }      
}