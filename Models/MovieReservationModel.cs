using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MovieReservationSystem.Models
{
    #region ***** CINEMAS *****
    public class GetCinemas
    {
        public byte cinema_id { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "Seat Count")]
        public int seat_count { get; set; }

        [Display(Name = "Default Price")]
        public decimal default_price { get; set; }
        public int reserved { get; set; }
        public string disabled { get; set; }
    }
    public class SaveCinema
    {
        public byte cinema_id { get; set; }
        public string description { get; set; }
        public int seats { get; set; }
        public decimal default_price { get; set; }
    }

    public class Seat
    {
        public byte seat_id { get; set; }
        public byte row { get; set; }
        public byte col { get; set; }
    }

    #endregion

    #region ***** MOVIES *****

    public class GetMovies
    {
        public int movie_id { get; set; }

        [Display(Name = "Title")]
        public string title { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }
        public string imgtype { get; set; }
        public string imgurl { get; set; }

        [Display(Name = "Default Price")]
        public decimal default_price { get; set; }
        public int reserved { get; set; }
        public string disabled { get; set; }
    }

    public class SaveMovie
    {
        public int movie_id { get; set; }
        public string title { get; set; } = "";
        public string description { get; set; } = "";
        public string imgtype { get; set; } = "";
        public HttpPostedFileBase imgfile { get; set; }
        public decimal default_price { get; set; }
    }

    #endregion

    #region ***** SCHEDULES *****

    public class GetSchedules
    {
        public int schedule_id { get; set; }
        public int movie_id { get; set; }
        public byte cinema_id { get; set; }

        [Display(Name = "Price")]
        public decimal price { get; set; }

        [Display(Name = "Date & Time")]
        public string date_time { get; set; }
        public string date_time_formatted { get; set; }

        [Display(Name = "Movie")]
        public string title { get; set; }

        [Display(Name = "Cinema")]
        public string description { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }
        public string status_badge { get; set; }
        public string disabled { get; set; }
        public int reserved { get; set; }
    }

    public class SaveSchedule
    {
        public int schedule_id { get; set; }
        public int movie_id { get; set; }
        public byte cinema_id { get; set; }
        public decimal price { get; set; }
        public string date_time { get; set; }
    }

    public class GetHistory
    {
        public int reservation_id { get; set; }

        [Display(Name = "Date & Time Reserved")]
        public string date_reserved { get; set; }
        public string date_reserved_formatted { get; set; }

        [Display(Name = "Customer's Name")]
        public string customer_name { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }
        public string status_badge { get; set; }

        [Display(Name = "Seats Reserved")]
        public string seats { get; set; }
        public string disabled { get; set; }
        public string date_time { get; set; }
        public string date_time_formatted { get; set; }
        public string description { get; set; }
        public string title { get; set; }

        [Display(Name = "Date & Time Cancelled")]
        public string date_cancelled { get; set; }
        public string date_cancelled_formatted { get; set; }
    }

    #endregion

    #region ***** RESERVATIONS *****

    public class GetNowShowing
    {
        public byte cinema_id { get; set; }
        public string description { get; set; }
        public int seat_count { get; set; }
        public string seat_count_badge { get; set; }
        public int schedule_id { get; set; }
        public int movie_id { get; set; }
        public decimal price { get; set; }
        public string date_time { get; set; }
        public string date_time_formatted { get; set; }
        public string starts { get; set; }
        public string on { get; set; }
        public string at { get; set; }
        public string movietitle { get; set; }
        public string moviedescription { get; set; }
        public string imgtype { get; set; }
        public string imgurl { get; set; }
    }

    public class GetCinemaSeats
    {
        public int seat_id { get; set; }
        public byte row { get; set; }
        public byte col { get; set; }
        public string status { get; set; }
    }

    public class SaveReservation
    {
        public int schedule_id { get; set; }
        public string customer_name { get; set; }
        public string reservation_details { get; set; }
        public decimal total { get; set; }
    }

    public class ReservationDetail
    {
        public int seat_id { get; set; }
    }

    public class CancelReservations
    {
        public string cancel_reservation { get; set; }
    }

    public class Reservation
    {
        public int reservation_id { get; set; }
    }

    public class CancelAllReservations
    {
        public int schedule_id { get; set; }
    }

    #endregion
}