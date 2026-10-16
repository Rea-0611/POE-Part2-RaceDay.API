namespace RaceDay.API.Models.Entities
{//start of namespace
    public class User
    {//start of class

        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"

        // Navigation properties
        public Organiser? Organiser { get; set; }
        public Participant? Participant { get; set; }
    }//end of class
}//end of namespace
