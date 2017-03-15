import socket
import sys
import random
import re
from threading import Thread
'''
MainServer.py
this is the main thread for the server program for the fill in the blank mobile card game
This file contains the class definitions of the gameThread and PlayerThread,

the gameThread will be responsable for alerting the PlayerThreads of changes in the game
state, the PlayerThreads will be responsable for listening for players connecting,
getting and sending messages to the players, and essentially managing the player
'''


#a few globals....yuck
N_SIT  =  72-1  # number of situation cards
N_FILL = 184-1  # number of fill cards



'''
PlayerThread,
a threaded class, it will take in a portnum ipaddress adn a unique player id
it will then listen for a connection at the port number, and then will
get player messages and send them as indicated from the server

'''
class PlayerThread(Thread):
	def __init__(self, playerID, game, gamePhase):
		self.game = game
		Thread.__init__(self)#set up the thread with this class

		self.playerID = playerID
		self.gamePhase = -1
		self.connected = False
		self.dealtIn = False#has the player been dealt into the game yet?
		self.remove = False#tells the game to remove it when looking for old players,
		#we can't just dynamically remove them just in case the game is looping through the players, so we just mark it for removal and the game class
		#will remove when safe.

		self.gamePhase = gamePhase

		#this is a list of commands that hte client may send
		#and the appropriate functions to call when the thread recieves
		#these messages
		self.messageCallbacks = [
			("Join:",self.join_MessageHandler),
			("ply:",self.play_MessageHandler),
			("vt:",self.vote_MessageHandler),
			("msg:",self.im_MessageHandler),
			("bye",self.leave_MessageHandler)
			]


		self.playerName = "empty"#hold the player name
		self.profileIconID = -1#to hold the profile id num
		self.score = 0#to hold the players score

		self.playerHand = []#a list for the card the player has


		self.playedCard = False#when a player votes or plays a card we will use this to alert the client it has selected
		self.playedCardID=-1#when a player votes or plays we will store the index of the card here
	#end init____________

	#this function returns the boolean value indicating if the player has played a card and what that cards id is
	def getPlayerSelection(self):
		msg = str(self.playerID) + " " + str(self.playedCardID)
		return str(msg)

	#resets the playedCard and playedCardID to false and -1, meaning the player hasn't chosen a card to play yet.
	def resetPlayerSelection(self):
		self.playedCard = False
		self.playedCardID = -1

	#setter function for the gamephase
	def setGamePhase(self, newPhase):
		self.gamePhase = newPhase


	#this is a blocking function, it will wait for a client to connect
	#and then will set up the connection
	def waitForConnection(self):
		try:
			self.game.socket.listen(5)#start listening
			self.connection, self.address = self.game.socket.accept()#accept connection
			self.game.sock_addr = self.address
			self.connected = True#flag this thread as connected to client, this way the main thread will start another listener thread
			print("#" + self.playerID + ": connection")
		except Exception as ex:
			print("-><-- Connection error --><-")

	#END waitForConnection-----------

	'''
	sends and recieve a message through the socket
	'''
	def sendMessage(self, M):
		print(" << ({}) << [{}]".format(self.playerID, M))
		if M == "":
			print("--< (sending empty message...)")
			M += "\n"
		elif M[-1] == "\n":
			print("--< already appended")
		else:
			M += "\n"
		if(self.connected==True):
			self.connection.send(M.encode())
		else:
			print("--!!-- ({}) disconnected!".format(self.playerID))
			print("--  " + self.game.ID)



	def recieveMessage(self):
		message = ""
		if (self.connected==True):
			try:
				message = self.connection.recv(512).decode()
				if not message:
					print(self.playerID+": disconnect")
					self.remove = True#tell the game it needs to be removed
					print("-//-marked for removal: " + self.playerID)
					
					self.connected = False#set connected to false so the game won't wait for it's input.
					self.connection.close()#close the socket connection
					self.game.playerLeft(self)

			except Exception as ex:
				self.connected = False
				self.remove = True
				print("-//-marked for removal: " + self.playerID)
				
				self.connection.close()
				self.game.playerLeft(self)
				# print("recieve message error")
				template = "An exception of type [{0}] occured.\nArguments:\n  {1!r}"
				ex_msg = template.format(type(ex).__name__, ex.args)
				print("--><--"*7)
				print(ex_msg)
				print("=-><-="*7)
				message = "---EXCEPTION---"
		message = message.rstrip("\n")
		print(" >> ({}) >> [{}]".format(self.playerID, message))
		return message

	def hasSelection(self):
		return True
	def incrementScore(self, N=1):
		self.score += N

	'''
	functions to add and remove cards from the players hand, and get the number of cards
	'''
	def addCard(self, cardId):
		self.playerHand.append(cardId)
	def removeCard(self, cardId):
		self.playerHand.remove(cardId)
	def numCardsInHand(self):
		return len(self.playerHand)

	#callback for a join message
	#message comes in as: Join: playerName playerProfileIconID
	def join_MessageHandler(self, words):
		print("#" + self.playerID + ": processing join message")
		self.playerName = words[1]#assign the player name
		self.profileIconID = words[2]#assign the profileIconID
		self.game.newPlayerConnected(self)
		self.game.dealOutCards()
		self.sendMessage("JoinAck: "+self.playerID)
		self.sendMessage(self.game.getUpToDateMessage())
        

	#callback for the vote message
	#message template: ply: cardID
	def play_MessageHandler(self,words):
		print("#" + self.playerID + ": processing play message")
		#if the gamePhase is 1 then we are expecting a play message, else we are expecting a vote message
		if (self.gamePhase !=0):
			#print error if gamePhase does not equal 1,
			print("#" + self.playerID + ": error, processing play message when vote message was expected")
		else:
			#set the playCardID to the next word and set played card to true
			self.playedCardID = int(words[1])
			self.playedCard = True
			for i in range(0,len(self.playerHand)):
				if self.playerHand[i] == self.playedCardID:
					del self.playerHand[i]
					break

	#callback for vote message
	#message template: vt: cardID
	def vote_MessageHandler(self, words):
		print("#" + self.playerID + ": processing vote message")
		#if the gamephase is 2 then we are expecting a vote message,
		#otherwise we are expecting a play message, so if the phase isn't 2 print an error,
		#otherwise set playCardID to the 2nd word and set playedCard to true
		if (self.gamePhase != 1):
			print("#" + self.playerID + ": error, processing vote message when play message was expected")
		else:
			self.playedCardID = (words[1])
			self.playedCard = True
			

	#this is just so we can have message functionality at a later date if we choose to implement it
	#right now it does nothing,
	def im_MessageHandler(self, words):
		print("#" + self.playerID + ": processing im message")

	#function callback for when a client sends a disconnect message,
	#it will disconnect the socket and set connected to false
	#FUNCTIONALITY TBA
	def leave_MessageHandler(self, words):
		print("#" + self.playerID + ": processing leave message")
		self.connected = False
		self.connection.close()
		self.remove = True
		print("-//-marked for removal: " + self.playerID)
		
		self.game.playerLeft(self)


	'''
	this function will get a a string containing the player information
	this is to send it to the other players when a player joins
	'''
	def toString(self):
		playerInfoString = "nwPlyr: "+self.playerName+" "+str(self.profileIconID)+" "+str(self.score)+ " "+self.playerID
		return playerInfoString
	#end toString_________


	'''
	this will continually recieve messages from the client and process them using the function callbacks
	in the messageCallbacks list,
	'''
	def recieveMessageLoop(self):
		print("#" + self.playerID + ": starting message recieve loop")
		while self.connected == True:#loop whiel the client is still connected
			message = self.recieveMessage()#get the message
			words = message.split(" ")#split the message up into words
			for i in range(0,len(self.messageCallbacks)):#loop through the registered callbacks and compare them to the message headers
				if (words[0] == self.messageCallbacks[i][0]):#if we find a header that matches we call that callback
					self.messageCallbacks[i][1](words)
					break#and break, since we have found the right message, no need to look anymore

			if self.gamePhase == 9:
				#-winner has been declared, time to end the thread
				return

	#this function will be the "main" function for this thread
	#it listens for a connection through the waitForConnection method
	#once a connection is established it will enter the recieveMessageLoop which will recieve and process messages from the client
	def run(self):
		while self.connected == False:
			self.waitForConnection()
		self.recieveMessageLoop()

#end player class_______
