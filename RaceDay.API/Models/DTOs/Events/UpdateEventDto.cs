namespace RaceDay.API.Models.DTOs.Events
{//start of namespace
    public class UpdateEventDto
    {//start of class

        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Distance { get; set; }
        public string EventType { get; set; } = "Run";
        public string EventStatus { get; set; } = "Draft";

    }//end of class
}//end of namespace
