using System;

namespace Gozba_na_klik.Models.Customers
{
	public class Address
	{
		public int Id { get; set; }
		public int UserId { get; set; }

		public string Label { get; set; } = "";
		public string Street { get; set; } = "";
		public string City { get; set; } = "";
		public string PostalCode { get; set; } = "";

		public string? Entrance { get; set; }
		public string? Floor { get; set; }
		public string? Apartment { get; set; }

		public double? Latitude { get; set; }
		public double? Longitude { get; set; }
		public string? Notes { get; set; }

		public bool IsDefault { get; set; }
		public bool IsActive { get; set; } = true;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
