using eBaseApp.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBaseApp.Helpers
{
    public static class DropdownHelper
    {
        public static IEnumerable<SelectListItem> ActionTypes()
        {
            using (eServicesDbContext core = new eServicesDbContext())
            {
                List<SelectListItem> settingsDataElement = new List<SelectListItem>();
                var settingsData = core.ActionTypes.Select(x => new SelectListItem
                {
                    Text = x.Name.ToString(),
                    Value = x.Id.ToString()
                }).ToList();
                settingsDataElement.AddRange(settingsData);
                return settingsDataElement;
            }
        }
        public static IEnumerable<SelectListItem> GETTECHNICIANS(Int32 Id)
        {
            using (eServicesDbContext core = new eServicesDbContext())
            {
                List<SelectListItem> settingsDataElement = new List<SelectListItem>();
                var settingsData = core.Technicians.Where(c => c.ContractorId == Id).Select(x => new SelectListItem
                {
                    Text = x.Username,
                    Value = x.Id.ToString()
                }).ToList();
                settingsDataElement.AddRange(settingsData);
                return settingsDataElement;
            }
        }
    }
}