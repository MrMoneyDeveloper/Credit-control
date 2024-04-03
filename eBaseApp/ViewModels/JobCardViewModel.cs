using eBaseApp.DataAccessLayer;
using eBaseApp.Models;
namespace eBaseApp.ViewModels
{
    public class JobCardViewModel
    {
        public R_CC_CUT_OFF_PROCESSING CUTOFF { get; set; }
        public AllocatedAccount AllocatedAccount { get; set; }
        public Contractor Contractor { get; set; }
        public Technician Technician { get; set; }
        public ActionType ActionType { get; set; }
        public JobCardBalance JobCardBalance { get; set; }
    }
}