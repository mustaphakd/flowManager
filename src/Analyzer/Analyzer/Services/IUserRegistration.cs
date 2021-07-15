namespace Analyzer.Services
{
    public interface IUserRegistration
    {
        string identifier { get; set; }
        string Email { get; }
        string Password { get; set; }
    };
}
