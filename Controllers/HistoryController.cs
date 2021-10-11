using MovieReservationSystem.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MovieReservationSystem.Controllers
{
    public class HistoryController : Controller
    {
        public async Task<ActionResult> Index(int schedule_id = 0)
        {
            await GetScheduleHistory(schedule_id);

            return View();
        }

        public async Task<ActionResult> GetScheduleHistory(int schedule_id = 0)
        {
            List<GetHistory> getScheduleHistory = new List<GetHistory>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CallAPI.APIBaseURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("schedules/history?"
                    + "schedule_id=" + schedule_id);

                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    getScheduleHistory = JsonConvert.DeserializeObject<List<GetHistory>>(response);
                }

                foreach (GetHistory schedule in getScheduleHistory)
                {
                    schedule.date_reserved_formatted = Convert.ToDateTime(schedule.date_reserved).ToString("MM/dd/yyyy hh:mm tt");

                    if (Convert.ToDateTime(schedule.date_cancelled) == Convert.ToDateTime("1/1/1900"))
                    {
                        schedule.date_cancelled_formatted = "";
                    }
                    else
                    {
                        schedule.date_cancelled_formatted = Convert.ToDateTime(schedule.date_cancelled).ToString("MM/dd/yyyy hh:mm tt");
                    }

                    if (schedule.status.ToUpper() == "RESERVED")
                    {
                        schedule.status_badge = "bg-warning";
                    }
                    else if (schedule.status.ToUpper() == "USED")
                    {
                        schedule.status_badge = "bg-success";
                        schedule.disabled = "disabled";
                    }
                    else
                    {
                        schedule.status_badge = "bg-danger";
                        schedule.disabled = "disabled";
                    }
                }

                return View(getScheduleHistory);
            }
        }

        public async Task<ActionResult> ExportHistory(int schedule_id = 0)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();

                List<GetHistory> getScheduleHistory = new List<GetHistory>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(CallAPI.APIBaseURL);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("schedules/history?"
                        + "schedule_id=" + schedule_id);

                    if (Res.IsSuccessStatusCode)
                    {
                        var response = Res.Content.ReadAsStringAsync().Result;
                        getScheduleHistory = JsonConvert.DeserializeObject<List<GetHistory>>(response);
                    }
                }

                if (getScheduleHistory.Count() == 0)
                {
                    return Json(new { data = "No records found." }, JsonRequestBehavior.AllowGet);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var excelPackage = new ExcelPackage(memoryStream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("History");

                    int row = 1, col = 1;

                    worksheet.Cells[row, col].Value = "CINEMA";
                    worksheet.Cells[row, ++col].Value = "MOVIE TITLE";
                    worksheet.Cells[row, ++col].Value = "DATE & TIME SHOWING";
                    worksheet.Cells[row, ++col].Value = "DATE & TIME RESERVED";
                    worksheet.Cells[row, ++col].Value = "CUSTOMER'S NAME";
                    worksheet.Cells[row, ++col].Value = "SEATS RESERVED";
                    worksheet.Cells[row, ++col].Value = "STATUS";
                    worksheet.Cells[row, ++col].Value = "DATE & TIME CANCELLED";

                    worksheet.Cells[row, 1, row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, col].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#f2f2f2"));
                    worksheet.Cells[row, 1, row, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, col].Style.Font.Bold = true;
                    worksheet.Row(row).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    foreach (GetHistory data in getScheduleHistory)
                    {
                        col = 1;
                        row++;

                        data.date_reserved_formatted = Convert.ToDateTime(data.date_reserved).ToString("MM/dd/yyyy hh:mm tt");
                        data.date_time_formatted = Convert.ToDateTime(data.date_time).ToString("MM/dd/yyyy hh:mm tt");

                        if (Convert.ToDateTime(data.date_cancelled) == Convert.ToDateTime("1/1/1900"))
                        {
                            data.date_cancelled_formatted = "";
                        }
                        else
                        {
                            data.date_cancelled_formatted = Convert.ToDateTime(data.date_cancelled).ToString("MM/dd/yyyy hh:mm tt");
                        }

                        worksheet.Cells[row, col].Value = data.description;
                        worksheet.Cells[row, ++col].Value = data.title;
                        worksheet.Cells[row, ++col].Value = data.date_time_formatted;
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, ++col].Value = data.date_reserved_formatted;
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, ++col].Value = data.customer_name;
                        worksheet.Cells[row, ++col].Value = data.seats;
                        worksheet.Cells[row, ++col].Value = data.status;
                        worksheet.Cells[row, ++col].Value = data.date_cancelled_formatted;
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    worksheet.DefaultRowHeight = 18;
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    TempData["Hisory"] = excelPackage.GetAsByteArray();
                    TempData["HisoryName"] = "Hisory-" + DateTime.Now.ToString("MMddyyyy");
                    return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception exception)
            {
                return Content(JsonConvert.SerializeObject(exception), "application/json");
            }
        }

        public ActionResult DownloadHistory()
        {
            try
            {
                if (TempData["Hisory"] != null)
                {
                    byte[] data = TempData["Hisory"] as byte[];
                    string fileName = TempData["HisoryName"].ToString();
                    return File(data, "application/octet-stream", fileName + ".xlsx");
                }
                else
                {
                    return new HttpNotFoundResult();
                }
            }
            catch (Exception exception)
            {
                return HttpNotFound();
            }
        }
    }
}