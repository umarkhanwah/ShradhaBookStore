using System.ComponentModel.DataAnnotations.Schema;

namespace ShradhaBookStore.Models
{
    [NotMapped]
    public class Allproduct
    {
        public List<Product> Products { get; set;}
        public List<Manufacturer> Manufacturers { get; set;}
        public List<Category> Categories { get; set;}
    }
}
