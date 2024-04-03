using eBaseApp.DataAccessLayer;
using eBaseApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBaseApp.Models.Cesar
{
    public class CesarSMS
    {
        private eServicesDbContext db = new eServicesDbContext();
        private CesarDbContext core = new CesarDbContext();

        public CesarSMS()
        {
            IdentityManager = new IdentityManager(db);
        }

        public IdentityManager IdentityManager { get; set; }

        public void GenerateSMS(string recipientMobileNumber, string textMessage,
        string referenceId, int statusId, string recipientName = "")
        {
            try
            {
                if (recipientMobileNumber == null)
                    return;

                var smsApplication = db.AppSettings.FirstOrDefault(a => a.Key == AppSettingKeys.EservicesApplication);
                var smsAccount = db.AppSettings.FirstOrDefault(a => a.Key == AppSettingKeys.EservicesEmailAccount);

                if (smsApplication == null) throw new Exception("Invalid Application setting");
                if (smsAccount == null) throw new Exception("Invalid Application setting");

                var sms = new SmsQueueItem
                {
                    ApplicationId = 1,
                    QueueDateTime = DateTime.Now,
                    SmsAccountId = 1,
                    MobileNumber = recipientMobileNumber,
                    TextMessage = textMessage,
                    FailureCount = 0,
                    ReferenceId = referenceId,
                    StatusId = statusId
                };

                core.SmsQueue.Add(sms);
                core.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}