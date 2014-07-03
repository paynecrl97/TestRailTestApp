using System;
using System.Collections.Generic;
using System.Linq;
using TestRail;
using TestRail.Types;

namespace TestRailTestApp
{
    class Program
    {
        public const int ProjectId = 24;
        public const int SuiteId = 292;
        public const int DefaultAssignee = 123;


        static void Main(string[] args)
        {
            var client = new TestRailClient("https://bedegaming.testrail.com", "chris.payne@bedegaming.com", "Cgsye2hyS5Ubl0T3Dr2w");

            var browsers = new List<string>
            {
                "IE",
                "Firefox",
                "Chrome",
                "Opera",
                "Safari",
            };

            var sites = new List<string>
            {
                "Bingo Godz",
                "Bingo Stars",
                "Health Bingo",
                "STV",
            };

            ulong browserGroupId = 3;
            ulong siteGroupId = 1;

            var allConfigs = client.GetConfigurationGroups(ProjectId);
            var browserGroup = allConfigs.Single(c => c.ID == browserGroupId);
            var siteGroup = allConfigs.Single(c => c.ID == siteGroupId);
            var runList = new List<Run>();
            var caseIds = new HashSet<ulong> {11578, 11579};

            foreach (var browser in browsers)
            {
                foreach (var site in sites)
                {
                    var browserConfig = browserGroup.Configurations.FirstOrDefault(r => r.Name == browser);
                    var siteConfig = siteGroup.Configurations.FirstOrDefault(r => r.Name == site);

                    if (browserConfig != null && siteConfig != null)
                    {
                        runList.Add(new Run { ConfigIDs = new List<ulong> { browserConfig.ID, siteConfig.ID }, CaseIDs = caseIds});
                    }
                }
            }
            
            var planEntries = new List<PlanEntry>();
            planEntries.Add(new PlanEntry
            {
                Name = "Membership",
                //CaseIDs = caseIds,
                SuiteID = 292,
                ConfigIDs = new List<ulong> { 1, 2, 3, 4, 5, 6, 7, 8 },
                RunList = runList
            });

            var planResult = client.AddPlan(ProjectId, string.Format("Test Plan {0}", DateTime.Now.Ticks), null, null, planEntries);
            //we have now successfully created a new test plan

            //update
            var rnd = new Random();
            var plan = client.GetPlan(planResult.Value);
            foreach (var entry in plan.Entries)
            {
                foreach (var run in entry.RunList)
                {
                    foreach (var caseId in caseIds)
                    {
                        switch (rnd.Next(1, 10))
                        {
                            case 1:
                                client.AddResultForCase(run.ID.Value, caseId, ResultStatus.Failed);
                                break;
                            case 2:
                                client.AddResultForCase(run.ID.Value, caseId, ResultStatus.Retest);
                                break;
                            case 4:
                                client.AddResultForCase(run.ID.Value, caseId, ResultStatus.Blocked);
                                break;
                            default:
                                client.AddResultForCase(run.ID.Value, caseId, ResultStatus.Passed);
                                break;
                        }
                    }
                }
            }
        }
    }
}
