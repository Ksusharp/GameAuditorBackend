namespace Users.App.Models
{
    public class Token
    {
        public string AccessToken { get; set; }

        public DateTime AccessTokenExpiryTime { get; set; }
    }
}
