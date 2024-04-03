using eBaseApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBaseApp.ViewModels
{
    public class ContractorJobSheetViewModel
    {
        public String Municipality { get; set; }
        public ActionType ActionType { get; set;}
        public Contractor Contractor { get; set;}
        public String AllocationDate { get; set; }
        public Int32 Allocated { get; set; }
        public Int32 Unassined { get; set; }
        public String GroupId { get; set; }
        public IEnumerable<AllocatedAccount> AllocatedAccounts { get; set; }

        public int TechnicianId { get; set; }
        public JobCardBalance jobCardBalance { get; set; }
    }
}