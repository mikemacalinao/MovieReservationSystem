using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MovieReservationSystem.Controllers
{
    public class CallAPI
    {
        public static string APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"].ToString();
        public static string UploadURL = ConfigurationManager.AppSettings["UploadURL"].ToString();
        public static string UploadFolder = ConfigurationManager.AppSettings["UploadFolder"].ToString();

        public async Task<object> Get(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, new CancellationTokenSource().Token))
                    {
                        var stream = await response.Content.ReadAsStreamAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            using (var streamReader = new StreamReader(stream))
                            {
                                string streamContent = await streamReader.ReadToEndAsync();
                                JavaScriptSerializer serializer = new JavaScriptSerializer
                                {
                                    MaxJsonLength = int.MaxValue
                                };

                                return (dynamic)serializer.DeserializeObject(streamContent);
                            }
                        }

                        return false;
                    }
                }

            }
            catch (Exception exception)
            {
                if (exception is TaskCanceledException)
                {
                    TaskCanceledException taskCanceledException = new TaskCanceledException(exception.Message, exception);

                    if (!taskCanceledException.CancellationToken.IsCancellationRequested)
                    {
                        return new { error = (int)HttpStatusCode.RequestTimeout, message = $"Error Code: {(int)HttpStatusCode.RequestTimeout} - Request Timeout" };
                    }
                }

                return new { error = exception.StackTrace, message = exception?.InnerException.Message ?? exception.Message };
            }
        }

        public async Task<object> Post(string url, StringBuilder data)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        request.Content = new StringContent(data.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
                        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, new CancellationTokenSource().Token))
                        {
                            var stream = await response.Content.ReadAsStreamAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                using (var streamReader = new StreamReader(stream))
                                {
                                    string streamContent = await streamReader.ReadToEndAsync();
                                    JavaScriptSerializer serializer = new JavaScriptSerializer
                                    {
                                        MaxJsonLength = int.MaxValue
                                    };

                                    return (dynamic)serializer.DeserializeObject(streamContent);
                                }
                            }

                            return false;
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                if (exception is TaskCanceledException)
                {
                    TaskCanceledException taskCanceledException = new TaskCanceledException(exception.Message, exception);

                    if (!taskCanceledException.CancellationToken.IsCancellationRequested)
                    {
                        return new { error = (int)HttpStatusCode.RequestTimeout, message = $"Error Code: {(int)HttpStatusCode.RequestTimeout} - Request Timeout" };
                    }
                }
                return new { error = exception.StackTrace, message = exception?.InnerException.Message ?? exception.Message };
            }
        }

        public async Task<object> Delete(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    using (var request = new HttpRequestMessage(HttpMethod.Delete, url))
                    using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, new CancellationTokenSource().Token))
                    {
                        var stream = await response.Content.ReadAsStreamAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            using (var streamReader = new StreamReader(stream))
                            {
                                string streamContent = await streamReader.ReadToEndAsync();
                                JavaScriptSerializer serializer = new JavaScriptSerializer
                                {
                                    MaxJsonLength = int.MaxValue
                                };

                                return (dynamic)serializer.DeserializeObject(streamContent);
                            }
                        }

                        return false;
                    }
                }
            }
            catch (Exception exception)
            {
                if (exception is TaskCanceledException)
                {
                    TaskCanceledException taskCanceledException = new TaskCanceledException(exception.Message, exception);
                    // Check exception.CancellationToken.IsCancellationRequested here.
                    // If false, it's pretty safe to assume it was a timeout.
                    if (!taskCanceledException.CancellationToken.IsCancellationRequested)
                    {
                        return new { error = (int)HttpStatusCode.RequestTimeout, message = $"Error Code: {(int)HttpStatusCode.RequestTimeout} - Request Timeout" };
                    }
                }
                return new { error = exception.StackTrace, message = exception?.InnerException.Message ?? exception.Message };
            }
        }
    }
}