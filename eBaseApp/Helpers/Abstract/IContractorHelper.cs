using eBaseApp.DataAccessLayer;
using eBaseApp.Models;
using eBaseApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBaseApp.Helpers.Abstract
{
    public interface IContractorHelper
    {
        IEnumerable<ContractorJobSheetViewModel> GetContractorJobSheets();
        IEnumerable<R_CC_CUT_OFF_PROCESSING> CUTOFFS();
        IEnumerable<AllocatedAccount> ALLOCATED();
        R_CC_CUT_OFF_PROCESSING GETCUTOFF(IEnumerable<R_CC_CUT_OFF_PROCESSING> Data, Int64 iD);
        Contractor GETCONTRACTOR(Int32 iD);
        ActionType GETACTION(Int32 iD);
        Int32 CHECKUNASSIGNED(IEnumerable<AllocatedAccount> accounts);
        Status GETSTATUS(String Key);
        IEnumerable<ContractorJobSheetViewModel> GETVIEWMODELASLIST(IEnumerable<dynamic> grouped);
        IEnumerable<AllocatedAccount> FILTERACCOUNTS(IEnumerable<AllocatedAccount> records);
        List<Int32> GETACCOUNTIDS();
        RoundRobinQueue AddTechnicianJobQueue(Int32 AccountId, Int32 TechnicianId);
        ResponsibilityType GetResponsibilityType(Int32 AccountId);
        void AllocateAccounts(String[] selectedAccounts, Int32 TechnicianId);
        JobCardBalance GenerateJobCard(RoundRobinQueue roundRobinQueue, int technicianId);
        string GenerateJobCardCode(RoundRobinQueue roundRobinQueue);
        IEnumerable<ContractorJobSheetViewModel> GetJobSheetAccounts();

    }
}
