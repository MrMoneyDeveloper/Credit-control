using eBaseApp.DataAccessLayer;
using eBaseApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace eBaseApp.Helpers
{
    public class ViewHelper
    {
        public static IEnumerable<R_CC_CUT_OFF_PROCESSING> CC_CUT_OFF_PROCESSING()
        {
            using (SolarESBDbContext core = new SolarESBDbContext())
            {
                return core.R_CC_CUT_OFF_PROCESSING.ToList();
            }
        }

        public static IEnumerable<Contractor> GetContractors()
        {
            using (eServicesDbContext core = new eServicesDbContext())
            {
                return core.Contractors.ToList();
            }
        }

        public static Contractor GetContractor(Int32 Id)
        {
            using (eServicesDbContext core = new eServicesDbContext())
            {
                return core.Contractors.Find(Id);
            }
        }
        public static R_CC_CUT_OFF_PROCESSING Get_CUT_OFF_PROCESSING(Int64 Id)
        {
            using (SolarESBDbContext core = new SolarESBDbContext())
            {
                return core.R_CC_CUT_OFF_PROCESSING.Find(Id);
            }
        }

        public static JobCardBalance GetBalanceSheet(long Id)
        {
            using (eServicesDbContext core = new eServicesDbContext())
            {
                var data = core.JobCardBalances.Include(a => a.Technician).Include(a => a.Status).FirstOrDefault(a => a.AllocatedAccountId == Id);
                return (data == null) ? new JobCardBalance() : data;
            }
        }

    }
}