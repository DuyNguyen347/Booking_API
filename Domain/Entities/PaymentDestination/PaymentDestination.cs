using Domain.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.PaymentDestination
{
    public class PaymentDestination : AuditableBaseEntity<long>
    {
        [Column("des_name", TypeName = "varchar(100)")]
        public string? DesName { get; set; } = string.Empty;

        [Column("des_shortname", TypeName = "varchar(50)")]
        public string? DesShortName { get; set; } = string.Empty;

        [Column("des_parent_id", TypeName = "varchar(50)")]
        public string? DesParentId { get; set; } = string.Empty;

        [Column("des_logo", TypeName = "varchar(50)")]
        public string? DesLogo { get; set; } = string.Empty;
        
        [Column("short_index", TypeName = "int")]
        public int SortIndex { get; set; }
        
        [Column("is_active", TypeName = "bit")]
        public bool IsActive { get; set; }
    }
}
