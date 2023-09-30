namespace ChatGPTtrading.Domain.Entities
{
    public class Referral : BaseEntity
    {
        /// <summary>
        /// связь с пользователем, который привлек реферала
        /// </summary>
        public Guid UserId { get; private set; } 
        /// <summary>
        /// связь с рефералом
        /// </summary>
        public Guid ReferralUserId { get; private set; } 
        /// <summary>
        /// кто привёл
        /// </summary>
        public User User { get; }
        /// <summary>
        /// кого привёл
        /// </summary>
        public User ReferralUser { get; }

        private Referral()
        {

        }

        /// <summary>
        /// Создать реферальную связь
        /// </summary>
        /// <param name="user">Кто привлёк реферала</param>
        /// <param name="referralUser">Приведённый пользователь</param>
        public Referral(User user, User referralUser)
        {
            UserId = user.Id;
            User = user;
            ReferralUserId = referralUser.Id;
            ReferralUser = referralUser;
        }
    }
}
