# ST10480375_PROG6221_POE_PART2
Cyber Crime Protection - ST10480375 PROG6221 POE Part 2

## Part 2 — WPF GUI Application

### New Features Added
- Full WPF graphical user interface with dark cybersecurity theme
- All Part 1 features into the GUI (voice greeting, ASCII logo, topics)
- Keyword recognition for topics
- Random responses — multiple answers per topic using a List
- Conversation - say "tell me more" or "another tip" to continue a topic
- Memory and recall — chatbot remembers your name, favourite topic, and last topic discussed
- Sentiment detection — detects worried, curious, and frustrated moods and adjusts responses
- Error handling — default responses for unrecognised input, no crashes on unexpected input
- Quick topic buttons for fast answers
- memory bar showing stored user information

### How to Use
1. Open the project in Visual Studio
2. Build and run
3. Enter your name when prompted
4. Type a question or click a quick topic button
5. Say "tell me more" to get another tip on the same topic
6. Say "my name is ..." or "I'm interested in ..." to personalise the conversation
7. Say "what do you remember about me?" to see stored memory
8. Type "help" to see all available topics

### Prompt Topics
- password
- phishing
- malware
- ransomware
- spyware
- virus / worms / macros
- safe browsing
- 2FA
- firewall
- antivirus
- privacy
- scam

### Project Structure
ST10480375_PROG6221_POE_PART_2/
├── Memory.cs            — stores user name, favourite topic, last topic
├── Chatbot.cs           — all chatbot main code, keyword detection, responses
├── MainWindow.xaml      — WPF GUI layout
├── MainWindow.xaml.cs   — input and displays messages
└── README.md


---

## Part 3 — POE Final Submission

### New Features Added
- Task Assistant — add, view, complete and delete cybersecurity tasks stored in SQL Server
- Cybersecurity Quiz — 12 questions, multiple choice and true/false with final score and feedback
- NLP Simulation — recognises different ways users phrase the same request
- Activity Log — tracks and displays the last 10 actions taken in the chatbot

### Commands
- `add task [description]` — add a cybersecurity task
- `view tasks` — see all your tasks
- `complete task [number]` — mark a task as done
- `delete task [number]` — remove a task
- `start quiz` — launch the cybersecurity quiz
- `show activity log` — view recent chatbot actions

### Database Setup
1. Open SQL Server Management Studio
2. Run `database.sql` to create the database and table
3. Update the connection string in `DatabaseHelper.cs` with your server name
4. Build and run

### Project Structure
```
ST10480375_PROG6221_POE_PART_3/
├── Memory.cs
├── ActivityLog.cs
├── DatabaseHelper.cs
├── QuizManager.cs
├── Chatbot.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── database.sql
└── README.md
```


Youtube link: 
