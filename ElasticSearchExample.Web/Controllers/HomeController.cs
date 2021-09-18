using ElasticSearchExample.Web.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;

namespace ElasticSearchExample.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult PersonUpload()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PersonUploads(IFormFile uploadFile)
        {
            if (uploadFile != null)
            {

                var pathExtension = Path.GetExtension(uploadFile.FileName);
                if (pathExtension != "xlsx")
                {
                    return RedirectToAction("Index", "Home");
                }
                MemoryStream target = new MemoryStream();
                uploadFile.CopyTo(target);

                IExcelDataReader excelReader;
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(target);
                var fileBytes = Convert.FromBase64String(Convert.ToBase64String(target.ToArray()));

                while (excelReader.Read())
                {
                    //excelReader.GetValue(1);
                }
            }

            return View();
        }

    }
}
