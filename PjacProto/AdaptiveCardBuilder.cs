using System;
using System.Collections.Generic;

namespace PjacProto
{
    class AdaptiveCardBuilder
    {
        public class PrintJob
        {
            public string Name { get; }
            public DateTime Submitted { get; }
            public string Status { get; }
            public string Owner { get; }
            public int Pages { get; }
            public DateTime Completed { get; }
            public PrintJob(string name, DateTime submitted, string status, string owner, int pages, DateTime completed)
            {
                Name = name;
                Submitted = submitted;
                Status = status;
                Owner = owner;
                Pages = pages;
                Completed = completed;
            }
            public override string ToString()
            {
                string card = Util.ReadTextAsset("pjac_adaptivecards_template_job.json");
                card = card.Replace("%%NAME%%", Name);
                card = card.Replace("%%STATUS%%", Status);
                card = card.Replace("%%OWNER%%", Owner);
                card = card.Replace("%%PAGES%%", Pages.ToString());
                // Submitted & completed are in this format: 2019-03-30T00:01:00Z
                card = card.Replace("%%DATE_SUBMITTED%%", Submitted.ToString("s"));
                card = card.Replace("%%DATE_COMPLETED%%", Completed.ToString("s"));
                return card;
            }
        }

        readonly List<PrintJob> m_printJobs = new List<PrintJob>();

        public void AddJob(PrintJob job)
        {
            m_printJobs.Add(job);
        }

        public override string ToString()
        {
            string jobs = "";
            foreach (var job in m_printJobs)
            {
                jobs += job.ToString();
            }
            string result = Util.ReadTextAsset("pjac_adaptivecards_template_begin.json") + jobs + Util.ReadTextAsset("pjac_adaptivecards_template_end.json");
            return result;
        }
    }
}
