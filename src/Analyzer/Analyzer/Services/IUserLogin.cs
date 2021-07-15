namespace Analyzer.Services
{
    public interface IUserLogin {
        string identifier { get; set; }
        string Password { get; }
        string Email { get; }
    }

    internal class UserLogin : IUserLogin
    {
        public UserLogin(string email, string password)
        {
            Core.Check.NotNull(email, nameof(email));
            Core.Check.NotNull(password, nameof(password));

            Email = email;
            Password = password;
        }
        public string identifier { get; set; }
        public string Password { get; private set; }
        public string Email { get; private set; }
    }
}
