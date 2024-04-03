using eBaseApp.DataAccessLayer;
using eBaseApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System;
using Microsoft.AspNet.Identity;
using static System.Net.WebRequestMethods;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Web.Razor.Text;
using System.Web.Routing;
using System.Web.Services.Description;

namespace eBaseApp.Helpers
{
    public class EkurhuleniActiveDirectory
    {
        private eServicesDbContext _context;

        public EkurhuleniActiveDirectory()
        {
            _context = new eServicesDbContext();
        }

        public EkuActiveDirectory GetEkuDetails(string UserName)
        {
            EkuActiveDirectory cr = new EkuActiveDirectory();
            try
            {
                var activeDirectoryDomain = _context.AppSettings.Where(x => x.Key == AppSettingKeys.activeDirectoryDomain).FirstOrDefault().Value;
                var aduser = _context.AppSettings.Where(x => x.Key == AppSettingKeys.ADUserCreds).FirstOrDefault();

                UserName = UserName.Trim();
                using (var context = new PrincipalContext(ContextType.Domain, activeDirectoryDomain, aduser.Description, aduser.Value))
                {
                    using (var userPrincipal = new UserPrincipal(context))
                    {
                        userPrincipal.SamAccountName = UserName;
                        using (var principalSearcher = new PrincipalSearcher())
                        {
                            cr.UserName = UserName;
                            cr.IsActiveDirectoryUser = true;
                            principalSearcher.QueryFilter = userPrincipal;
                            Principal searchResult = principalSearcher.FindOne();
                            if (searchResult != null)
                            {
                                DirectoryEntry directoryEntry = (DirectoryEntry)searchResult.GetUnderlyingObject();

                                if (directoryEntry.Properties.Contains("mail"))
                                    cr.EmailAddress = directoryEntry.Properties["mail"].Value.ToString();

                                if (directoryEntry.Properties.Contains("sn"))
                                    cr.LastName = directoryEntry.Properties["sn"].Value.ToString();

                                if (directoryEntry.Properties.Contains("givenname"))
                                    cr.FirstName = directoryEntry.Properties["givenname"].Value.ToString();

                                if (directoryEntry.Properties.Contains("cn"))
                                    cr.UserName = directoryEntry.Properties["cn"].Value.ToString();

                                if (directoryEntry.Properties.Contains("telephoneNumber"))
                                    cr.MobileNumber = directoryEntry.Properties["telephoneNumber"].Value.ToString();

                                if (directoryEntry.Properties.Contains("mobile"))
                                    cr.MobileNumber = directoryEntry.Properties["mobile"].Value.ToString();

                                if (directoryEntry.Properties.Contains("l"))
                                    cr.CCC = directoryEntry.Properties["l"].Value.ToString();

                                if (directoryEntry.Properties.Contains("department"))
                                    cr.Department = directoryEntry.Properties["department"].Value.ToString();

                                if (directoryEntry.Properties.Contains("co"))
                                    cr.Country = directoryEntry.Properties["co"].Value.ToString();

                                if (directoryEntry.Properties.Contains("company"))
                                    cr.Company = directoryEntry.Properties["company"].Value.ToString();
                            }
                        }
                    }

                    return cr;
                }



            }
            catch
            {
                //TODO: Inject logging and log exception
                return cr;
            }
        }

    }
}

//ldap://10.31.3.52:389/CN=Albfinenq,CN=Albfinenq,OU=Finance,OU=EMM%20Users,DC=ekurhuleni,DC=gov,DC=za
//objectClass: top
//objectClass: person
//objectClass: organizationalPerson
//objectClass: user
//cn: Albfinenq
//c: ZA
//l: Alberton
//title: Area Manager
//description: Area Manager
//telephoneNumber: 011 999 2214 / 2142 / 2140
//givenName: Albfinenq
//distinguishedName: CN = Albfinenq,OU = Finance,OU = EMM Users, DC = ekurhuleni, DC = gov, DC = za
//instanceType: [Writable]
//whenCreated: 2010 - 01 - 29 10:10:23 AM
//whenChanged: 2023 - 07 - 17 08:51:09 AM
//displayName: Albfinenq
//uSNCreated: 83628
//memberOf: CN = TESTGRPWEB,OU = WEBFILTERING,OU = EMM Groups, DC = ekurhuleni, DC = gov, DC = za
//memberOf: CN = VPN_Full_ Access, OU = VPNGRP, OU = EMM Groups, DC = ekurhuleni, DC = gov, DC = za
//memberOf: CN = U - S - ServiceAccounts,OU = ServiceAccounts,DC = ekurhuleni,DC = gov,DC = za
//memberOf: CN = allsubscribers658905ed,OU = DistributionGroups,OU = EMM Users, DC = ekurhuleni, DC = gov, DC = za
//uSNChanged: 761388506
//co: South Africa
//department: Finance
//company: City of Ekurhuleni
//proxyAddresses: x500:/ o = ExchangeLabs / ou = Exchange Administrative Group(FYDIBOHF23SPDLT)/ cn = Recipients / cn = 66fe74fad7874b8fbc86ce8a4837b1b1 - Albfinenq
//proxyAddresses: x500:/ o = EKURHULENI / ou = Exchange Administrative Group(FYDIBOHF23SPDLT)/ cn = Recipients / cn = Albfinenq
//proxyAddresses: X400: C = US; A = ; P = EKURHULENI; O = Exchange; S = Albfinenq;
//proxyAddresses: X500:/ o = ExchangeLabs / ou = Exchange Administrative Group(FYDIBOHF23SPDLT)/ cn = Recipients / cn = e307c9004f604519bac067b141e4c34c - Albfinenq
//proxyAddresses: smtp: Albfinenq @EMMOnline.mail.onmicrosoft.com
//proxyAddresses: smtp: Albfinenq @ekurhuleni.com
//proxyAddresses: SMTP: Albfinenq @ekurhuleni.gov.za
//publicDelegates: CN = Lerato E.Maffa,OU = Finance,OU = EMM Users, DC = ekurhuleni, DC = gov, DC = za
//publicDelegates: CN = Candice Freeman, OU = Interns, OU = EMM Users, DC = ekurhuleni, DC = gov, DC = za
//publicDelegates: CN = Sarah Mapena, OU = EMM Users, DC = ekurhuleni, DC = gov, DC = za
//publicDelegates: CN = Benny Moleko(Alberton),OU = Information Communications & Technology,OU = EMM Users, DC = ekurhuleni, DC = gov, DC = za
//publicDelegates: CN = Beverley Hansrod - Le Grange, OU = Income, OU = Finance, OU = EMM Users, DC = ekurhuleni, DC = gov, DC = za
//publicDelegates: CN = Cornelius  Swanepoel, OU = Income, OU = Finance, OU = EMM Users, DC = ekurhuleni, DC = gov, DC = za
//publicDelegates: CN = Louise Janse Van Rensburg (Alberton),OU = Income,OU = Finance,OU = EMM Users, DC = ekurhuleni, DC = gov, DC = za
//streetAddress: Alberton
//Civic
//publicDelegatesBL: CN = Sibongele  Lubisi, OU = EMM Users, DC = ekurhuleni, DC = gov, DC = za
//targetAddress: SMTP: Albfinenq @EMMOnline.mail.onmicrosoft.com
//mailNickname: Albfinenq
//wWWHomePage: www.ekurhuleni.gov.za
//protocolSettings: HTTP§1§1§§§§§§
//protocolSettings: OWA§1
//name: Albfinenq
//userAccountControl: [NormalAccount]
//badPwdCount: 0
//codePage: 0
//countryCode: 710
//badPasswordTime: 2023 - 07 - 27 10:05:34 AM
//lastLogoff: unspecified
//lastLogon: 2023 - 07 - 27 08:29:02 AM
//pwdLastSet: 2023 - 06 - 27 11:01:34 AM
//primaryGroupID: 513
//accountExpires: never
//logonCount: 9499
//sAMAccountName: Albfinenq
//sAMAccountType: samUserAccount
//showInAddressBook: CN = Default Global Address List, CN = All Global Address Lists, CN = Address Lists Container, CN = EKURHULENI, CN = Microsoft Exchange, CN = Services, CN = Configuration, DC = Municipal, DC = com
//showInAddressBook: CN = Alberton,CN = SOUTH,CN = All Address Lists, CN = Address Lists Container, CN = EKURHULENI, CN = Microsoft Exchange, CN = Services, CN = Configuration, DC = Municipal, DC = com
//showInAddressBook: CN = All Users, CN = All Address Lists, CN = Address Lists Container, CN = EKURHULENI, CN = Microsoft Exchange, CN = Services, CN = Configuration, DC = Municipal, DC = com
//legacyExchangeDN: / o = EKURHULENI / ou = External(FYDIBOHF25SPDLT) / cn = Recipients / cn = 846fd19be2ec4e63be3a80fb261f3451
//userPrincipalName: Albfinenq @ekurhuleni.gov.za
//lockoutTime: unspecified
//objectCategory: CN = Person,CN = Schema,CN = Configuration,DC = Municipal,DC = com
//dSCorePropagationData: 2023 - 02 - 15 09:07:15 AM
//dSCorePropagationData: 2022 - 05 - 18 04:30:22 PM
//dSCorePropagationData: 2021 - 09 - 28 01:19:36 PM
//dSCorePropagationData: 2021 - 08 - 02 12:19:38 PM
//dSCorePropagationData: 1601 - 07 - 15 12:36:49 AM
//msDS-User-Account-Control-Computed: [ ]
//lastLogonTimestamp: 2023 - 07 - 17 08:50:55 AM
//textEncodedORAddress: X400: C = US; A = ; P = EKURHULENI; O = Exchange; S = Albfinenq;
//mail: Albfinenq @ekurhuleni.gov.za
//ciscoEcsbuUnityAttributes: Encryption: 0
//ciscoEcsbuListInUMDirectory: TRUE
//ciscoEcsbuObjectType: 1
//msExchUserAccountControl: 0
//ciscoEcsbuDtmfId: 992892
//ciscoEcsbuUndeletable: FALSE
//ciscoEcsbuTransferId: 992892
//msExchPoliciesIncluded: 856dd71b - 51c4 - 4e3f - bc24 - aa66e5665f31
//msExchPoliciesIncluded: { 26491cfc - 9e50 - 4857 - 861b - 0cb8df22b5d7}
//msExchUMDtmfMap: reversedPhone: 041224124122999110
//msExchUMDtmfMap: emailAddress: 252346367
//msExchUMDtmfMap: lastNameFirstName: 252346367
//msExchUMDtmfMap: firstNameLastName: 252346367
//msExchRecipientDisplayType: ACLable Synced Mailbox User
//msExchELCMailboxFlags: 16
//msExchVersion: 88218628259840
//msExchRecipientTypeDetails: Remote User Mailbox
//msExchArchiveStatus: 1
//msExchTextMessagingState: 302120705
//msExchTextMessagingState: 16842751
//msDS - ExternalDirectoryObjectId: User_b2fa764c - 4c51 - 4ac9 - 9602 - 63860ef18341
//msExchWhenMailboxCreated: 2015 - 04 - 17 08:55:22 AM
//msExchRemoteRecipientType: Migrated, Provision Archive
//msExchUserHoldPolicies: -mbx9743231006404928b0e8f833968e658f
//msExchArchiveName: In - Place Archive - Albfinenq
//objectGUID: { C0C0D262 - 4953 - 496B - BE53 - 24D489D75872}
//objectSid: S - 1 - 5 - 21 - 2730933329 - 88772031 - 2985635704 - 3725
//mS - DS - ConsistencyGuid: 62  D2 C0  C0  53  49  6B  49  BE  53  24  D4  89  D7  58  72
//msExchMailboxGuid: { 072C2B25 - 3611 - 4957 - 990D - 4E4477DE72B5}
//ciscoEcsbuUMLocationObjectId: 01  00  07  00  30  00  39  00  3A  00  7B  00  44  00  38  00...
//msExchSafeSendersHash: 48  79  2D  00  A5 F8  38  00  F2  5F  5D  00  F8  68  6E  00...
//msExchArchiveGUID: { A4A3D258 - BCE1 - 4DC5 - A84E - A0CA0286F4BE}
