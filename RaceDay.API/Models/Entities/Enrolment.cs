namespace RaceDay.API.Models.Entities
{//start of namespace
    public class Enrolment
    {//start of class

        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public int CategoryId { get; set; }
        public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;
        public string PaymentStatus { get; set; } = "Pending"; // "Pending" or "Paid"

        // Navigation properties
        public Participant Participant { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public Result? Result { get; set; }

    }//end of class
}//end of namespace
