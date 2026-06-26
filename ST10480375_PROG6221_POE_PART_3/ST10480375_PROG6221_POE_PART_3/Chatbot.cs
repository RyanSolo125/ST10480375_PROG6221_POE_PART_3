using ST10480375_PROG6221_POE_PART_3;
using System;
using System.Collections.Generic;

namespace ST10480375_PROG6221_POE_PART_3
{
    public class Chatbot
    {
        private Memory memory = new Memory();
        private Random random = new Random();
        private DatabaseHelper db = new DatabaseHelper();
        private QuizManager quiz = new QuizManager();
        private ActivityLog activityLog = new ActivityLog();

        // used when bot is waiting for a reminder after adding a task
        private bool awaitingReminder = false;
        private string pendingTaskTitle = "";
        private string pendingTaskDesc = "";

        // stores multiple responses per topic for random selection
        private Dictionary<string, List<string>> responses = new Dictionary<string, List<string>>()
        {
            { "password", new List<string> {
                "Use strong passwords with letters, numbers and symbols. Never reuse them.",
                "Never share your password with anyone, not even IT support.",
                "Use a password manager to keep track of strong unique passwords."
            }},
            { "phishing", new List<string> {
                "Phishing tricks you into giving personal info. Always check the sender's email.",
                "Be cautious of urgent emails asking you to click links or enter details.",
                "Hover over links before clicking to see where they really go."
            }},
            { "malware", new List<string> {
                "Malware is harmful software. Keep your antivirus updated to stay protected.",
                "Signs of malware include slow performance and unexpected pop-ups.",
                "Only download software from trusted, official sources."
            }},
            { "ransomware", new List<string> {
                "Ransomware locks your files and demands payment. Never pay — back up your files instead.",
                "Regular backups are the best defence against ransomware attacks.",
                "Ransomware usually arrives through phishing emails or unsafe downloads."
            }},
            { "privacy", new List<string> {
                "Check your social media privacy settings and limit what strangers can see.",
                "Use a VPN on public Wi-Fi to protect your personal data.",
                "Be careful what personal information you share online."
            }},
            { "scam", new List<string> {
                "If something looks too good to be true, it probably is a scam.",
                "Scammers pretend to be banks or government offices. Always verify by calling back.",
                "Never pay anyone using gift cards — this is always a scam."
            }},
            { "firewall", new List<string> {
                "A firewall blocks unauthorised access to your computer or network.",
                "Always keep your firewall turned on, even at home.",
                "Firewalls work best when combined with antivirus software."
            }},
            { "antivirus", new List<string> {
                "Antivirus software detects and removes harmful programs from your device.",
                "Keep your antivirus updated so it can catch the latest threats.",
                "Run regular scans to make sure your device stays clean."
            }},
            { "2fa", new List<string> {
                "Two-factor authentication means even a stolen password won't let attackers in.",
                "Use an authenticator app for 2FA instead of SMS — it is more secure.",
                "Turn on 2FA for every account that supports it, especially email."
            }},
            { "virus", new List<string> {
                "A virus attaches to programs and spreads when you run them.",
                "Avoid downloading files from unknown or untrusted websites.",
                "Keep your operating system updated to patch security holes viruses exploit."
            }},
            { "spyware", new List<string> {
                "Spyware secretly records your activity and sends it to attackers.",
                "Be careful installing free apps — many contain hidden spyware.",
                "A good antivirus with real-time protection can block spyware."
            }},
            { "safe browsing", new List<string> {
                "Always check for HTTPS and a padlock before entering personal details.",
                "Avoid clicking on pop-up ads — they can lead to malicious sites.",
                "Use a browser extension to block harmful ads and tracking scripts."
            }}
        };

        public Memory GetMemory() { return memory; }
        public ActivityLog GetActivityLog() { return activityLog; }

        public string GetResponse(string userInput)
        {
            string input = userInput.ToLower().Trim();
            string sentimentNote = DetectSentiment(input);
            bool hasSentiment = sentimentNote != "";

            // if quiz is active, send input to quiz
            if (quiz.IsActive)
            {
                if (input.Contains("stop quiz") || input.Contains("quit quiz"))
                {
                    quiz = new QuizManager();
                    activityLog.Add("Quiz exited early.");
                    return "Quiz stopped. Type 'start quiz' to try again.";
                }
                string quizResult = quiz.SubmitAnswer(input);
                if (!quiz.IsActive)
                    activityLog.Add("Quiz completed.");
                return quizResult;
            }

            // if bot is waiting for a reminder response
            if (awaitingReminder)
            {
                awaitingReminder = false;

                if (input.Contains("no") || input.Contains("nope"))
                {
                    db.AddTask(pendingTaskTitle, pendingTaskDesc, "");
                    activityLog.Add("Task added: '" + pendingTaskTitle + "' with no reminder.");
                    pendingTaskTitle = "";
                    pendingTaskDesc = "";
                    return "No problem! Task saved without a reminder.";
                }
                else if (input.Contains("yes") || input.Contains("sure") || input.Contains("ok"))
                {
                    return "When would you like to be reminded? (e.g., 'in 3 days', 'tomorrow')";
                }
                else
                {
                    // they typed a time directly like "in 3 days"
                    db.AddTask(pendingTaskTitle, pendingTaskDesc, userInput);
                    activityLog.Add("Task added: '" + pendingTaskTitle + "' with reminder: " + userInput);
                    pendingTaskTitle = "";
                    pendingTaskDesc = "";
                    return "Got it! I'll remind you " + userInput + ". Task saved successfully.";
                }
            }

            // activity log
            if (input.Contains("activity log") || input.Contains("what have you done") || input.Contains("show log"))
            {
                return activityLog.GetLog();
            }

            // quiz triggers
            if (input.Contains("start quiz") || input.Contains("take quiz") ||
                input.Contains("play quiz") || input.Contains("test me") ||
                input.Contains("quiz me") || input.Contains("begin quiz"))
            {
                activityLog.Add("Quiz started.");
                return quiz.Start();
            }

            // adding tasks
            if (input.Contains("add task") || input.Contains("create task") ||
                input.Contains("new task") || input.Contains("set a task") ||
                input.Contains("remind me to") || input.Contains("set reminder") ||
                input.Contains("remember to"))
            {
                // remove the command phrase to get just the task content
                string taskContent = input;
                string[] removePhrases = { "add task", "create task", "new task",
                                           "set a task", "remind me to",
                                           "set reminder", "remember to" };
                foreach (string phrase in removePhrases)
                    taskContent = taskContent.Replace(phrase, "").Trim();

                if (taskContent.Length < 2)
                    return "What task would you like to add? Example: 'add task review privacy settings'";

                string title = char.ToUpper(taskContent[0]) + taskContent.Substring(1);
                string desc = GetTaskDescription(taskContent);

                pendingTaskTitle = title;
                pendingTaskDesc = desc;
                awaitingReminder = true;

                return "Task added: '" + title + "'\n" + desc +
                       "\nWould you like to set a reminder for this task? (yes / no)";
            }

            // task: view
            if (input.Contains("view task") || input.Contains("show task") ||
                input.Contains("list task") || input.Contains("my tasks") ||
                input.Contains("see tasks"))
            {
                return ViewTasks();
            }

            // task: complete
            if (input.Contains("complete task") || input.Contains("done with task") ||
                input.Contains("finish task") || input.Contains("mark task"))
            {
                foreach (string word in userInput.Split(' '))
                {
                    if (int.TryParse(word, out int taskId))
                    {
                        if (db.CompleteTask(taskId))
                        {
                            activityLog.Add("Task #" + taskId + " marked as completed.");
                            return "Task #" + taskId + " marked as completed!";
                        }
                    }
                }
                return "Please include the task number. Example: 'complete task 2'";
            }

            // task: delete
            if (input.Contains("delete task") || input.Contains("remove task"))
            {
                foreach (string word in userInput.Split(' '))
                {
                    if (int.TryParse(word, out int taskId))
                    {
                        if (db.DeleteTask(taskId))
                        {
                            activityLog.Add("Task #" + taskId + " deleted.");
                            return "Task #" + taskId + " has been deleted.";
                        }
                    }
                }
                return "Please include the task number. Example: 'delete task 2'";
            }

            // name
            if (input.Contains("my name is"))
            {
                string name = input.Replace("my name is", "").Trim();
                name = char.ToUpper(name[0]) + name.Substring(1);
                memory.UserName = name;
                activityLog.Add("User introduced themselves as " + name + ".");
                return "Nice to meet you, " + name + "! How can I help you stay safe online?";
            }

            // interest
            if (input.Contains("i am interested in") || input.Contains("i'm interested in"))
            {
                string topic = input.Replace("i'm interested in", "")
                                    .Replace("i am interested in", "").Trim();
                memory.FavouriteTopic = topic;
                activityLog.Add("User expressed interest in: " + topic + ".");
                return "Great! I'll remember that you're interested in " + topic + ".";
            }

            // memory recall
            if (input.Contains("what do you remember") || input.Contains("what do you know about me"))
            {
                return GetMemorySummary();
            }

            // follow-up on last topic
            if (input.Contains("tell me more") || input.Contains("another tip") || input.Contains("more info"))
            {
                if (memory.LastTopic != null)
                    return sentimentNote + GetRandomResponse(memory.LastTopic);
                return "What topic would you like more information on?";
            }

            // help
            if (input.Contains("help") || input.Contains("what can you do"))
            {
                return "You can ask me about:\n" +
                       "- password, phishing, malware, ransomware\n" +
                       "- privacy, scam, firewall, antivirus\n" +
                       "- 2fa, virus, spyware, safe browsing\n\n" +
                       "Task commands:\n" +
                       "- 'add task [description]'\n" +
                       "- 'view tasks'\n" +
                       "- 'complete task [number]'\n" +
                       "- 'delete task [number]'\n\n" +
                       "Other commands:\n" +
                       "- 'start quiz' — test your knowledge\n" +
                       "- 'show activity log' — see recent actions\n" +
                       "- 'my name is ...' — I will remember you\n" +
                       "- 'I am interested in ...' — save your interest";
            }

            if (input.Contains("how are you"))
                return "I am doing great and ready to help you stay safe online!";

            if (input.Contains("purpose") || input.Contains("who are you"))
                return "I am CCP, your Cyber Crime Protection chatbot. I help you stay safe online, manage cybersecurity tasks, and test your knowledge.";

            // keyword detection for cybersecurity topics
            foreach (string keyword in responses.Keys)
            {
                if (input.Contains(keyword))
                {
                    memory.LastTopic = keyword;
                    string tip = GetRandomResponse(keyword);
                    activityLog.Add("Discussed topic: " + keyword + ".");
                    return sentimentNote + tip;
                }
            }

            // sentiment detected but no keyword matched
            if (hasSentiment)
            {
                if (memory.LastTopic != null)
                    return sentimentNote + GetRandomResponse(memory.LastTopic);
                return sentimentNote + "I am here to help. Type 'help' to see all available topics.";
            }

            // default fallback
            return "I am not sure I understand. Type 'help' to see what I can assist with.";
        }

        // generates a description based on keywords in the task
        private string GetTaskDescription(string taskContent)
        {
            if (taskContent.Contains("password"))
                return "Description: Update your password to a strong, unique one.";
            if (taskContent.Contains("2fa") || taskContent.Contains("two factor") || taskContent.Contains("authentication"))
                return "Description: Enable two-factor authentication to secure your account.";
            if (taskContent.Contains("privacy"))
                return "Description: Review your account privacy settings to protect your data.";
            if (taskContent.Contains("antivirus") || taskContent.Contains("scan"))
                return "Description: Run a full antivirus scan on your device.";
            if (taskContent.Contains("backup"))
                return "Description: Back up your important files to a secure location.";
            if (taskContent.Contains("update") || taskContent.Contains("patch"))
                return "Description: Install the latest software updates and security patches.";
            return "Description: " + char.ToUpper(taskContent[0]) + taskContent.Substring(1) + ".";
        }

        // formats all tasks from the database into a readable list
        private string ViewTasks()
        {
            List<TaskItem> tasks = db.GetAllTasks();
            if (tasks.Count == 0)
                return "You have no tasks yet. Say 'add task' followed by a description to create one.";

            string result = "Your cybersecurity tasks:\n";
            foreach (TaskItem task in tasks)
            {
                string status = task.IsCompleted ? "[Done]" : "[Pending]";
                result += "\n" + task.Id + ". " + status + " " + task.Title;
                if (task.Description != "") result += "\n   " + task.Description;
                if (task.Reminder != "") result += "\n   Reminder: " + task.Reminder;
            }
            return result;
        }

        // picks a random response from the list for a given topic
        private string GetRandomResponse(string topic)
        {
            List<string> options = responses[topic];
            return options[random.Next(options.Count)];
        }

        // detects the user's mood and returns an empathetic prefix
        private string DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("scared") ||
                input.Contains("anxious") || input.Contains("nervous") ||
                input.Contains("afraid") || input.Contains("unsafe"))
            {
                return "It's completely understandable to feel that way. " +
                       "Scammers and cybercriminals can be very convincing. " +
                       "Let me share something that can help:\n";
            }

            if (input.Contains("frustrated") || input.Contains("confused") ||
                input.Contains("don't understand") || input.Contains("complicated") ||
                input.Contains("difficult") || input.Contains("too hard"))
            {
                return "I hear you — cybersecurity can feel overwhelming at first. " +
                       "Let me break it down simply:\n";
            }

            if (input.Contains("curious") || input.Contains("interesting") ||
                input.Contains("want to know") || input.Contains("tell me about"))
            {
                return "Great curiosity — learning about this is the first step to staying safe!\n";
            }

            return "";
        }

        // builds a summary of what the bot remembers about the user
        private string GetMemorySummary()
        {
            if (memory.UserName == null && memory.FavouriteTopic == null && memory.LastTopic == null)
                return "I don't know much about you yet! Try saying 'my name is ...' or 'I am interested in ...'";

            string result = "Here is what I remember: ";
            if (memory.UserName != null) result += "Your name is " + memory.UserName + ". ";
            if (memory.FavouriteTopic != null) result += "You are interested in " + memory.FavouriteTopic + ". ";
            if (memory.LastTopic != null) result += "We last talked about " + memory.LastTopic + ".";
            return result;
        }
    }
}