using System;
using System.Collections.Generic;

namespace ST10480375_PROG6221_POE_PART_3
{
    public class ActivityLog
    {
        private List<string> log = new List<string>();

        // adds an entry with a timestamp
        public void Add(string action)
        {
            string entry = "[" + DateTime.Now.ToString("HH:mm") + "] " + action;
            log.Add(entry);
        }

        // returns the last 10 entries as a formatted string
        public string GetLog()
        {
            if (log.Count == 0)
                return "No activity recorded yet.";

            int start = Math.Max(0, log.Count - 10);
            string result = "Here is a summary of recent actions:\n";
            int number = 1;
            for (int i = start; i < log.Count; i++)
            {
                result += number + ". " + log[i] + "\n";
                number++;
            }
            return result;
        }
    }
}