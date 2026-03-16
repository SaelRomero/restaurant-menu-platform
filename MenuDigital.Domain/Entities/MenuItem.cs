namespace MenuDigital.Domain.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsPromotion { get; set; } = false;
        public decimal DiscountPercent { get; set; } = 0;

        public Category? Category { get; set; }
    }
}
