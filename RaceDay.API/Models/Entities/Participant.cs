namespace RaceDay.API.Models.Entities
{//start of namespace
    public class Participant
    {//start of class

        public int ParticipantId { get; set; } // PK + FK to User
        public DateTime? DateOfBirth { get; set; }
        public string? ContactNumber { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    }//end of class
}//end namespace
