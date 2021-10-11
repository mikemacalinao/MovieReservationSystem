using MovieReservationSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MovieReservationSystem.Controllers
{
    public class MoviesController : Controller
    {
        private CallAPI CallAPI = new CallAPI();
        protected StringBuilder apiRequestBody = new StringBuilder();

        public async Task<ActionResult> Index()
        {
            await GetMovies();

            return View();
        }

        public async Task<ActionResult> GetMovies()
        {
            List<GetMovies> getMovies = new List<GetMovies>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CallAPI.APIBaseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("movies");

                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    getMovies = JsonConvert.DeserializeObject<List<GetMovies>>(response);
                }

                foreach (GetMovies movie in getMovies)
                {
                    movie.imgurl = CallAPI.APIBaseURL + CallAPI.UploadFolder + movie.movie_id + movie.imgtype;

                    if (movie.reserved != 0)
                    {
                        movie.disabled = "disabled";
                    }
                }

                return View(getMovies);
            }
        }

        public async Task<ActionResult> GetMovie(int movie_id = 0)
        {
            try
            {
                dynamic result = await CallAPI.Get(CallAPI.APIBaseURL + "movies?"
                    + "movie_id=" + movie_id
                    );

                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> CheckMovie(int movie_id = 0, string title = "", string description = "")
        {
            try
            {
                dynamic result = await CallAPI.Get(CallAPI.APIBaseURL + "movies/check?"
                    + "movie_id=" + movie_id
                    + "&title=" + title
                    + "&description=" + description
                    );

                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public async Task<ActionResult> SaveMovie(SaveMovie param)
        {
            try
            {
                dynamic result;
                string imgtype = param.imgtype;

                if (param.imgfile != null)
                {
                    string filename = param.movie_id.ToString();

                    if (filename == "0")
                    {
                        result = await CallAPI.Get(CallAPI.APIBaseURL + "movies/newid");
                        filename = result.ToString();
                    }

                    imgtype = Path.GetExtension(param.imgfile.FileName);
                    string imgfile = CallAPI.UploadURL + filename + imgtype;
                    param.imgfile.SaveAs(imgfile);
                }
                

                result = await CallAPI.Post(CallAPI.APIBaseURL + "movies/save",
                    apiRequestBody
                    .Append("movie_id=" + param.movie_id)
                    .Append("&title=" + HttpUtility.UrlEncode(param.title))
                    .Append("&description=" + HttpUtility.UrlEncode(param.description))
                    .Append("&imgtype=" + imgtype)
                    .Append("&default_price=" + param.default_price)
                    );

                return RedirectToAction("Index", "Movies");
            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }
    }
}