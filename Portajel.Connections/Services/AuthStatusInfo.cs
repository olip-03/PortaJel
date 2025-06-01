namespace Portajel.Connections.Services
{
    public enum AuthState
    {
        NotStarted,
        InProgress,
        Success,
        Failed
    }

    public class AuthStatusInfo
    {
        public AuthState State { get; set; } = AuthState.NotStarted; // Default state
        public bool IsAuthenticating => State == AuthState.InProgress;
        public bool IsFailed => State == AuthState.Failed;
        public string Token { get; set; }
        public DateTime TokenExpiry { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public Dictionary<string, string> AdditionalData { get; set; } = new Dictionary<string, string>();
        
        public static AuthStatusInfo CreateAuthenticatedSuccess(string token, DateTime tokenExpiry, string message = "Authenticated successfully.", string userId = null, string username = null, List<string> roles = null, Dictionary<string, string> additionalData = null)
        {
             return new AuthStatusInfo
            {
                State = AuthState.Success,
                Token = token,
                TokenExpiry = tokenExpiry,
                Message = message,
                UserId = userId,
                Username = username,
                Roles = roles ?? new List<string>(),
                AdditionalData = additionalData ?? new Dictionary<string, string>()
            };
        }

        public static AuthStatusInfo CreateAnonymousSuccess(string message = "Anonymous access granted.")
        {
            return new AuthStatusInfo
            {
                State = AuthState.Success,
                Token = string.Empty, // No token for anonymous
                TokenExpiry = DateTime.MinValue, // No expiry for anonymous
                Message = message,
                UserId = "Anonymous",
                Username = "Anonymous",
                Roles = new List<string> { "Anonymous" },
                AdditionalData = new Dictionary<string, string> { { "AccessLevel", "Anonymous" } }
            };
        }


        // Represents an authentication failure
        public static AuthStatusInfo CreateFailed(string message)
        {
            return new AuthStatusInfo
            {
                State = AuthState.Failed,
                Token = string.Empty, // No token on failure
                TokenExpiry = DateTime.MinValue, // No expiry on failure
                Message = message,
                UserId = null, // No user info on failure
                Username = null, // No user info on failure
                Roles = new List<string>(),
                AdditionalData = new Dictionary<string, string>()
            };
        }

         // Factory method for indicating that authentication is currently in progress
        public static AuthStatusInfo CreateInProgress(string message = "Authentication in progress...")
        {
            return new AuthStatusInfo
            {
                State = AuthState.InProgress,
                Message = message,
                // Other properties would typically be default/null at this stage
                Token = string.Empty,
                TokenExpiry = DateTime.MinValue,
                UserId = null,
                Username = null,
                Roles = new List<string>(),
                AdditionalData = new Dictionary<string, string>()
            };
        }

        // --- Mapped Original Static Methods (Optional - Keep for backward compatibility) ---
        // You can keep these if existing code calls them, mapping them to the new structure.
        // Otherwise, you can remove them and update call sites to use the new Create methods.

        public static AuthStatusInfo Ok()
        {
            // Original Ok() seems to represent a success state without specific user/token details,
            // likely anonymous access or a state where authentication wasn't needed but is confirmed ok.
            return CreateAnonymousSuccess();
        }

        public static AuthStatusInfo Unauthorized(string message = "Unauthorized")
        {
            // Unauthorized is a specific type of failure
            return CreateFailed(message);
        }

        public static AuthStatusInfo Unneccesary()
        {
            // Unnecessary means authentication wasn't required but is successfully bypassed or allowed anonymously.
            return CreateAnonymousSuccess("This connection does not need to be authenticated, anonymous access is permitted by default");
        }

         // Original Failed method
        public static AuthStatusInfo Failed(string message)
        {
            // This maps directly to the new CreateFailed method
            return CreateFailed(message);
        }
    }
}
