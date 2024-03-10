namespace ShradhaBookStore.Models
{
    public class CategoryProductViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
