namespace RaceDay.API.Models.DTOs.Events
{//start of namespace
    public class EventDto
    {//start of class

        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EventStatus { get; set; } = string.Empty;
        public string OrganiserName { get; set; } = string.Empty;
        public List<CategoryDto> Categories { get; set; } = new();

    }//end of class
}//end of namespace
