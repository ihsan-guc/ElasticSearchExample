using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchExample.Data.DAL.Repository.Core
{
    public interface IUnitOfWork
    {
        PersonRepository PersonRepository { get; set; }
        int Commit();
    }
}
