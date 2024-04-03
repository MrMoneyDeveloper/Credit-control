using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBaseApp.Models
{
    public class JobCardBalance : BaseModel
    {
        [Column(Order = 10)]
        public DateTime DateIssued { get; set; }

        [Column(Order = 11)]
        public string JobCardCode { get; set; }

        [Column(Order = 12)]
        public Nullable<int> StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }

        [Column(Order = 13)]
        public int TechnicianId { get; set; }
        [ForeignKey("TechnicianId")]
        public Technician Technician { get; set; }

        [Column(Order = 14)]
        public int AllocatedAccountId { get; set; }
        [ForeignKey("AllocatedAccountId")]
        public AllocatedAccount AllocatedAccount { get; set; }

        [Column(Order = 15)]
        public int RoundRobinQueueId { get; set; }
        [ForeignKey("RoundRobinQueueId")]
        public RoundRobinQueue RoundRobinQueue { get; set; }
    }
}