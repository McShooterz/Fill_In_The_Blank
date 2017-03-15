#! /usr/bin/env python
#-----------------------------------------
#	Travis Brown		2016-02-24
#	server.py
#-----------------------------------------
#	
#	This is a simple server program for testing.
#	This will be built up to become the primary server code.
#	
#---------------------
#	
#	the import print_function should allow this to run on python 2 or python 3
#	
#-----------------------------------------
"""
"""
from __future__ import print_function

import sys
import os
import random
import socket               # Import socket module


#---------------------------------
# '''

# This is server.py file

#-list used to colorize cmd output
#--...did not work as planned...
colorList = []
for C in range(0,10):
	colorList.append(C)
colorList.append("A")
colorList.append("B")
colorList.append("C")
colorList.append("D")
colorList.append("E")
colorList.append("F")


message_list = []
message_list.append("Dear Mr. Client,\n\tThank you for connecting. Have a wonderful day.\n\nsigned,\nThe Server")
message_list.append("...\nmessage?? What message???")
message_list.append("\ntesting, testing...Hello?\n\n...??\n")
message_list.append("Aaaggh!! I'm trapped in a shitty python server!!!1!111!!!\n\n...\n\n..Hello?")

s = socket.socket()         # Create a socket object
host = socket.gethostname() # Get local machine name
port = 12345                # Reserve a port for your service.
s.bind((host, port))        # Bind to the port

s.listen(5)                 # Now wait for client connection.
N = 0
print("...now serving on port: [{}]...".format(port))
while True:
	c, addr = s.accept()     # Establish connection with client.
	print(" - Got connection from: [{}]".format(addr))
	print(" - server: sending message...")
	
	#-Send message;  notice the use of encode()
	# message = message_list[0]
	message = message_list[random.randint(0, len(message_list)-1)]
	c.send(message.encode())

	#-Colorize cmd output  ...did not work as intended...
	# for i in colorList:
		# os.system("color {}".format(i))
		# print(" - color {}".format(i))

	print(" ... message sent. Closing connection now.")
	c.close()                # Close the connection
	
	#-Make sure to take breaks
	N += 1
	if N > 5:
		break


# '''
print(" - server: ending program")
##  end program

