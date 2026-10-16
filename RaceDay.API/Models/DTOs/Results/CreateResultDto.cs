namespace RaceDay.API.Models.DTOs.Results
{//start of namespace
    public class CreateResultDto
    {//start of class

        public TimeSpan? FinishTime { get; set; }
        public int? OverallPosition { get; set; }
        public int? CategoryPosition { get; set; }

    }//end of class
}//end of namespace
