#! /usr/bin/env python
#-----------------------------------------
#	PythonServer_v3.py
#	cs340 team05		spring 2016
#	Mike Shoots
#	Travis Brown
#	Tyler Poff
#-----------------------------------------
#	This is the server for the "Fill in the ______" game
#	for cs340 Software Engineering group project.
#------------
#	This is v3, created to prepare the code to handle
#	multiple players at once through the use of threads.
#-----------------------------------------
import socket
import sys
import random
import re
import time
from threading import Thread
from PlayerThread import PlayerThread
from AiPlayer import AiPlayer
from Game import Game

#=========================================
#====== Global Variables (settings) ======

RUN_SERVER = True
SEND_GAME_LIST = False


#--card files
N_SIT  =  72-1  # number of situation cards
N_FILL = 184-1  # number of fill cards


#--Network connection
host = "0.0.0.0"
# host = "107.23.124.173"
# port = 11000
port = 10000
# port = 10500

if (SEND_GAME_LIST):
	LISTENER = socket.socket()
	LISTENER.bind((host, port))

#--Game Data
randID = random.randint(1, 999)
game_list = []
game_port = port
if (SEND_GAME_LIST) : game_port += 10

def nextGameID():
	global randID
	randID += 1
	return "gm_{:04d}".format(randID)


def waitConnection():
	'''
	listen for a client to connect, then accept connection
	Return connected socket with a 60 sec timeout
	return False if Exception occured
	'''
	try:
		print("..listening..", end=" ")
		LISTENER.listen(5)
		print(" .. ..")
		c, addr = LISTENER.accept()
		print("__accepted__")
	except Exception as ex:
		template = "An exception of type [{0}] occured.\nArguments:\n  {1!r}"
		ex_msg = template.format(type(ex).__name__, ex.args)
		print("--><--"*7)
		print(ex_msg)
		print("=-><-="*7)
		return False
	c.settimeout(60.0)
	print("timeout set to 60")
	return c


def my_send(sock, M):
	'''
	wrapper for socket's send function
	
	Currently just prints message before appending EOL
	Can be expanded to check syntax, etc
	'''
	print("<<< [{}]".format(M))
	
	if M == "":
		print("--< (sending empty message...)")
		M += "\n"
	elif M[-1] == "\n":
		print("--< already appended")
	else:
		M += "\n"
	sock.send(M.encode())  #socket_send
	return

#---First, generate some games to play

#-Create 'Anarchy' game
game_ID = nextGameID()
G = Game(game_port, host, game_ID, 5, 4, "A")
game_port += 10
game_list.append(G)
G.start()
time.sleep(1)

#-Create 'Anarchy' game
game_ID = nextGameID()
G = Game(game_port, host, game_ID, 5, 4, "A")
game_port += 10
game_list.append(G)
G.start()
time.sleep(1)

#-Create 'Anarchy' game
game_ID = nextGameID()
G = Game(game_port, host, game_ID, 5, 4, "A")
game_port += 10
game_list.append(G)
G.start()
time.sleep(1)

#-Create 'Democracy' game
game_ID = nextGameID()
G = Game(game_port, host, game_ID, 5, 4, "D")
game_port += 10
game_list.append(G)
G.start()
time.sleep(1)

#-Create 'Democracy' game
game_ID = nextGameID()
G = Game(game_port, host, game_ID, 5, 4, "D")
game_port += 10
game_list.append(G)
G.start()
time.sleep(1)

#-Create 'Democracy' game
game_ID = nextGameID()
G = Game(game_port, host, game_ID, 5, 4, "D")
game_port += 10
game_list.append(G)
G.start()
time.sleep(1)

print("\n--------------------------------------------------\n- Available Games:")
for i, G in enumerate(game_list):
	print(" {} : ".format(i+1) + G.getGameInfo())
	# print("---- " + G.socket.gethostbyname(gethostname()))
	# print("---- " + G.sock_addr)
print("--------------------------------------------------")

#---connect to client and send list of available games
if (SEND_GAME_LIST):
	# while RUN_SERVER:
	con_sock = waitConnection()
	if con_sock:
		for G in game_list:
			msg = "game: "
			msg += G.getGameInfo()
			my_send(con_sock, msg)
		print(".:.:. con_sock was")
	else:
		#-client never connected
		print("--no connection--")

#---Make sure all game threads have completed
print("__x_ joining [{}]x games in list...".format(str(len(game_list))))
for G in game_list:
	print("-x----G.join(.. ..", end=" ")
	G.join()
	print(".. ..)")

print("\n --!!--  ALL GAMES COMPLETE, ENDING PROGRAM  --!!--\n")

## end program

