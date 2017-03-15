#! /usr/bin/env python
#-----------------------------------------
#	Travis Brown		2016-02-24
#	client.py
#-----------------------------------------
#	
#	This is a simple client program for testing.
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
import time
import socket               # Import socket module


#---------------------------------
# '''

# This is client.py file


s = socket.socket()         # Create a socket object
# host = socket.gethostname() # Get local machine name
host = "192.168.1.210"      # Hard-code specific IP
port = 12345                # Reserve a port for your service.

s.connect((host, port))
message_from_server = s.recv(1024).decode()
print("-"*42)
print("- Message from server:")
print("-"*21)
print ("{}".format(message_from_server))
print("-"*42)
s.close                     # Close the socket when done


# '''
print("-  client: ending program")
##  end program

