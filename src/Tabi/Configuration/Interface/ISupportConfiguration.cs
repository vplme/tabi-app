namespace Tabi
{
    public interface ISupportConfiguration
    {
        bool Available { get; set; }
        string PhoneNumber { get; set; }
        string DisplayPhoneNumber { get; set; }
        string Email { get; set; }
        string EmailSubject { get; set; }
        string Url { get; set; }
        string DisplayUrl { get; set; }
    }
}