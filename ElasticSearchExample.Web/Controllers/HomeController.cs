using ElasticSearchExample.Entites.Entities;
using ElasticSearchExample.Web.Core;
using ElasticSearchExample.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ElasticSearchExample.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private IWebHostEnvironment _hostingEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment) : base(environment)
        {
            _logger = logger;
            _hostingEnvironment = environment;
        }

        public IActionResult Index()
        {
            ElasticSearchHelper.CreateNewIndex();

            var client = ElasticSearchHelper.ElasticClientNode();
            //var response = client.Search<ElasticSearchViewModel>(p => p
            //  .From(0)
            //  .Size(10)
            //  .Query(q =>
            //  q.Term(f => f.FullName, 2) || q.MatchPhrasePrefix(mq => mq.Field(f => f.FullName).Query("HANNUS"))));
            //var responses = response.Documents?.ToList();

            var dataList = client.Search<ElasticSearchViewModel>(s =>
            s.Query(q => q.Bool(b => b.Should(sh => sh.Fuzzy(f => f.Field(fi => fi.FullName).Fuzziness(Fuzziness.EditDistance(2))
                  .Value("Search Value"))))).Size(100));
            var response = dataList.Documents;
            return View();
        }

        public IActionResult PersonList()
        {
            var personList = UnitOfWork.PersonRepository.GetAll().ToList();
            PersonViewModel model = new PersonViewModel();
            model.Persons.AddRange(personList);
            return View(model);
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
                if (pathExtension == ".csv")
                {
                    MemoryStream target = new MemoryStream();
                    uploadFile.CopyTo(target);
                    var lastName = new List<string>();
                    var images = new List<string>();
                    images.AddRange(new string[] { "https://cdn.pixabay.com/photo/2021/08/04/03/06/hanoi-6520941__480.jpg", "https://cdn.pixabay.com/photo/2020/01/20/22/21/palace-4781577__340.jpg", "https://cdn.pixabay.com/photo/2017/07/15/19/13/hotel-2507432__340.jpg", "https://cdn.pixabay.com/photo/2021/06/22/14/55/girl-6356393__340.jpg", "https://cdn.pixabay.com/photo/2020/12/03/12/35/sunset-5800386__340.jpg", "https://cdn.pixabay.com/photo/2019/03/25/20/17/kaohsiung-4081259__340.jpg" });
                    var lastNamePath = Path.Combine(_hostingEnvironment.WebRootPath) + "\\LastName\\" + "LastName.csv";
                    using (StreamReader sr = new StreamReader(lastNamePath, Encoding.Default, true))
                    {
                        string currentLine;
                        int counter = 1;
                        Regex rexCsvSplitter = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
                        var count = 1;
                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            if (count > 1)
                            {
                                var row = currentLine;
                                var csvLastItem = rexCsvSplitter.Split(row);
                                lastName.Add(csvLastItem[3]?.ToString());
                            }
                            count++;

                        }
                    }
                    var path = Path.Combine(_hostingEnvironment.WebRootPath) + "\\FileDocument\\" + Guid.NewGuid().ToString().Substring(0, 8) + "_UploadPerson" + pathExtension;

                    var returnPath = path;
                    var fileBytes = Convert.FromBase64String(Convert.ToBase64String(target.ToArray()));

                    using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                    {
                        fs.Write(fileBytes, 0, (int)fileBytes.Length);
                        fs.Close();
                        fs.Close();
                    }

                    Thread.Sleep(3000);
                    using (StreamReader sr = new StreamReader(path, Encoding.Default, true))
                    {
                        string currentLine;
                        int counter = 1;
                        Regex rexCsvSplitter = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
                        var count = 1;
                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            if (count > 1)
                            {
                                var person = new Person();
                                var row = currentLine;
                                var csvItem = rexCsvSplitter.Split(row);
                                //if (csvItem.Count() == 1)
                                //{
                                person.FirstName = csvItem[1]?.ToString()?.ToLower();
                                person.LastName = lastName.OrderBy(p => Guid.NewGuid()).FirstOrDefault()?.ToLower();
                                person.ImagePath = images.OrderBy(p => Guid.NewGuid()).FirstOrDefault();
                                person.Email = (person.FirstName + person.LastName).ToLower() + "@companygmail.com";
                                person.UserName = (person.FirstName + " " + person.LastName).ToUpper();
                                person.Password = person.FirstName + "123";
                                UnitOfWork.PersonRepository.Add(person);

                                //}
                            }
                            count++;
                        }
                        UnitOfWork.Commit();
                    }
                    
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

        public async void ElasticSearchCreateIndex()
        {
            var personList = UnitOfWork.PersonRepository.GetAll().Where(p => p.CreateDate >= DateTime.Now.AddHours(-1)).ToList().Select(p => new ElasticSearchViewModel()
            {
                Id = p.Id,
                FullName = p.FirstName + " " + p.LastName
            });
            ElasticSearchHelper.CreateNewIndex();
            var client = ElasticSearchHelper.ElasticClientNode();
            var response = client.IndexManyAsync(personList.Take(2000), "defaultindex");
        }

        public static string ToPascalCase(string original)
        {
            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }
    }
}
