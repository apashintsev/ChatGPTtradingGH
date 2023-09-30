namespace ChatGPTtrading.Domain.Entities
{
    public class VerificationCode : BaseEntity
    {
        //Phone or number
        public string VerifyingEntity { get; private set; }

        public string Code { get; private set; }

        public int Attempts { get; private set; }

        public VerificationCode(string verifyingEntity) : base()
        {
            Attempts = 0;
            VerifyingEntity = verifyingEntity;
            Code = GenerateVerificationCode();
        }

        private string GenerateVerificationCode()
        {
            string verificationCode = "";
            for (int i = 0; i < 6; i++)
            {
                verificationCode += Random.Shared.Next(0, 10).ToString(); // Random number between 0 and 9
            }
            return verificationCode;
        }

        public void AddFailAttmpt()
        {
            Attempts++;
        }
    }
}
