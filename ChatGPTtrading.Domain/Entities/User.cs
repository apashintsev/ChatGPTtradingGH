using System.Numerics;
using System.Text.RegularExpressions;

namespace ChatGPTtrading.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    /// <summary>
    /// Место жительства
    /// </summary>
    public string Residency { get; private set; }
    /// <summary>
    /// Гражданство
    /// </summary>
    public string Citizenship { get; private set; }
    /// <summary>
    /// Вид деятельности
    /// </summary>
    public string Occupation { get; private set; }
    /// <summary>
    /// ВНЖ
    /// </summary>
    public string TemporaryResidencePermit { get; private set; }
    /// <summary>
    /// Если true, то KYC пройден
    /// </summary>
    public bool KycStatus { get; private set; }
    /// <summary>
    /// true если юзер отправлял данные на куц
    /// </summary>
    public bool KycSended { get; private set; }
    /// <summary>
    /// Реферальный идентификатор
    /// </summary>
    public string ReferralLink { get; private set; }
    public string AvatarUrl { get; private set; }

    public bool IsReferralThresholdPayed { get; private set; }

    public ICollection<Document> Documents { get; private set; }

    public ICollection<Referral> Referrals { get; }
    public ICollection<ReferralProfit> ReferralProfits { get; }

    public ICollection<Activity> Activities { get; }

    public ICollection<Withdrawal> Withdrawals { get; }

    public UserAccount UserAccount { get; private set; }

    protected User()
    {
    }

    public User(string email, string phone, string refUrl)
    {
        phone = CleanPhoneNumber(phone);
        if (!IsValidEmail(email)) throw new Exception("Email некорректный");
        if (!IsValidPhone(phone)) throw new Exception("Телефон некорректный");
        Email = email;
        Phone = phone;
        ReferralLink = refUrl;//CreateRefUrl();
        UserAccount = new UserAccount();
        Documents = new List<Document>();
        Referrals = new List<Referral>();
        ReferralProfits = new List<ReferralProfit>();
        Activities = new List<Activity>();
        Withdrawals = new List<Withdrawal>();
    }

    public void SetKycData(string firstName, string lastName, string email, string phone, string residency, string citizenship,
        string occupation, string temporaryResidence, string avatarUrl)
    {
        if (!IsValidEmail(email)) throw new Exception("Email некорректный");
        if (!IsValidPhone(phone)) throw new Exception("Телефон некорректный");
        Email = email;
        Phone = phone;
        FirstName = firstName;
        LastName = lastName;
        Residency = residency;
        Citizenship = citizenship;
        Occupation = occupation;
        TemporaryResidencePermit = temporaryResidence;
        AvatarUrl = avatarUrl;
        KycSended = true;
    }

    /// <summary>
    /// Добавить документ КУЦ
    /// </summary>
    /// <param name="document"></param>
    public void AddDocument(Document document)
    {
        Documents.Add(document);
    }

    /// <summary>
    /// Пометить КУЦ пройденным
    /// </summary>
    public void AcceptKyc()
    {
        KycStatus = true;
    }
    /// <summary>
    /// Пометить КУЦ пройденным
    /// </summary>
    public void RejectKyc()
    {
        KycSended = false;
    }

    //private string CreateRefUrl()
    //{
    //    // Преобразование массива байтов в BigInteger
    //    BigInteger bigInt = new(Guid.NewGuid().ToByteArray());
    //    // Кодирование BigInteger в base62
    //    return EncodeToBase62(bigInt).ToString();
    //}

    //private string EncodeToBase62(BigInteger value)
    //{
    //    const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    //    string result = "";
    //    value = BigInteger.Abs(value);
    //    while (value > 0)
    //    {
    //        int remainder = (int)(value % 62);
    //        value /= 62;
    //        result = chars[remainder] + result;
    //    }
    //    return result;
    //}

    public static bool IsValidEmail(string email)
    {
        // Use a regular expression pattern to match against the email string
        // This pattern is based on the RFC 5322 specification for email addresses
        // Note that this pattern is not perfect and may not catch all invalid email addresses
        string pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
        Regex regex = new(pattern);
        return regex.IsMatch(email);
    }

    public static bool IsValidPhone(string phone)
    {
        //is for matching international phone numbers that start with a plus sign (+), followed by between 6 to 14 digits
        string pattern = @"^\+(?:[0-9]●?){6,14}[0-9]$";
        Regex regex = new(pattern);
        return regex.IsMatch(phone);
    }

    /// <summary>
    /// Очищает телефонный номер от пробелов и скобок, оставляя только плюс впереди и цифры номера
    /// </summary>
    /// <param name="um"></param>
    /// <param name="phone"></param>
    /// <returns></returns>
    public static string CleanPhoneNumber(string phone)
    {
        var digitsOnly = new Regex(@"[^\d]");
        return $"+{digitsOnly.Replace(phone, "")}";
    }

    /// <summary>
    /// Баланс только инвестированного
    /// </summary>
    /// <returns></returns>
    public decimal GetInvestedBalance()
    {
        return UserAccount.InvestedBalance;
    }

    /// <summary>
    /// Баланс только профита
    /// </summary>
    /// <returns></returns>
    public decimal GetProfitBalance()
    {
        return UserAccount.AccumulatedProfit;
    }

    public decimal GetBlockedInvestedBalance(int holdPeriodInMinutes)
    {
        var dt = DateTime.UtcNow.AddMinutes(-1 * holdPeriodInMinutes);
        return Activities.Where(x =>
        (x.ActionType == Enums.ActionType.Deposit || x.ActionType == Enums.ActionType.Reinvest)
        && x.CreatedAt >= dt).Sum(x => x.Amount);
    }

    /// <summary>
    /// Получить баланс пользователя, который он может вывести
    /// </summary>
    /// <param name="holdPeriodInMinutes"></param>
    /// <returns></returns>
    public decimal GetInvestedBalanceThatUserCanWithdraw(int holdPeriodInMinutes)
    {
        return UserAccount.InvestedBalance -
            GetBlockedInvestedBalance(holdPeriodInMinutes)
            + UserAccount.WithdrawedInvested;//добавляем это чтобы заглушить погашенные депозиты
    }

    /// <summary>
    /// Внести указанную сумму на Инвест счёт пользователя
    /// </summary>
    /// <param name="amount"></param>
    public void Deposit(decimal amount)
    {
        UserAccount.Deposit(amount);
        Activities.Add(new Activity(Enums.ActionType.Deposit, "USDT", amount));
    }

    /// <summary>
    /// Начислить профит
    /// </summary>
    /// <param name="profit"></param>
    public void ChargeProfit(decimal profit)
    {
        UserAccount.ChargeProfit(profit);
        Activities.Add(new Activity(Enums.ActionType.ChargeProfit, "USDT", profit));
    }

    /// <summary>
    /// Начислить реф вознаграждение
    /// </summary>
    /// <param name="referal">кто принёс</param>
    /// <param name="refProfit">сколько принёс</param>
    /// <param name="refDeposit">депозит реферала на момент начисления</param>
    public void AddRefPayout(Referral referal, decimal refProfit, decimal refDeposit)
    {
        UserAccount.ChargeProfit(refProfit);
        Activities.Add(new Activity(Enums.ActionType.ReferalPayout, "USDT", refProfit));
        ReferralProfits.Add(new ReferralProfit(referal, refProfit, refDeposit));
    }

    /// <summary>
    /// Начислить разовое реф вознаграждение
    /// </summary>
    /// <param name="referal">кто принёс</param>
    /// <param name="refProfit">сколько принёс</param>
    /// <param name="refDeposit">депозит реферала на момент начисления</param>
    public void AddOneTimeRefPayout(Referral referal, decimal refProfit, decimal refDeposit)
    {
        UserAccount.ChargeProfit(refProfit);
        Activities.Add(new Activity(Enums.ActionType.OneTimeReferalPayout, "USDT", refProfit));
        ReferralProfits.Add(new ReferralProfit(referal, refProfit, refDeposit));
    }

    /// <summary>
    /// Сделали разовую выплату
    /// </summary>
    public void RefTresholdPayoutDone()
    {
        IsReferralThresholdPayed = true;
    }

    /// <summary>
    /// Реинвестировать всю накопленную прибыль
    /// </summary>
    public void Reinvest()
    {
        Activities.Add(new Activity(Enums.ActionType.Reinvest, "USDT", UserAccount.AccumulatedProfit));
        UserAccount.Reinvest();
    }

    /// <summary>
    /// Добавляет юзеру заявку на вывод
    /// </summary>
    /// <param name="address"></param>
    /// <param name="amount"></param>
    /// <param name="currency"></param>
    /// <param name="method"></param>
    public void AddWithdrawal(string address, decimal amount, string currency, string method, bool fromInvested = false)
    {
        Withdrawals.Add(new Withdrawal(address, amount, currency, method));
        if (fromInvested)
        {
            UserAccount.WithdrawInvested(amount);
        }
        else
        {
            UserAccount.WithdrawProfit(amount);
        }

    }
}
