﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConsoleTest.Controllers
{
    public class AnObject{
        public void DoStuff(){

        }
    }
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string connectionString;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            connectionString = "";
        }

        public object WireSerialize(string input, System.IO.Stream s)
        {
            var wire = new Wire.Serializer();
            var theObject = wire.Deserialize(s);
            return theObject;
        }
        private object Encode(string input)
        {

            var obj1 = (AnObject)WireSerialize("my data", System.IO.File.OpenRead(input));

            var obj2 = (AnObject)WireSerialize(input, new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(input ?? "")));
            obj1.DoStuff();
            obj2.DoStuff();

            var encoder = System.Text.Encodings.Web.HtmlEncoder.Create(new System.Text.Encodings.Web.TextEncoderSettings());
            var myObject = new { Text = encoder.Encode(input) };
            return myObject;
        }

        private IEnumerable<string> Query(string input)
        {
            var list = new List<string>();
            string queryString =
                "SELECT OrderID, CustomerID FROM dbo.Orders where name = '" + input + "'";

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(String.Format("{0}, {1}", reader[0], reader[1]));
                    }
                }

                command = new SqlCommand("delete dbo.Orders where name = '" + input + "'", connection);
                command.ExecuteNonQuery();
            }
            int x = 0;
            doStuff(ref x);
            return list;

        }

        private void doStuff(ref int value)
        {
            value++;
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
            var vulnerableInput = Request.Form["vulnerable"].ToString();

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

        public IActionResult Index2()
        {
            var vulnerableInput = Request.Query["vulnerable"].ToString();

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