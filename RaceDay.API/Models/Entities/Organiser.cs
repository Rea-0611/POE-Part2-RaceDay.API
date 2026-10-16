namespace RaceDay.API.Models.Entities
{//start of namespace
    public class Organiser
    {//start of class

        public int OrganiserId { get; set; } // PK + FK to User
        public string? OrganisationName { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }//end of class
}//end namespace
