namespace Entities.Exceptions
{
    public class PriceOutOfRangeException : BadRequestException
    {
        public PriceOutOfRangeException() 
            : base($"Price is out of range. It must be between 1 and 1000.")
        {
        }
    }
}
