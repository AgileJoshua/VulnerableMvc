using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace ConsoleTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string connectionString;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            connectionString = "";
        }

        private object Encode(string input)
        {
            var encoder = System.Text.Encodings.Web.HtmlEncoder.Create(new System.Text.Encodings.Web.TextEncoderSettings());
            var myObject = new { Text = encoder.Encode(input) };
            return myObject;
        }

        private IEnumerable<string> Query(string input)
        {
            var list = new List<string>();
            string queryString =
                @$"SELECT OrderID, CustomerID FROM dbo.Orders where name = '{input}'";

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(String.Format(" { 0 }, { 1 }", reader[0], reader[1]));
                    }
                }
            }
            return list;
        }

        public IActionResult Index(string vulnerableInput)
        {
            try
            {
                ViewBag.Encoded = Encode(vulnerableInput);
                ViewBag.QueryList = Query(vulnerableInput);
                return View();
            }
            catch
            {
                return Redirect(vulnerableInput);
            }
        }

        public IActionResult Index()
        {
            var vulnerableInput = Request.Form["vulnerable"];

            try
            {
                ViewBag.Encoded = Encode(vulnerableInput);
                ViewBag.QueryList = Query(vulnerableInput);
                return View();
            }
            catch
            {
                return Redirect(vulnerableInput);
            }
        }
    }
}