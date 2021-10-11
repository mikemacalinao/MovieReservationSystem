using MovieReservationSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MovieReservationSystem.Controllers
{
    public class CinemasController : Controller
    {
        private CallAPI CallAPI = new CallAPI();
        protected StringBuilder apiRequestBody = new StringBuilder();

        public async Task<ActionResult> Index()
        {
            await GetCinemas();

            return View();
        }

        public async Task<ActionResult> GetCinemas()
        {
            List<GetCinemas> getCinemas = new List<GetCinemas>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CallAPI.APIBaseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("cinemas");

                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    getCinemas = JsonConvert.DeserializeObject<List<GetCinemas>>(response);
                }

                foreach(GetCinemas cinema in getCinemas)
                {
                    if (cinema.reserved != 0)
                    {
                        cinema.disabled = "disabled";
                    }
                }

                return View(getCinemas);
            }
        }

        public async Task<ActionResult> GetCinema(int cinema_id = 0)
        {
            try
            {
                dynamic result = await CallAPI.Get(CallAPI.APIBaseURL + "cinemas?"
                    + "cinema_id=" + cinema_id
                    );

                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> CheckCinema(int cinema_id = 0, string description = "")
        {
            try
            {
                dynamic result = await CallAPI.Get(CallAPI.APIBaseURL + "cinemas/check?"
                    + "cinema_id=" + cinema_id
                    + "&description=" + description
                    );

                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> SaveCinema(SaveCinema param)
        {
            try
            {
                dynamic result = await CallAPI.Post(CallAPI.APIBaseURL + "cinemas/save",
                    apiRequestBody
                    .Append("cinema_id=" + param.cinema_id)
                    .Append("&description=" + HttpUtility.UrlEncode(param.description))
                    .Append("&seats=" + param.seats)
                    .Append("&default_price=" + param.default_price)
                    );

                return RedirectToAction("Index", "Cinemas");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }
    }
}