using eBaseApp.DataAccessLayer;
using eBaseApp.Engine.Generic.Abstract;
using eBaseApp.Engine.Generic.Concrete;
using eBaseApp.Helpers.Abstract;
using eBaseApp.Keys;
using eBaseApp.Models;
using eBaseApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBaseApp.Helpers
{
    public class ContractorHelper : IContractorHelper
    {
        private readonly eServicesDbContext _context;
        private readonly SolarESBDbContext _solarcontext;
        private readonly IGenericEntityBuilder _genericEntityBuilder;
        private readonly IGenericEntityBuilder _solargenericEntityBuilder;
        public ContractorHelper(eServicesDbContext context) 
        {
            _context = context;
            _solarcontext = new SolarESBDbContext();
            _genericEntityBuilder = new GenericEntityBuilder(_context);
            _solargenericEntityBuilder = new GenericEntityBuilder(_solarcontext);
        }
        public IEnumerable<ContractorJobSheetViewModel> GetContractorJobSheets()
        {
            IEnumerable<R_CC_CUT_OFF_PROCESSING> CUTOFF = CUTOFFS();
            IEnumerable<AllocatedAccount> allocatedAccounts = ALLOCATED();

            foreach(AllocatedAccount allocated in  allocatedAccounts)
            {
                Int64 cutoffId = allocated.CUT_OFF_PROCESSING_ID;
                allocated.R_CC_CUT_OFF_PROCESSING = GETCUTOFF(CUTOFF, cutoffId);
                allocated.Contractor = GETCONTRACTOR(allocated.ContractorId);
                allocated.ActionType = GETACTION(allocated.ActionTypeId);
            }

            var grouped = allocatedAccounts.GroupBy(a => new
            {
                a.R_CC_CUT_OFF_PROCESSING.Municipality,
                CreatedDateTime = Convert.ToDateTime(a.CreatedDateTime).Date,
                a.ActionType,
                a.Contractor
            }).Select(g => new
            {
                GroupKey = g.Key,
                Allocated = g.Count(),
                Records = g.ToList(),
            });
            return GETVIEWMODELASLIST(grouped);   
        }
        public IEnumerable<ContractorJobSheetViewModel> GetJobSheetAccounts()
        {
            IEnumerable<R_CC_CUT_OFF_PROCESSING> CUTOFF = CUTOFFS();
            IEnumerable<AllocatedAccount> allocatedAccounts = ALLOCATED();

            foreach (AllocatedAccount allocated in allocatedAccounts)
            {
                Int64 cutoffId = allocated.CUT_OFF_PROCESSING_ID;
                allocated.R_CC_CUT_OFF_PROCESSING = GETCUTOFF(CUTOFF, cutoffId);
                allocated.Contractor = GETCONTRACTOR(allocated.ContractorId);
                allocated.ActionType = GETACTION(allocated.ActionTypeId);
            }

            var grouped = allocatedAccounts.GroupBy(a => new
            {
                a.R_CC_CUT_OFF_PROCESSING.Municipality,
                CreatedDateTime = Convert.ToDateTime(a.CreatedDateTime).Date,
                a.ActionType,
                a.Contractor
            }).Select(g => new
            {
                GroupKey = g.Key,
                Allocated = g.Count(),
                Records = g.ToList(),
            });
            return GETVIEWMODELASLIST2(grouped);
        }
        public IEnumerable<R_CC_CUT_OFF_PROCESSING> CUTOFFS()
        {
            return _solargenericEntityBuilder.GetSolar<R_CC_CUT_OFF_PROCESSING>();
        }
        public IEnumerable<AllocatedAccount> ALLOCATED()
        {
            return _genericEntityBuilder.GetActionsTypes<AllocatedAccount>();
        }
        public R_CC_CUT_OFF_PROCESSING GETCUTOFF(IEnumerable<R_CC_CUT_OFF_PROCESSING> Data, Int64 iD)
        {
            return Data.FirstOrDefault(x => x.ID == iD);
        }
        public Contractor GETCONTRACTOR(Int32 iD)
        {
            return _genericEntityBuilder.GetTypeByKey<Contractor>(a => a.Id == iD);
        }
        public ActionType GETACTION(Int32 iD)
        {
            return _genericEntityBuilder.GetTypeByKey<ActionType>(a => a.Id == iD);
        }
        public Int32 CHECKUNASSIGNED(IEnumerable<AllocatedAccount> accounts)
        {
            return accounts.Where(c => !GETACCOUNTIDS().Contains(c.Id)).Count();
        }
        public Status GETSTATUS(String Key)
        {
            return _genericEntityBuilder.GetTypeByKey<Status>(a => a.Key == Key);
        }
        public IEnumerable<ContractorJobSheetViewModel> GETVIEWMODELASLIST(IEnumerable<dynamic> grouped)
        {
            var vm = new List<ContractorJobSheetViewModel>();
            Int32 ind = 1;
            foreach (var g in grouped)
            {
                vm.Add(new ContractorJobSheetViewModel
                {
                    Allocated = g.Allocated,
                    Municipality = g.GroupKey.Municipality,
                    Contractor = g.GroupKey.Contractor,
                    ActionType = g.GroupKey.ActionType,
                    Unassined = CHECKUNASSIGNED(g.Records),
                    AllocationDate = g.GroupKey.CreatedDateTime.ToString().Substring(0, 10),
                    AllocatedAccounts = FILTERACCOUNTS(g.Records),
                    GroupId = String.Format("bs-example-modal-xl-{0}", ind),
                });
                ind ++; 
            }
            return vm;
        }

        public IEnumerable<ContractorJobSheetViewModel> GETVIEWMODELASLIST2(IEnumerable<dynamic> grouped)
        {
            var vm = new List<ContractorJobSheetViewModel>();
            Int32 ind = 1;
            foreach (var g in grouped)
            {
                vm.Add(new ContractorJobSheetViewModel
                {
                    Allocated = g.Allocated,
                    Municipality = g.GroupKey.Municipality,
                    Contractor = g.GroupKey.Contractor,
                    ActionType = g.GroupKey.ActionType,
                    Unassined = CHECKUNASSIGNED(g.Records),
                    AllocationDate = g.GroupKey.CreatedDateTime.ToString().Substring(0, 10),
                    AllocatedAccounts = g.Records,
                    GroupId = String.Format("bs-example-modal-xl-{0}", ind)
                });
                ind++;
            }
            return vm;
        }
        public IEnumerable<AllocatedAccount> FILTERACCOUNTS(IEnumerable<AllocatedAccount> records)
        {
            return records.Where(c => !GETACCOUNTIDS().Contains(c.Id));
        }
        public List <Int32> GETACCOUNTIDS()
        {
            return _genericEntityBuilder.GetActionsTypes<RoundRobinQueue>().Select(a => a.AllocatedAccountId).ToList();
        }
        public RoundRobinQueue AddTechnicianJobQueue(Int32 AccountId, Int32 TechnicianId)
        {
            return new RoundRobinQueue
            {
                AllocatedAccountId = AccountId,
                AssignedToUserId = TechnicianId,
                CurrentTaskDateTime = DateTime.Now,
                ResponsibilityTypeId = GetResponsibilityType(AccountId).Id,
                StatusId  = GETSTATUS(StatusKeys.Submitted).Id
            };
        }
        public ResponsibilityType GetResponsibilityType(Int32 AccountId)
        {
            string responsibilityType;
            AllocatedAccount account = _genericEntityBuilder.GetById<AllocatedAccount>(AccountId);
            ActionType actionType = _genericEntityBuilder.GetById<ActionType>(account.ActionTypeId);
            if (actionType.Key == ActionTypeKeys.PreTerminationNote)
                responsibilityType = ResponsibilityTypeKeys.PreTerminationNote;
            else if (actionType.Key == ActionTypeKeys.CCECut1)
                responsibilityType = ResponsibilityTypeKeys.CCECut1;
            else if (actionType.Key == ActionTypeKeys.CCReconCut1)
                responsibilityType = ResponsibilityTypeKeys.CCReconCut1;
            else responsibilityType = string.Empty;
            return _genericEntityBuilder.GetTypeByKey<ResponsibilityType>(r=>r.Key == responsibilityType);
        }
        public void AllocateAccounts(String[] selectedAccounts, Int32 TechnicianId)
        {
            foreach (String Id in selectedAccounts)
            {
                RoundRobinQueue roundRobinQueue;
                roundRobinQueue = AddTechnicianJobQueue(Convert.ToInt32(Id), TechnicianId);
                _genericEntityBuilder.AddToTable(roundRobinQueue);
                _genericEntityBuilder.Complete();

                _ = GenerateJobCard(roundRobinQueue, TechnicianId);
            }
        }

        public JobCardBalance GenerateJobCard(RoundRobinQueue roundRobinQueue, int technicianId)
        {
            JobCardBalance jobCardBalance;
            jobCardBalance = new JobCardBalance
            {
                AllocatedAccountId = roundRobinQueue.AllocatedAccountId,
                RoundRobinQueueId = roundRobinQueue.Id,
                TechnicianId = technicianId,
                DateIssued = DateTime.Now,
                JobCardCode = GenerateJobCardCode(roundRobinQueue)
            };
            _genericEntityBuilder.AddToTable(jobCardBalance);
            _genericEntityBuilder.Complete();
            return jobCardBalance;
        }

        public string GenerateJobCardCode(RoundRobinQueue roundRobinQueue)
        {
            string jobCardCode;
            AllocatedAccount account;
            ActionType actionType;
            Contractor contractor;

            account = _genericEntityBuilder.GetById<AllocatedAccount>(roundRobinQueue.AllocatedAccountId);
            if (account == null) throw new Exception($"invalid account {nameof(account)} => {roundRobinQueue.AllocatedAccountId}");

            actionType = _genericEntityBuilder.GetById<ActionType>(account.ActionTypeId);
            if (actionType == null) throw new Exception($"invalid account {nameof(actionType)} => {account.ActionTypeId}");

            contractor = _genericEntityBuilder.GetById<Contractor>(account.ContractorId);
            if (contractor == null) throw new Exception($"invalid account {nameof(contractor)} => {account.ContractorId}");

            jobCardCode = $"{DateTime.Now.ToString("yyyyMMdd")}-{contractor.Key}-DCC-{actionType.Description}";

            return jobCardCode;
        }

       
    }
}