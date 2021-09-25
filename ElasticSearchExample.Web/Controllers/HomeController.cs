﻿using ElasticSearchExample.Entites.Entities;
using ElasticSearchExample.Web.Core;
using ElasticSearchExample.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using RestSharp;
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
            return View();
        }

        public IActionResult PersonList()
        {
            var personList = UnitOfWork.PersonRepository.GetAll().OrderBy(p => Guid.NewGuid()).Take(10);
            PersonViewModel model = new PersonViewModel();
            model.Persons.AddRange(personList);
            return View(model);
        }
        public JsonResult PersonSearch(string searchValue)
        {
            var dataList = ElasticSearchName(searchValue)?.Select(p => p.Id.ToString());
            var personList = UnitOfWork.PersonRepository.GetQueryable().Where(p => dataList.Contains(p.Id.ToString()));
            return Json(personList);
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
                    var imageList = ImageUrlList()?.data?.Select(p => p.image_url);
                    if (imageList != null)
                    {
                        foreach (var imageItem in imageList)
                        {
                            var image = "https://randomwordgenerator.com" + imageItem;
                            images.Add(image);
                        }
                    }
                    else
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

                    Thread.Sleep(1000);
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
                                    person.FirstName = ToPascalCase(csvItem[1]?.ToString()?.ToLower());
                                    person.LastName = ToPascalCase(lastName.OrderBy(p => Guid.NewGuid()).FirstOrDefault()?.ToLower());
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
                    ElasticSearchCreateIndex();
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
        public List<ElasticSearchViewModel> ElasticSearchName(string value)
        {
            var client = ElasticSearchHelper.ElasticClientNode();
            var dataList = client.Search<ElasticSearchViewModel>(s =>
            s.Query(q => q.Bool(b => b.Should(sh => sh.Fuzzy(f => f.Field(fi => fi.FullName).Fuzziness(Fuzziness.EditDistance(1)).Boost(2)
                   .Value(value))
            , m => m.Match(mq => mq.Field(f => f.FullName).Query(value).Operator(Operator.And).Fuzziness(Fuzziness.EditDistance(1))))
            )).Size(10));
            //var dataList = client.Search<ElasticSearchViewModel>(s =>
            //s.Query(q => q.Bool(b => b.Should(sh => sh.Fuzzy(f => f.Field(fi => fi.FullName).Fuzziness(Fuzziness.EditDistance(1))
            //       .Value(value))
            //, m => m.Match(mq => mq.Field(f => f.FullName).Query(value).Operator(Operator.And).Fuzziness(Fuzziness.EditDistance(1)))
            //)).Size(10));
            return dataList.Documents.ToList();
        }

        public ImageDTO ImageUrlList()
        {
            var client = new RestClient("https://randomwordgenerator.com/json/pictures.php?category=all");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            ImageDTO imageList = JsonConvert.DeserializeObject<ImageDTO>(response.Content);
            return imageList;
        }
    }
}
