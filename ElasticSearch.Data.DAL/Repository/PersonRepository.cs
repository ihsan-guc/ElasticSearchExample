using ElasticSearchExample.Data.DAL.Repository.Core;
using ElasticSearchExample.Entites.Entities;

namespace ElasticSearchExample.Data.DAL.Repository
{
    public interface IPersonRepository : IRepository<Person>
    {

    }
    public class PersonRepository : EfRepository<Person>, IPersonRepository
    {
        public PersonRepository(PersonContext context) : base(context)
        {

        }
    }
}
