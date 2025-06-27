using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CyberSecurityAssistant
{
    public partial class MainWindow : Window
    {
        private List<string> chatHistory = new();
        private List<(string Title, string ReminderDate)> tasks = new();
        private List<string> activityLog = new();

        private int quizScore = 0;
        private int quizIndex = 0;
        private bool quizActive = false;

        private readonly List<(string Question, List<string> Options, string Answer, string Explanation)> quizQuestions = new()
        {
            ("What should you do if you receive an email asking for your password?",
             new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" },
             "C",
             "Correct! Reporting phishing emails helps prevent scams."),

            ("True or False: It's safe to reuse the same password for multiple accounts.",
             new List<string> { "A) True", "B) False" },
             "B",
             "Correct! Reusing passwords increases your risk if one site is compromised."),

            ("Which of these is a strong password?",
             new List<string> { "A) 123456", "B) password", "C) Summer2024!@#", "D) qwerty" },
             "C",
             "Correct! Use complex passwords with letters, numbers, and symbols."),

            ("What is two-factor authentication (2FA)?",
             new List<string> { "A) Using two devices", "B) Logging in twice", "C) A second layer of security", "D) Changing passwords often" },
             "C",
             "Correct! 2FA adds a second verification step to secure your accounts."),

            ("True or False: Public Wi-Fi networks are always safe to use.",
             new List<string> { "A) True", "B) False" },
             "B",
             "Correct! Avoid using public Wi-Fi for sensitive activities."),

            ("Which action can help protect your online privacy?",
             new List<string> { "A) Sharing your location always", "B) Using incognito mode", "C) Reviewing account settings", "D) Posting everything online" },
             "C",
             "Correct! Regularly review your privacy settings."),

            ("What is phishing?",
             new List<string> { "A) A type of virus", "B) Fake messages to steal data", "C) Spam emails", "D) Email newsletters" },
             "B",
             "Correct! Phishing attempts to trick you into revealing personal information."),

            ("True or False: Clicking popups on websites is a safe way to find deals.",
             new List<string> { "A) True", "B) False" },
             "B",
             "Correct! Popups may lead to scams or malware."),

            ("Which of the following is safest when creating a password?",
             new List<string> { "A) Your pet's name", "B) A random mix of characters", "C) Your birthday", "D) Your favorite color" },
             "B",
             "Correct! Use unpredictable, unique passwords."),

            ("What should you do if your device is acting strangely and slowing down?",
             new List<string> { "A) Ignore it", "B) Restart only", "C) Run a malware scan", "D) Close all programs" },
             "C",
             "Correct! Strange behavior may mean malware — scan your device!"),
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput)) return;

            AppendToChat($"🧑‍💻 You: {userInput}");
            string response = GetBotResponse(userInput);
            AppendToChat($"🤖 Bot: {response}");

            UserInput.Clear();
        }

        private void AppendToChat(string message)
        {
            ChatBox.AppendText(message + "\n");
            ChatBox.ScrollToEnd();
        }

        private string GetBotResponse(string input)
        {
            input = input.ToLower();

            if (quizActive)
            {
                return EvaluateQuizAnswer(input);
            }

            if (input.Contains("quiz"))
            {
                quizActive = true;
                quizIndex = 0;
                quizScore = 0;
                activityLog.Add($"Quiz started at {DateTime.Now.ToShortTimeString()}");
                return AskNextQuizQuestion();
            }
            else if (input.Contains("task") || input.Contains("remind"))
            {
                return HandleTaskInput(input);
            }
            else if (input.Contains("summary") || input.Contains("what have you done") || input.Contains("activity log"))
            {
                return GetActivityLog();
            }
            else if (input.Contains("password"))
            {
                return "🔐 Use strong, unique passwords and consider a password manager!";
            }
            else if (input.Contains("phishing"))
            {
                return "🎣 Beware of emails asking for personal info. Always verify the sender.";
            }

            return "🤔 I’m not sure I understand. Try asking for a quiz, adding a task, or requesting tips on cybersecurity.";
        }

        private string AskNextQuizQuestion()
        {
            if (quizIndex >= quizQuestions.Count)
            {
                quizActive = false;
                activityLog.Add($"Quiz completed. Score: {quizScore}/{quizQuestions.Count}");
                string feedback = quizScore >= 8 ? "🎉 Great job! You're a cybersecurity pro!" :
                                  quizScore >= 5 ? "👍 Good effort! Keep learning to stay safe online!" :
                                  "📘 Keep learning — cybersecurity is important!";
                return $"Quiz complete! You scored {quizScore}/{quizQuestions.Count}.\n{feedback}";
            }

            var (question, options, _, _) = quizQuestions[quizIndex];
            string formattedOptions = string.Join("\n", options);
            return $"Question {quizIndex + 1}: {question}\n{formattedOptions}\n(Type A, B, C or D)";
        }

        private string EvaluateQuizAnswer(string input)
        {
            var (_, _, correctAnswer, explanation) = quizQuestions[quizIndex];

            if (input.ToUpper().StartsWith(correctAnswer))
            {
                quizScore++;
                AppendToChat("✅ Correct!");
            }
            else
            {
                AppendToChat("❌ Incorrect.");
            }

            AppendToChat(explanation);
            quizIndex++;
            return AskNextQuizQuestion();
        }

        private string HandleTaskInput(string input)
        {
            string taskTitle = "";
            string reminder = "";

            if (input.Contains("password"))
                taskTitle = "Update my password";
            else if (input.Contains("2fa"))
                taskTitle = "Enable two-factor authentication";
            else
                taskTitle = "Cybersecurity Task";

            if (input.Contains("tomorrow"))
                reminder = DateTime.Now.AddDays(1).ToShortDateString();
            else if (input.Contains("in "))
            {
                var parts = input.Split("in ");
                if (parts.Length > 1 && int.TryParse(parts[1].Split(' ')[0], out int days))
                    reminder = DateTime.Now.AddDays(days).ToShortDateString();
            }

            tasks.Add((taskTitle, reminder));

            if (!string.IsNullOrEmpty(reminder))
            {
                activityLog.Add($"Reminder set for '{taskTitle}' on {reminder}.");
                return $"Reminder set for '{taskTitle}' on {reminder}.";
            }
            else
            {
                activityLog.Add($"Task added: '{taskTitle}' (no reminder).");
                return $"Task added: '{taskTitle}' (no reminder set).";
            }
        }

        private string GetActivityLog()
        {
            if (activityLog.Count == 0)
                return "No recent activity found.";

            var recent = activityLog.TakeLast(10).Reverse();
            string summary = "Here's a summary of recent actions:\n";
            int count = 1;
            foreach (var entry in recent)
                summary += $"{count++}. {entry}\n";

            return summary.TrimEnd();
        }

        private void ShowLog_Click(object sender, RoutedEventArgs e)
        {
            AppendToChat($"🤖 Bot: {GetActivityLog()}");
        }
    }
}
