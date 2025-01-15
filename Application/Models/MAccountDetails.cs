namespace Application.Models
{
    public class MAccountDetails
    {
        public string AccountName { get; set; }
        public string AccountStatus { get; set; }
    }
    public class AccountDetailsStatus
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountStatus { get; set; }
        public string CIFID { get; set; } = "";
        public bool frez_code { get; set; }
    }
}
