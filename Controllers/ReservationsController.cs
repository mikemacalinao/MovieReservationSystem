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
    public class ReservationsController : Controller
    {
        private CallAPI CallAPI = new CallAPI();
        protected StringBuilder apiRequestBody = new StringBuilder();

        public async Task<ActionResult> Index(int schedule_id = 0)
        {
            await GetSeats(schedule_id);

            return View();
        }

        public async Task<ActionResult> GetSeats(int schedule_id = 0)
        {
            List<GetCinemaSeats> getCinemaSeats = new List<GetCinemaSeats>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CallAPI.APIBaseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("reservations/seats?"
                    + "schedule_id=" + schedule_id);

                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    getCinemaSeats = JsonConvert.DeserializeObject<List<GetCinemaSeats>>(response);
                }

                return View(getCinemaSeats);
            }
        }

        public async Task<ActionResult> GetCinemaSchedules(int cinema_id = 0)
        {
            try
            {
                dynamic result = await CallAPI.Get(CallAPI.APIBaseURL + "reservations/schedules?"
                    + "cinema_id=" + cinema_id
                    );
                
                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> GetCinemaSeats(int schedule_id = 0)
        {
            try
            {
                dynamic result = await CallAPI.Get(CallAPI.APIBaseURL + "reservations/seats?"
                    + "schedule_id=" + schedule_id
                    );

                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> Seats(int schedule_id = 0)
        {
            await GetSeats(schedule_id);

            return PartialView();
        }

        public async Task<ActionResult> SaveReservation(SaveReservation param)
        {
            try
            {
                dynamic result = await CallAPI.Post(CallAPI.APIBaseURL + "reservations/save",
                    apiRequestBody
                    .Append("schedule_id=" + param.schedule_id)
                    .Append("&customer_name=" + HttpUtility.UrlEncode(param.customer_name))
                    .Append("&reservation_details=" + param.reservation_details)
                    .Append("&total=" + param.total)
                    );

                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> CancelReservations(CancelReservations param)
        {
            try
            {
                dynamic result = await CallAPI.Post(CallAPI.APIBaseURL + "reservations/cancel",
                    apiRequestBody
                    .Append("cancel_reservation=" + param.cancel_reservation)
                    );

                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> CancelAllReservations(CancelAllReservations param)
        {
            try
            {
                dynamic result = await CallAPI.Post(CallAPI.APIBaseURL + "reservations/cancelall",
                    apiRequestBody
                    .Append("schedule_id=" + param.schedule_id)
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