namespace RaceDay.API.Models.Entities
{//start of namespace
    public class Category
    {//start of class

        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryType { get; set; } = "Distance"; // "Age" or "Distance"
        public decimal EntryFee { get; set; }

        // Navigation properties
        public Event Event { get; set; } = null!;
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    }//end of class
}//end of namespace
