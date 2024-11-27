namespace AdamServer.Interfaces.WebApiHandlerService.Models
{
    public interface IAuthenticationChecker
    {
        bool ValidateCredentials(string username, string password);
    }

    public class AuthenticationChecker : IAuthenticationChecker
    {
        public bool ValidateCredentials(string username, string password)
        {
            return username.Equals("adam") && password.Equals("adam1234");
        }
    }
}
