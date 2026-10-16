namespace RaceDay.API.Models.DTOs.Results
{//start of namespace
    public class ResultDto
    {//start of class

        public int ResultId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public TimeSpan? FinishTime { get; set; }
        public int? OverallPosition { get; set; }
        public int? CategoryPosition { get; set; }

    }//end of class
}//end of namespace
