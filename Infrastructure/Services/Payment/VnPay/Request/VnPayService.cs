using Application.Interfaces;
using Application.Interfaces.Payment;
using Application.Shared.Payment;
using Domain.Helpers;
using Microsoft.Extensions.Options;
using Shared.Configurations.PaymentConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Infrastructure.Services.Payment.VnPay.Request
{
    public class VnPayService : IVnPayService
    {
        public VnPayConfig _config { get; }
        private readonly ICurrentUserService _currentUserService;
        public SortedList<string, string> requestData
            = new SortedList<string, string>(new VnPayCompare());
        public VnPayService(IOptions<VnPayConfig> vnPayConfig, ICurrentUserService currentUserService) 
        {
            _config = vnPayConfig.Value;
            _currentUserService = currentUserService;
        }

        public void Init(DateTime createDate, string ipAddress,
            decimal amount, string currCode, string orderType, string orderInfo, string txnRef)
        {
            this.vnp_CreateDate = createDate.ToString("yyyyMMddHHmmss");
            this.vnp_Version = _config.Version;
            this.vnp_Locale = "vn";
            this.vnp_IpAddr = ipAddress;
            this.vnp_CurrCode = currCode;
            this.vnp_TmnCode = _config.TmnCode;
            this.vnp_Amount = amount;
            this.vnp_Command = "pay";
            this.vnp_OrderType = orderType;
            this.vnp_OrderInfo = orderInfo;
            this.vnp_ReturnUrl = _currentUserService.HostServerName + _config.ReturnUrl;
            this.vnp_TxnRef = txnRef;
            this.vnp_ExpireDate = createDate.AddMinutes(15).ToString("yyyyMMddHHmmss");
        }

        public string GetLink(string urlHostName)
        {
            MakeRequestData();
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in requestData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string result = _config.PaymentUrl + "?" + data.ToString();
            var secureHash = HashHelper.HmacSHA512(_config.HashSecret, data.ToString().Remove(data.Length - 1, 1));
            return result += "vnp_SecureHash=" + secureHash;
        }

        public void MakeRequestData()
        {
            if (vnp_Amount != null)
                requestData.Add("vnp_Amount", vnp_Amount.ToString() ?? string.Empty);
            if (vnp_Command != null)
                requestData.Add("vnp_Command", vnp_Command);
            if (vnp_CreateDate != null)
                requestData.Add("vnp_CreateDate", vnp_CreateDate);
            if (vnp_CurrCode != null)
                requestData.Add("vnp_CurrCode", vnp_CurrCode);
            if (vnp_BankCode != null)
                requestData.Add("vnp_BankCode", vnp_BankCode);
            if (vnp_IpAddr != null)
                requestData.Add("vnp_IpAddr", vnp_IpAddr);
            if (vnp_Locale != null)
                requestData.Add("vnp_Locale", vnp_Locale);
            if (vnp_OrderInfo != null)
                requestData.Add("vnp_OrderInfo", vnp_OrderInfo);
            if (vnp_OrderType != null)
                requestData.Add("vnp_OrderType", vnp_OrderType);
            if (vnp_ReturnUrl != null)
                requestData.Add("vnp_ReturnUrl", vnp_ReturnUrl);
            if (vnp_TmnCode != null)
                requestData.Add("vnp_TmnCode", vnp_TmnCode);
            if (vnp_ExpireDate != null)
                requestData.Add("vnp_ExpireDate", vnp_ExpireDate);
            if (vnp_TxnRef != null)
                requestData.Add("vnp_TxnRef", vnp_TxnRef);
            if (vnp_Version != null)
                requestData.Add("vnp_Version", vnp_Version);
        }
        public decimal? vnp_Amount { get; set; }
        public string? vnp_Command { get; set; }
        public string? vnp_CreateDate { get; set; }
        public string? vnp_CurrCode { get; set; }
        public string? vnp_BankCode { get; set; }
        public string? vnp_IpAddr { get; set; }
        public string? vnp_Locale { get; set; }
        public string? vnp_OrderInfo { get; set; }
        public string? vnp_OrderType { get; set; }
        public string? vnp_ReturnUrl { get; set; }
        public string? vnp_TmnCode { get; set; }
        public string? vnp_ExpireDate { get; set; }
        public string? vnp_TxnRef { get; set; }
        public string? vnp_Version { get; set; }
        public string? vnp_SecureHash { get; set; }
    }
}
