import socket
import sys
import random
import re
from threading import Thread
from PlayerThread import PlayerThread
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
an ai player class for testing
has the same api as the playerThread class, so it's treated teh same in the gamecode
but it doesn't run in a thread,
randomly selects
'''
class AiPlayer:
	def __init__(self, aiNum, game):
		self.playerHand = []
		self.playerName = "ai"+str(aiNum)
		self.profileIconID = aiNum
		self.score =0
		# self.playerID = str(aiNum)
		self.playerID = "AI_" + str(aiNum)
		self.gamePhase =1
		self.connected=True
		self.playedCard = True
		self.playedCardID = -1
		self.game = game
		self.remove = False
	#this function returns the boolean value indicating if the player has played a card and what that cards id is
	#for the ai it's a random play card, for voting it's always the first player(which will always be an ai)
	#this function returns the boolean value indicating if the player has played a card and what that cards id is
	#this function returns the boolean value indicating if the player has played a card and what that cards id is
	def getPlayerSelection(self):
		msg = str(self.playerID)+ " "+str(self.playedCardID)
		if (self.gamePhase ==1):
			for i in range(0,len(self.playerHand)):
				if self.playerHand[i]==self.playedCardID:
					del self.playerHand[i]
					break
		return str(msg)
	#this function doens't do anything, empty, but we still need it
	#so we can use the same mechanics as we do for normal players
	def resetPlayerSelection(self):
		self.temp=2
	#setter function for the gamephase,
	def setGamePhase(self, newPhase):
		self.gamePhase = newPhase
		if (newPhase == 0):#reset for cards in his hand            
			self.playedCardID = self.playerHand[random.randint(0,len(self.playerHand)-1)]
		else:#if it's the vote phase then we select a random player to vote for
			foundPlayer = False
			while (foundPlayer==False):
				playerIndex = random.randint (0, len(self.game.playerThreads)-1)
				foundPlayer = self.game.playerThreads[playerIndex].connected
				
			self.playedCardID = str(self.game.playerThreads[playerIndex].playerID)

	#a dummy function for sending a message,
	#created so that the ai class has the same interface as the player class
	#meaning it can be treated as a player
	def sendMessage(self, message):
		self.temp=1
	def hasSelection(self):
		return True
	def incrementScore(self):
		self.score+=1
	
	def join(self):
		#-need to mimic the Threaded class PlayerThread
		return True
	
	'''
	functions to add and remove cards from the players hand, and get the number of cards
	'''
	def addCard(self, cardId):
		self.playerHand.append(cardId)		
	def removeCard(self, cardId):
		self.playerHand.remove(cardId)
	def numCardsInHand(self):
		return len(self.playerHand)
	def start(self):
		self.temp =1
		
	'''
	this function will get a a string containing the player information
	this is to send it to the other players when a player joins
	'''
	def toString(self):
		playerInfoString = "nwPlyr: "+self.playerName+" "+str(self.profileIconID)+" "+str(self.score)+ " "+self.playerID
		return playerInfoString
	#end toString_________
