namespace Share.DataTransferModels.Auth
{
    public class RegisterPost
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Identification { get; set; }
        public string Dob { get; set; }
        public string Password { get; set; }
    }
}
