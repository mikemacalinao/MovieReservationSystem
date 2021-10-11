using MovieReservationSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MovieReservationSystem.Controllers
{
    public class SchedulesController : Controller
    {
        private CallAPI CallAPI = new CallAPI();
        protected StringBuilder apiRequestBody = new StringBuilder();

        public async Task<ActionResult> Index()
        {
            await GetSchedules();

            return View();
        }

        public async Task<ActionResult> GetSchedules()
        {
            List<GetSchedules> getSchedules = new List<GetSchedules>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CallAPI.APIBaseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("schedules");

                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    getSchedules = JsonConvert.DeserializeObject<List<GetSchedules>>(response);
                }

                foreach(GetSchedules schedule in getSchedules)
                {
                    schedule.date_time_formatted = Convert.ToDateTime(schedule.date_time).ToString("MM/dd/yyyy hh:mm tt");

                    if (schedule.reserved != 0)
                    {
                        schedule.disabled = "disabled";
                    }

                    if (schedule.status.ToUpper() == "OPEN")
                    {
                        schedule.status_badge = "bg-success";
                    }
                    else
                    {
                        schedule.status_badge = "bg-danger";
                        schedule.disabled = "disabled";
                    }
                }

                return View(getSchedules);
            }
        }

        public async Task<ActionResult> GetSchedule(int schedule_id = 0)
        {
            try
            {
                dynamic result = await CallAPI.Get(CallAPI.APIBaseURL + "schedules?"
                    + "schedule_id=" + schedule_id
                    );

                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> CheckSchedule(int schedule_id = 0, int cinema_id = 0, string date_time = "")
        {
            try
            {
                dynamic result = await CallAPI.Get(CallAPI.APIBaseURL + "schedules/check?"
                    + "schedule_id=" + schedule_id
                    + "&cinema_id=" + cinema_id
                    + "&date_time=" + date_time
                    );

                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> CheckScheduleCount(int schedule_id = 0, int cinema_id = 0, string date_time = "")
        {
            try
            {
                dynamic result = await CallAPI.Get(CallAPI.APIBaseURL + "schedules/checkschedulecount?"
                    + "schedule_id=" + schedule_id
                    + "&cinema_id=" + cinema_id
                    + "&date_time=" + date_time
                    );

                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> SaveSchedule(SaveSchedule param)
        {
            try
            {
                DateTime date_time = string.IsNullOrEmpty(param.date_time) ? Convert.ToDateTime("1/1/1900") : Convert.ToDateTime(param.date_time);

                dynamic result = await CallAPI.Post(CallAPI.APIBaseURL + "schedules/save",
                    apiRequestBody
                    .Append("schedule_id=" + param.schedule_id)
                    .Append("&movie_id=" + param.movie_id)
                    .Append("&cinema_id=" + param.cinema_id)
                    .Append("&price=" + param.price)
                    .Append("&date_time=" + date_time)
                    );

                return RedirectToAction("Index", "Schedules");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> GetScheduleHistory(int schedule_id = 0)
        {
            try
            {
                dynamic result = await CallAPI.Get(CallAPI.APIBaseURL + "schedules/history?"
                    + "schedule_id=" + schedule_id
                    );

                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }
    }
}