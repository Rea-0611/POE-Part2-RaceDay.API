namespace RaceDay.API.Models.DTOs.Auth
{//start of namespace
    public class AuthResponseDto
    {//start of class

        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

    }//end of class
}//end of namespace
