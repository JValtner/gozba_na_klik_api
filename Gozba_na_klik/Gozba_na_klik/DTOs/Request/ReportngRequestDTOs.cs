using System;
using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Request
{
    public class RestaurantProfitReportRequestDTO
    {
        [Required(ErrorMessage = "RestaurantId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "RestaurantId must be a positive integer.")]
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "StartDate must be a valid DateTime.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "EndDate must be a valid DateTime.")]
        public DateTime EndDate { get; set; }
    }

    public class MealSalesReportRequestDTO
    {
        [Required(ErrorMessage = "RestaurantId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "RestaurantId must be a positive integer.")]
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "MealId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "MealId must be a positive integer.")]
        public int MealId { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "StartDate must be a valid DateTime.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "EndDate must be a valid DateTime.")]
        public DateTime EndDate { get; set; }
    }

    public class RestaurantOrdersReportRequestDTO
    {
        [Required(ErrorMessage = "RestaurantId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "RestaurantId must be a positive integer.")]
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "StartDate must be a valid DateTime.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "EndDate must be a valid DateTime.")]
        public DateTime EndDate { get; set; }
    }
}