using MovieReservationSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MovieReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            await GetNowShowing();

            return View();
        }

        public async Task<ActionResult> GetNowShowing()
        {
            List<GetNowShowing> getNowShowing = new List<GetNowShowing>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CallAPI.APIBaseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("nowshowing");

                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    getNowShowing = JsonConvert.DeserializeObject<List<GetNowShowing>>(response);
                }

                foreach (GetNowShowing nowShowing in getNowShowing)
                {
                    nowShowing.imgurl = CallAPI.APIBaseURL + CallAPI.UploadFolder + nowShowing.movie_id + nowShowing.imgtype;
                    nowShowing.date_time_formatted = Convert.ToDateTime(nowShowing.date_time).ToString("hh:mm tt");

                    if (Convert.ToDateTime(nowShowing.date_time).Date == DateTime.Now.Date)
                    {
                        nowShowing.starts = "Today";
                        nowShowing.at = " at";
                    }
                    else if (Convert.ToDateTime(nowShowing.date_time).Date == DateTime.Now.Date.AddDays(1))
                    {
                        nowShowing.starts = "Tomorrow";
                        nowShowing.at = " at";
                    }
                    else
                    {
                        nowShowing.starts = Convert.ToDateTime(nowShowing.date_time).ToString("MMMM dd");
                        nowShowing.on = "on ";
                    }

                    if (nowShowing.seat_count == 0)
                    {
                        nowShowing.seat_count_badge = "bg-dark";
                    }
                    else if (nowShowing.seat_count <= 5)
                    {
                        nowShowing.seat_count_badge = "bg-danger";
                    }
                    else if (nowShowing.seat_count <= 10)
                    {
                        nowShowing.seat_count_badge = "bg-warning";
                    }
                    else
                    {
                        nowShowing.seat_count_badge = "bg-success ";
                    }
                    
                }

                return View(getNowShowing);
            }
        }
    }
}