using eBaseApp.DataAccessLayer;
using eBaseApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eBaseApp.Controllers
{
    public class HomeController : Controller
    {
        private eServicesDbContext _context;
        public HomeController() 
        {
            _context = new eServicesDbContext();    
        }
        public ActionResult Index()
        {
            DatabaseHelper databaseHelper = new DatabaseHelper(new DataAccessLayer.eServicesDbContext());
            databaseHelper.ActionTypes();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact(string[] selectedAccounts = null)
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetResults()
        {
            IEnumerable<dynamic> list = _context.ActionTypes.ToList();
            return Json(list, JsonRequestBehavior.AllowGet);  
        }

        public ActionResult Construction() 
        { 
            return View();
        }
    }
}