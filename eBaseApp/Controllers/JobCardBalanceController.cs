using eBaseApp.DataAccessLayer;
using eBaseApp.Engine.Generic.Abstract;
using eBaseApp.Engine.Generic.Concrete;
using System.Web.Mvc;
using System.Data.Entity;
using eBaseApp.Helpers;
using eBaseApp.Helpers.Abstract;
using eBaseApp.ViewModels;
using eBaseApp.Models;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace eBaseApp.Controllers
{
    public class JobCardBalanceController : Controller
    {
        private readonly eServicesDbContext _context;
        private readonly SolarESBDbContext _solarESB;
        private readonly IContractorHelper _contractorHelper;
        private readonly IGenericEntityBuilder _genericEntityBuilder;
        private readonly IGenericEntityBuilder _solargenericEntityBuilder;

        public JobCardBalanceController()
        {
            _solarESB = new SolarESBDbContext();
            _context = new eServicesDbContext();
            _genericEntityBuilder = new GenericEntityBuilder(_context);
            _solargenericEntityBuilder = new GenericEntityBuilder(_solarESB);
            _contractorHelper = new ContractorHelper(_context);

        }
        public ActionResult Index()
        {   
            return View(_contractorHelper.GetJobSheetAccounts());
        }

        public async Task<ActionResult> Details(int Id, JobCardViewModel viewModel = null)
        {
            await SendCustomerWarningNotices.SendCustomerSMS("277897437071", "a28dd97c-1ffb-4fcf-99f1-0b557ed381d");
            viewModel = new JobCardViewModel();
            viewModel.AllocatedAccount = _genericEntityBuilder.GetById<AllocatedAccount>(Id);
            viewModel.CUTOFF = _solargenericEntityBuilder.GetSolarById<R_CC_CUT_OFF_PROCESSING>(viewModel.AllocatedAccount.CUT_OFF_PROCESSING_ID);
            viewModel.ActionType = _genericEntityBuilder.GetById<ActionType>(viewModel.AllocatedAccount.ActionTypeId);
            viewModel.JobCardBalance = _genericEntityBuilder.GetTypeByKey<JobCardBalance>(a => a.AllocatedAccountId.Equals(Id));
            viewModel.Technician = viewModel.JobCardBalance == null ? new Technician() : _genericEntityBuilder.GetById<Technician>(viewModel.JobCardBalance.TechnicianId);
            viewModel.Contractor = _genericEntityBuilder.GetById<Contractor>(viewModel.AllocatedAccount.ContractorId);
            if (viewModel.JobCardBalance == null)
                viewModel.JobCardBalance = new JobCardBalance();
            return View(viewModel);
        }

        public async Task<object> SendWhasAppGrapeVine()
        {
            await SendCustomerWarningNotices.SendCustomerSMS("277897437071", "a28dd97c-1ffb-4fcf-99f1-0b557ed381d");
            await SendCustomerWarningNotices.SendWhatsAppNotification("277897437071", "a28dd97c-1ffb-4fcf-99f1-0b557ed381d");
            return Json(true, JsonRequestBehavior.AllowGet);
        }


    }
}