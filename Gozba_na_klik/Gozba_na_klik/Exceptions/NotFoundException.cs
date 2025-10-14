namespace Gozba_na_klik.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }

        public NotFoundException(int id)
            : base($"Entity with ID {id} was not found.") { }
    }
}
