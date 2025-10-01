namespace Gozba_na_klik.DTOs.Addresses
{
    public class AddressListItemDto
    {
        public int Id { get; set; }
        public string Label { get; set; } = "";
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public bool IsDefault { get; set; }
    }

    public class AddressCreateDto
    {
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
    }

    public class AddressUpdateDto : AddressCreateDto { }
}
