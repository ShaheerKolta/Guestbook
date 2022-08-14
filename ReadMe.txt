Assumptions :
	1-the guestbook is done for one website only (i.e. users cannot create a new guestbook)
	2-the admin has an email admin@admin.com and userId = 1
	3-the frontend will show main messages without reply and if a user wishes to see the replies then he should click on message to retrieve them
	4-User Cannot Change his Email
	5-Message and their replies Can be Seen by anyone (Even non users) but only users can add / edit / delete or reply
	6-in edit user : if data is unchanged of a certain attribute it shall be sent as well in the object (Except for password in case of not changing it it shall be sent as empty string)


How to Use :

	-Create user by signing up
	-Login using the account created to recieve the security token
	-Show all the messages (no token or sig in required)
	-Create a new Message  (sign in required and Authorization token in header) :
		only need User_Id and Message_content from frontend
	-Reply to a message (sign in required and Authorization token in header):
		only need User_Id , Message_content and Message_id of parent from frontend


	-Edit Comment (sign in required and Authorization token in header)
	-Delete Comment (sign in required and Authorization token in header)

Improvements :
	-Use Cryptographically secure key to Hash the token (a string is used in the project)
	-Create Log to trace actions 
	-Use Salt with Password and also store it with each password
	-Use DTO



Decisions :
	1-Used Repository Design Pattern to create a layer between databse and server logic
	2-Created Static Class Hashing to has password which threr is no need to create an object from that class so I created it Static
	3-seperated the messages from the reply to decrease amount of data sent and perfermance of operation
	4-Used Post Message as a function to create a message or to reply to a message 
	5-Made a self relation in database so a reply could have another reply
	6-Get messages are ordered by date as well as get replies