namespace Gozba_na_klik.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }

        public BadRequestException(int id)
            : base($"Bad request for entity with ID: {id}") { }
    }
}
