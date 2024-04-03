using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBaseApp.Helpers
{
    public class AppSettingKeys
    {
        //ad login keys
        //ActiveDirectory
        public const string ActiveDirectoryActive = "active_directory_active";
        public const string activeDirectoryDomain = "active_directory_domain";
        public const string ADUserCreds = "ad_admin_user_creds";

        public const string EservicesApplication = "eservices_applicationID";
        public const string EservicesEmailAccount = "eservices_email_account";

        public const string EservicesDefaultEmailTemplate = "eservices_default_email_template";
    }
}