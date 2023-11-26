namespace Application.Features.Payment.Command
{
    public class ProcessVnPayReturnResponse
    {
        public string? PaymentId { get; set; }
        /// <summary>
        /// 00: Success
        /// 99: Unknown
        /// 10: Error
        /// </summary>
        public string? PaymentStatus { get; set; }
        public string? PaymentMessage { get; set; }
        /// <summary>
        /// Format: yyyyMMddHHmmss
        /// </summary>
        public string? PaymentDate { get; set; }
        public decimal? Amount { get; set; }
        public string? Signature { get; set; }
        public string? QRCode { get; set; }
    }
}
