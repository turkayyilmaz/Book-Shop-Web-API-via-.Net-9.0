namespace Entities.Exceptions
{
    // şimdi kimse ulaşamasın diye mühürlediğimiz entity not found classımızı yazalım
    public sealed class BookNotFoundException : NotFoundException
    {
        public BookNotFoundException(int id) : base($"The book with {id} could not be found.")
        {

        }
    }
}
