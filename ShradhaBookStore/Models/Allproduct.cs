using System.ComponentModel.DataAnnotations.Schema;

namespace ShradhaBookStore.Models
{
    [NotMapped]
    public class Allproduct
    {
        public List<Product> Products { get; set;}
        public List<Manufacturer> Manufacturers { get; set;}
        public List<Category> Categories { get; set;}
        // Additional properties for first review stars and review counts
        public Dictionary<int, int?> FirstReviewStars { get; set; }
        public Dictionary<int, int> ReviewCounts { get; set; }
    }
}
