namespace Tabi
{
    public interface IAppConfiguration
    {
        string AppName { get; set; }
        bool Developer { get; set; }
        string LicensesUrl { get; set; }
        string AgreementUrl { get; set; }
    }
}