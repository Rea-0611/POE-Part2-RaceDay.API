namespace RaceDay.API.Models.DTOs.Categories
{//start of namespace
    public class CreateCategoryDto
    {//start of class

        public string CategoryName { get; set; } = string.Empty;
        public string CategoryType { get; set; } = "Distance";
        public decimal EntryFee { get; set; }

    }//end of class
}// 
