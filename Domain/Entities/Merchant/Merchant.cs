using Domain.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Merchant
{
    [Table("merchant")]
    public class Merchant : AuditableBaseEntity<long>
    {
        [Required]
        [Column("merchant_name", TypeName = "varchar(20)")]
        public string? MerchantName { get; set; } = string.Empty;
        
        [Column("merchant_weblink", TypeName = "varchar(max)")]
        public string? MerchantWebLink { get; set; } = string.Empty;

        [Column("merchant_ipn_url", TypeName = "varchar(max)")]
        public string? MerchantIpnUrl { get; set; } = string.Empty;

        [Column("merchant_return_url", TypeName = "varchar(max)")]
        public string? MerchantReturnUrl { get; set; } = string.Empty;
        
        [Column("secret_key", TypeName = "varchar(100)")]
        public string? SecretKey { get; set; } = string.Empty;

        [Column("isActive", TypeName = "bit")]
        public bool IsActive { get; set; }
    }
}
