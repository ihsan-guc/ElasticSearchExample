using ElasticSearchExample.Data.DAL.Repository.Core;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchExample.Web.Controllers
{
    public class BaseController : Controller
    {
        public IUnitOfWork _UnitOfWork { get; set; }
        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (_UnitOfWork == null)
                    _UnitOfWork = (IUnitOfWork)HttpContext.RequestServices.GetService(typeof(IUnitOfWork));
                return _UnitOfWork;
            }
        }
    }
}
