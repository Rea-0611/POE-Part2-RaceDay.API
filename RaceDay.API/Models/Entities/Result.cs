namespace RaceDay.API.Models.Entities
{//start of namespace
    public class Result
    {//start of class

        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? OverallPosition { get; set; }
        public int? CategoryPosition { get; set; }

        // Navigation properties
        public Enrolment Enrolment { get; set; } = null!;

    }//end of class
}//end namespace
