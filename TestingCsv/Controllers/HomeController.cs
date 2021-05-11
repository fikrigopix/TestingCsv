using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using TestingCsv.Models;
using TestingCsv.Models.Views;

namespace TestingCsv.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["message"] = TempData["message"];
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private static string FileLocation()
        {
            return Path.Combine(Environment.CurrentDirectory, $"Testing.csv");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertCsv()
        {
            string message = "success";
            try
            {
                using (var streamWriter = new StreamWriter(FileLocation()))
                {
                    using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                    {
                        GenerateCsvData csvData = new GenerateCsvData();

                        List<CsvData> datas = csvData.Generate(100000);
                        csvWriter.WriteRecords(datas);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            TempData["message"] = message;

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchCsv(string keyword)
        {
            List<CsvDataViewModel> viewModels = new List<CsvDataViewModel>();

            bool err = false;
            if (!string.IsNullOrEmpty(keyword))
            {
                try
                {
                    using (var streamReader = new StreamReader(FileLocation()))
                    {
                        using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                        {
                            var records = csvReader.GetRecords<CsvData>().ToList();

                            foreach (var item in records)
                            {
                                int foundCount = 0;
                                for (int i = 0; i < item.Content.Length; i++)
                                {
                                    if (i + keyword.Length <= item.Content.Length)
                                    {
                                        if (item.Content.Substring(i, keyword.Length) == keyword)
                                        {
                                            foundCount++;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (foundCount > 0)
                                {
                                    viewModels.Add(new CsvDataViewModel
                                    {
                                        Id = item.ID.ToString(),
                                        Content = item.Content,
                                        MatchTimes = foundCount
                                    });
                                }

                            }

                        }
                    }
                }
                catch (Exception)
                {
                    err = true;
                }
                
            }

            if (!err)
            {
                return Json(new { aaData = viewModels });
            }
            else
            {
                return Json(new { aaData = new List<CsvDataViewModel>() });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
