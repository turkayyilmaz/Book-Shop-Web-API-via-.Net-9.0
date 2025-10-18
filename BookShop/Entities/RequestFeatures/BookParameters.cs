namespace Entities.RequestFeatures
{
    public class BookParameters : RequestParameters
    {
        // uint => değer negatif olamaz
        public uint MinPrice { get; set; } = 0;
        public uint MaxPrice { get; set; } = uint.MaxValue;
        // MaxPrice'ın MinPrice'tan büyük olup olmadığını kontrol eder
        public bool ValidPriceRange => MaxPrice > MinPrice;
        public string? SearchTerm { get; set; }
        public BookParameters()
        {
            OrderBy = "BookId";
        }
    }
}
