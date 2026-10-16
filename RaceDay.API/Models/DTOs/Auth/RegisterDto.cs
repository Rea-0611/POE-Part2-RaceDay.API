namespace RaceDay.API.Models.DTOs.Auth
{//start of namespace
    public class RegisterDto
    {//start of class

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"

        // Organiser-specific
        public string? OrganisationName { get; set; }

        // Participant-specific
        public DateTime? DateOfBirth { get; set; }
        public string? ContactNumber { get; set; }

    }//end of class
}//end of namespace
