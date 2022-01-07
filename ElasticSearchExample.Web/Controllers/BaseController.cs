using ElasticSearchExample.Data.DAL.Repository.Core;
using ElasticSearchExample.Web.Core;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ElasticSearchExample.Web.Controllers
{
    public class BaseController : Controller
    {
        private IWebHostEnvironment _hostingEnvironment;
        private IBackgroundJobClient _backgroundJobs;
        public BaseController(IWebHostEnvironment environment, IBackgroundJobClient backgroundJobs)
        {
            _hostingEnvironment = environment;
            _backgroundJobs = backgroundJobs;
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
        public static string ToPascalCase(string original)
        {
            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }
    }
}
