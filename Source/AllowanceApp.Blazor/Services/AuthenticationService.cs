
using Microsoft.AspNetCore.Identity;

namespace AllowanceApp.Blazor.Services
{
    public class AuthenticationService(IConfiguration config)
    {
        public bool IsAuthenticated { get; private set; } = false;
        public event Action? OnChange;
        private readonly IConfiguration _config = config;
        private readonly PasswordHasher<string> _hashpipe = new();

        public void Login(string password)
        {
            var storedPassword = _config["ParentAuth:PasswordHash"];
            if (storedPassword is not null && _hashpipe.VerifyHashedPassword("", storedPassword, password) == PasswordVerificationResult.Success)
            {
                IsAuthenticated = true;
                NotifyStateChanged();
            }
        }

        public void Logout()
        {
            IsAuthenticated = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}

