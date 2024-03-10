using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShradhaBookStore.Models
{
    public class CategoryList
    {
        public int Id { get; set; }
        public List<SelectListItem> CategorList { get; set;}
    }
}
