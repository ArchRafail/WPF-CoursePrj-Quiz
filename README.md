# WPF-CoursePrj-Quiz
</br>
Intellectual Quiz</br>
Course project</br>
</br></br>
There are 3 windows: Authorization, Account Cabinet and Game in the game.</br>
On the first window you can create new account or enter into account.</br>
On the second window you can look at the ranking table and change password if you want.</br>
The third window is a game window with quiz's question and answer options.</br>
</br>
Need to add files Quiz And Answers.txt and Users.bin in root folder (for ex. source\repos\Quiz - Course project\Quiz - Course project\bin\Debug)</br>
To try quiz you have to create account with password.</br>
Login of the account has to be unique.</br>
Login and password shall pass the REGEX checking.
Have a look at the tips on login and password fields.</br>
</br>
There are 3 difficult levels:</br>
1 - 10 questions with 60 sec. time per each;</br>
2 - 20 questions with 40 sec. time per each;</br>
3 - 30 questions with 20 sec. time per each.</br>
</br>
Question picks up from file Quiz And Answers.txt and shows randomly from list.</br>
Each question has 4 variants of answer where just one is right.</br>
During game user can use:</br>
- Button 50%/50%: only 2 answers will shows for user, where 1 of them will be correct.</br>
- Button Hall's Help: 1 answer will shows for user. There is no 100% guarantee that answer will be correct.</br>
</br>
In total, user receives points (10 per each question) and average point.</br>
More average point you will receive if you pass more questions.</br>
User with correct answer per 5 questions from total 5 will receive less av.points from user that pass 6 questions with 5 correct answers.</br>
Any time you can leave the game with button "Close", "Cancel". Have a look at the tips on the buttons.</br>
