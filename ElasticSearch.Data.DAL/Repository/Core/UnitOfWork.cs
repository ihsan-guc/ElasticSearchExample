using System;

namespace ElasticSearchExample.Data.DAL.Repository.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        public PersonContext context;
        public UnitOfWork(PersonContext _context, PersonRepository _personRepository)
        {
            context = _context;
            PersonRepository = _personRepository;
        }

        public PersonRepository PersonRepository { get; set; }

        public int Commit()
        {
            try
            {
                return context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
