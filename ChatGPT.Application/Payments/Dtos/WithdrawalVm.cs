using ChatGPTtrading.Domain.Enums;

namespace ChatGPT.Application.Payments.Dtos
{
    public class WithdrawalVm
    {
        ///<summary>
        /// Дата
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// Способ вывода
        /// </summary>
        public string PaymentMethod { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public TransactionStatus Status { get; set; }
    }
}
