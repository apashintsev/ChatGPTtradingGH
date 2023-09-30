namespace ChatGPTtrading.Domain.Config;

public class TelegramSettings
{
    public string BotToken { get; set; }
    public string Url { get; set; }
    public long[] Admins { get; set; }
    public long KycChat { get; set; }
    public long WithdrawChat { get; set; }
}
