namespace RaceDay.API.Models.DTOs.Enrolments
{//start of namespace
    public class EnrolmentDto
    {//start of class

        public int EnrolmentId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime EnrolmentDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;

    }//end of class
}//end of namespace
