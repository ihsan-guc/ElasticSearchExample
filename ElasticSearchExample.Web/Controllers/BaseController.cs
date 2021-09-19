using ElasticSearchExample.Data.DAL.Repository.Core;
using ElasticSearchExample.Web.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchExample.Web.Controllers
{
    public class BaseController : Controller
    {
        private IWebHostEnvironment _hostingEnvironment;
        public BaseController(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }
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
