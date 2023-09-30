using Microsoft.AspNetCore.Identity;

namespace ChatGPTtrading.Infrastructure
{
    /// <summary>
    /// Default user for application.
    /// Add profile data for application users by adding properties to the ApplicationUser class
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Profile identity
        /// </summary>
        public Guid UserId { get; private set; }

        public ApplicationUser(Guid userId) : base()
        {
            UserId = userId;
        }
    }
}