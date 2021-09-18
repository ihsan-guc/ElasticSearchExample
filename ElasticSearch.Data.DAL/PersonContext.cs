using ElasticSearchExample.Entites.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElasticSearchExample.Data.DAL
{
    public class PersonContext : DbContext
    {
        public PersonContext(DbContextOptions<PersonContext> options) : base(options)
        {

        }
        public DbSet<Person> Person { get; set; }

    }
}
