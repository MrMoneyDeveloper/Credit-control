using eBaseApp.DataAccessLayer;
using eBaseApp.Engine.Generic.Abstract;
using eBaseApp.Engine.Generic.Concrete;
using eBaseApp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBaseApp.Controllers
{
    public class CreditControlController : Controller
    {
        private readonly eServicesDbContext _context;
        private readonly SolarESBDbContext _solarESB;
        private readonly IGenericEntityBuilder _genericEntityBuilder;
        private readonly IGenericEntityBuilder _solargenericEntityBuilder;

        public CreditControlController()
        {
            _solarESB = new SolarESBDbContext();    
            _context = new eServicesDbContext();
            _genericEntityBuilder = new GenericEntityBuilder(_context);
            _solargenericEntityBuilder = new GenericEntityBuilder(_solarESB);
        }

        public ActionResult BulkAllocate()
        {
           return View();
        }
        //[Authorize]
        [HttpPost]
        public ActionResult BulkAllocate(FormCollection form, String[] selectedAccounts)
        {
            IEnumerable<R_CC_CUT_OFF_PROCESSING> cutOffProcessing = _solargenericEntityBuilder.GetSolar<R_CC_CUT_OFF_PROCESSING>();
            List<ContreactorVM> contreactorVMs = new List<ContreactorVM>();
            List<String> sac = selectedAccounts.ToList();
            List<String> c = new List<String>();
            List<String> _aac = new List<String>();
            foreach (var k in form.AllKeys)
                if (k.Contains("contr_")) c.Add(k);

            foreach(var _c in c)
            {
                Int32 _i = Convert.ToInt32(_c.Split('_').ToList()[1]);
                Int32 _a = Convert.ToInt32(form[_c]);
                Contractor contractor = _genericEntityBuilder.GetById<Contractor>(_i);
                if (_a >= 1)
                {
                    foreach(var _b in sac.Take(_a))
                    {
                        _aac.Add(_b);
                        contreactorVMs.Add(new ContreactorVM { ContractorId = _i, AccountId = Convert.ToInt32(_b) });
                    }
                    sac = sac.Except(_aac).ToList();
                }
                contractor.Allocated = _a;
                _genericEntityBuilder.UpdateTable(contractor);
            }
            foreach(ContreactorVM data  in contreactorVMs)
            {
                var account = new AllocatedAccount
                {
                    ContractorId = data.ContractorId,
                    CUT_OFF_PROCESSING_ID = data.AccountId,
                    ActionTypeId = Convert.ToInt32(form["ActionTypeKey"].Replace(",", ""))
                };
                _genericEntityBuilder.AddToTable(account);
                
            }
            _genericEntityBuilder.Complete();
            return View();
        }

        public class ContreactorVM
        {
            public Int32 ContractorId { get; set; }
            public Int32 AccountId { get; set; }
        }
    }
}