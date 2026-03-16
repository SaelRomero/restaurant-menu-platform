using System.Collections.Generic;

namespace MenuDigital.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; } = true;

        public Restaurant? Restaurant { get; set; }
        public ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
    }
}
