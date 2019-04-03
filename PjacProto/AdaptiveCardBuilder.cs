using System;
using System.Collections.Generic;

namespace PjacProto
{
    class AdaptiveCardBuilder
    {
        public class PrintJob
        {
            public enum JobStatus
            {
                Printing,
                Printed
            }

            public string Name { get; }
            public DateTime Submitted { get; }
            public JobStatus Status { get; }
            public string IconUrl { get; }
            public string Owner { get; }
            public int Pages { get; }
            public DateTime Completed { get; }
            public PrintJob(string name, DateTime submitted, JobStatus status, string iconUrl, string owner, int pages, DateTime completed)
            {
                Name = name;
                Submitted = submitted;
                Status = status;
                IconUrl = iconUrl;
                Owner = owner;
                Pages = pages;
                Completed = completed;
            }
            public override string ToString()
            {
                string statusText = (Status == JobStatus.Printed) ? "Printed" : "Printing";

                string card = Util.ReadTextAsset("pjac_adaptivecards_template_job.json");
                card = card.Replace("%%NAME%%", Name);
                card = card.Replace("%%STATUS%%", statusText);
                card = card.Replace("%%ICON_URL%%", IconUrl);
                card = card.Replace("%%OWNER%%", Owner);
                card = card.Replace("%%PAGES%%", Pages.ToString());
                card = card.Replace("%%DATE_SUBMITTED%%", Submitted.ToString("s"));

                string completedJson = "";
                if (Status == JobStatus.Printed)
                {
                    completedJson = Util.ReadTextAsset("pjac_adaptivecards_template_job_completed.json");
                    completedJson = completedJson.Replace("%%DATE_COMPLETED%%", Completed.ToString("s"));
                }
                card = card.Replace("%%COMPLETED_JSON%%", completedJson);

                return card;
            }
        }

        readonly List<PrintJob> m_printJobs = new List<PrintJob>();

        public void AddJob(PrintJob job)
        {
            m_printJobs.Insert(0, job);
        }

        public override string ToString()
        {
            string jobs = "";
            bool needCommaSeparator = false;
            foreach (var job in m_printJobs)
            {
                if (needCommaSeparator)
                {
                    jobs += ",";
                }
                else
                {
                    needCommaSeparator = true;
                }

                jobs += job.ToString();
            }
            string result = Util.ReadTextAsset("pjac_adaptivecards_template_begin.json") + jobs + Util.ReadTextAsset("pjac_adaptivecards_template_end.json");
            return result;
        }
    }
}
