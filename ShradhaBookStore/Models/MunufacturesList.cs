using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShradhaBookStore.Models
{
    public class MunufacturesList
    {
        public int Id {  get; set; } 
        public List<SelectListItem> ManufacturerList { get; set; }
    }
}
