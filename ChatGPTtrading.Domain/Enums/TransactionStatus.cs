namespace ChatGPTtrading.Domain.Enums;

public enum TransactionStatus
{
    /// <summary>
    /// Ожидает 
    /// </summary>
    Waiting,
    /// <summary>
    /// Подтверждена
    /// </summary>
    Confirmed,
    /// <summary>
    ///  Отклонена
    /// </summary>
    Rejected,
    /// <summary>
    /// Ошибка
    /// </summary>
    Error,
    /// <summary>
    /// Завершена
    /// </summary>
    Completed
       
}
