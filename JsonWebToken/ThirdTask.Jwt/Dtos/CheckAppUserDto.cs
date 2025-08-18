namespace ThirdTask.Jwt.Dtos
{
    public class CheckAppUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public bool IsExist { get; set; }
        public List<string> Permissions { get; set; }
    }
}
