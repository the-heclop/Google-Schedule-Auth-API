# Schedule App API

Google API to list available time blocks from Google Calendar API and API to schedule a 1 hour time block on the calendar.

Auth Controller demonstrates to generate a JWT if the user exists within a sql server database and verify the BCrypt hashed password to generate token.

To implement the AuthController, BCrypt hash will be needed for stored passwords to implement on client application 


# Requirements:

1. Google Controller
[x] The Google event should be able to send user inputs into requested event
[] Implement Authorize decorator for available times for JWT token
[x] List available time blocks based on free time from Google calendar
[x] Client can ony schedule 1 event at a time

2. Account Controller
[x] Create JWT token on account registration
[x] Create JWT when user logs in
[x] Store JWT in local storage for later use in decorated endpoints
[X] Bcrypt passwords and verify hashed passwords

3. Query Controller
[X] CRUD commands to look up data by user input
[x] Allow multiple entries

# Features
• MySQL database connection to store and read information


![Image](C:\Users\bich\OneDrive\Pictures\landing.png?raw=true)
  
