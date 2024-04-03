using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eBaseApp.Models.Audits
{
    public class StatusAudit : BaseTypeAudit
    {
        [Column(Order = 20)]
        [Display(Name = "StatusType")]
        public int StatusTypeId { get; set; }
        [ForeignKey("StatusTypeId")]
        public StatusType StatusType { get; set; }
    }
}