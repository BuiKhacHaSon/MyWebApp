using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyWebApp.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("read")]
        public async Task<IActionResult> ReadMessagePage()
        {
            List<Message> lsMessage = new List<Message>();
            HttpClient httpClient = new HttpClient();
            var apiResponse = await httpClient.GetAsync("http://localhost:5001/mes");
            try
            {
                string response = await apiResponse.Content.ReadAsStringAsync();
                response = "{\"data\":" + response + "}";
                JObject jObResponse = JObject.Parse(response);
                lsMessage = JsonConvert.DeserializeObject<List<Message>>(jObResponse["data"].ToString());
            }
            catch(Exception ex)
            {
                lsMessage = null;
            }
            return View(lsMessage);
        }
        [Route("readnew")]
        public async Task<IActionResult> ReadNewMessagePage()
        {
            List<Message> lsMessage = new List<Message>();
            HttpClient httpClient = new HttpClient();
            var apiResponse = await httpClient.GetAsync("http://localhost:5001/mes/new");
            try
            {
                string response = await apiResponse.Content.ReadAsStringAsync();
                response = "{\"data\":" + response + "}";
                JObject jObResponse = JObject.Parse(response);
                lsMessage = JsonConvert.DeserializeObject<List<Message>>(jObResponse["data"].ToString());
            }
            catch (Exception ex)
            {
                lsMessage = null;
            }
            return View(lsMessage);
        }

        public async Task<IActionResult> CheckRead(int id)
        {
            HttpClient httpClient = new HttpClient();
            var httpContent = new StringContent(null, Encoding.UTF8, "application/json"); ;
            var apiResponse = await httpClient.PutAsync("http://localhost:5001/mes/" + id.ToString(), httpContent);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Delte(int id)
        {
            HttpClient httpClient = new HttpClient();
            var apiResponse = await httpClient.DeleteAsync("http://localhost:5001/mes/" + id.ToString());
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
