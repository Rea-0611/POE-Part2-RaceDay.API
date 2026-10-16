namespace RaceDay.API.Models.Entities
{//start of namespace
    public class Event
    {//start of class

        public int EventId { get; set; }
        public int OrganiserId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Distance { get; set; }
        public string EventType { get; set; } = "Run"; // "Run", "Walk", "Cycle"
        public string EventStatus { get; set; } = "Draft";

        // Navigation properties
        public Organiser Organiser { get; set; } = null!;
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }//end of class
}//end of namespace
