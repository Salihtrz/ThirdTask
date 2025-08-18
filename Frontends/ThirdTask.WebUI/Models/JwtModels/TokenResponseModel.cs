namespace ThirdTask.WebUI.Models.JwtModels
{
    public class TokenResponseModel
    {
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
