using eBaseApp.DataAccessLayer;
using eBaseApp.Engine.Generic.Abstract;
using eBaseApp.Engine.Generic.Concrete;
using eBaseApp.Helpers;
using eBaseApp.Helpers.Abstract;
using eBaseApp.Models;
using eBaseApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBaseApp.Controllers
{
    public class ContractorController : Controller
    {
        private readonly eServicesDbContext _context;
        private readonly IGenericEntityBuilder _genericEntityBuilder;
        private readonly IContractorHelper _contractorHelper;
        private readonly BaseHelper _baseHelper;
        public ContractorController()
        {
            _context = new eServicesDbContext();
            _baseHelper = new BaseHelper();
            _baseHelper.Initialise(_context);
            _contractorHelper = new ContractorHelper(_context);
            _genericEntityBuilder = new GenericEntityBuilder(_context);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]


        public ActionResult Add(Contractor contractor)
        {
            try
            {
                _genericEntityBuilder.AddToTable(contractor);
                _genericEntityBuilder.Complete();   
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult TechnicianBulkAssign()
        {
            return View(_contractorHelper.GetContractorJobSheets());
        }
        [HttpPost]
        public ActionResult TechnicianBulkAssign(String[] selectedAccounts, Int32 TechnicianId)
        {
            _contractorHelper.AllocateAccounts(selectedAccounts, TechnicianId);
            return RedirectToAction("TechnicianBulkAssign");
        }
    }
}
