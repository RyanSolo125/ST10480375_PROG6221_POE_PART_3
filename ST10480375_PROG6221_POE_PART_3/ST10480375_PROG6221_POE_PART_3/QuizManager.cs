using System;
using System.Collections.Generic;

namespace ST10480375_PROG6221_POE_PART_3
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; }
    }

    public class QuizManager
    {
        private List<QuizQuestion> questions;
        private int currentIndex = 0;
        private int score = 0;
        public bool IsActive { get; private set; } = false;

        public QuizManager()
        {
            questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> {
                        "A) Reply with your password",
                        "B) Delete the email",
                        "C) Report it as phishing",
                        "D) Ignore it" },
                    CorrectIndex = 2,
                    Explanation = "Correct answer: C. Reporting phishing emails helps prevent scams."
                },
                new QuizQuestion
                {
                    Question = "True or False: Using the same password for all accounts is safe.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "Correct answer: False. If one account is breached, all accounts are at risk."
                },
                new QuizQuestion
                {
                    Question = "What does HTTPS mean in a website address?",
                    Options = new List<string> {
                        "A) The site is fast",
                        "B) The site is popular",
                        "C) The connection is encrypted and secure",
                        "D) The site is free" },
                    CorrectIndex = 2,
                    Explanation = "Correct answer: C. HTTPS means the connection is encrypted."
                },
                new QuizQuestion
                {
                    Question = "True or False: Two-factor authentication makes your account more secure.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 0,
                    Explanation = "Correct answer: True. 2FA adds an extra layer beyond just a password."
                },
                new QuizQuestion
                {
                    Question = "What is malware?",
                    Options = new List<string> {
                        "A) A type of hardware",
                        "B) Harmful software designed to damage systems",
                        "C) A secure email service",
                        "D) A firewall type" },
                    CorrectIndex = 1,
                    Explanation = "Correct answer: B. Malware is any software designed to harm or disrupt."
                },
                new QuizQuestion
                {
                    Question = "True or False: Public Wi-Fi is always safe to use for banking.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "Correct answer: False. Public Wi-Fi can be monitored by attackers."
                },
                new QuizQuestion
                {
                    Question = "What is ransomware?",
                    Options = new List<string> {
                        "A) Software that speeds up your PC",
                        "B) A type of antivirus",
                        "C) Malware that locks your files and demands payment",
                        "D) A firewall program" },
                    CorrectIndex = 2,
                    Explanation = "Correct answer: C. Ransomware encrypts files and demands money."
                },
                new QuizQuestion
                {
                    Question = "True or False: You should click links in emails from unknown senders.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "Correct answer: False. Links from unknown senders can lead to malware."
                },
                new QuizQuestion
                {
                    Question = "Which of these is the strongest password?",
                    Options = new List<string> {
                        "A) password123",
                        "B) john1990",
                        "C) Tr0ub4dor&3!",
                        "D) 12345678" },
                    CorrectIndex = 2,
                    Explanation = "Correct answer: C. Strong passwords mix uppercase, lowercase, numbers and symbols."
                },
                new QuizQuestion
                {
                    Question = "What does a firewall do?",
                    Options = new List<string> {
                        "A) Speeds up your internet",
                        "B) Blocks unauthorised network access",
                        "C) Removes viruses",
                        "D) Backs up your files" },
                    CorrectIndex = 1,
                    Explanation = "Correct answer: B. A firewall monitors traffic and blocks suspicious connections."
                },
                new QuizQuestion
                {
                    Question = "True or False: Antivirus software should be updated regularly.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 0,
                    Explanation = "Correct answer: True. Updates ensure antivirus can detect the latest threats."
                },
                new QuizQuestion
                {
                    Question = "What is social engineering in cybersecurity?",
                    Options = new List<string> {
                        "A) Building social media apps",
                        "B) Tricking people into revealing confidential information",
                        "C) Coding social networks",
                        "D) Managing IT teams" },
                    CorrectIndex = 1,
                    Explanation = "Correct answer: B. Social engineering manipulates people to give up sensitive info."
                }
            };
        }

        // starts the quiz and returns the first question
        public string Start()
        {
            IsActive = true;
            currentIndex = 0;
            score = 0;
            return "Quiz started! Answer with A, B, C, or D.\n\n" + GetCurrentQuestion();
        }

        // returns the current question as a formatted string
        public string GetCurrentQuestion()
        {
            if (currentIndex >= questions.Count)
                return EndQuiz();

            QuizQuestion q = questions[currentIndex];
            string output = "Question " + (currentIndex + 1) + " of " + questions.Count + ":\n";
            output += q.Question + "\n";
            foreach (string option in q.Options)
                output += option + "\n";
            return output;
        }

        // checks the user's answer and returns feedback
        public string SubmitAnswer(string userInput)
        {
            if (!IsActive) return "";

            userInput = userInput.ToLower().Trim();
            QuizQuestion q = questions[currentIndex];

            int answerIndex = -1;
            if (userInput == "a") answerIndex = 0;
            else if (userInput == "b") answerIndex = 1;
            else if (userInput == "c") answerIndex = 2;
            else if (userInput == "d") answerIndex = 3;

            if (answerIndex == -1)
                return "Please answer with A, B, C, or D.";

            string feedback;
            if (answerIndex == q.CorrectIndex)
            {
                score++;
                feedback = "Correct!\n" + q.Explanation;
            }
            else
            {
                feedback = "Not quite.\n" + q.Explanation;
            }

            currentIndex++;

            if (currentIndex >= questions.Count)
                return feedback + "\n\n" + EndQuiz();

            return feedback + "\n\n" + GetCurrentQuestion();
        }

        // ends the quiz and returns the final score with feedback
        private string EndQuiz()
        {
            IsActive = false;
            string result = "Quiz complete! You scored " + score + " out of " + questions.Count + ".\n";

            if (score == questions.Count)
                result += "Outstanding! You are a cybersecurity pro!";
            else if (score >= questions.Count * 0.7)
                result += "Great job! You have solid cybersecurity knowledge.";
            else if (score >= questions.Count * 0.4)
                result += "Good effort! Keep learning to stay safe online.";
            else
                result += "Keep learning — cybersecurity knowledge is your best defence!";

            return result;
        }
    }
}