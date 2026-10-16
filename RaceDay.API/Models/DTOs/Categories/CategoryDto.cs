using RaceDay.API.Models.DTOs.Categories;

namespace RaceDay.API.Models.DTOs.Categories

{//start of namespace
    public class CategoryDto
    {//start of class
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryType { get; set; } = string.Empty;
        public decimal EntryFee { get; set; }


    }//end of class
}//end of namespace
