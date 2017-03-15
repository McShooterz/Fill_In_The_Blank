import socket
import sys
import random
import re
import time
from threading import Thread
from PlayerThread import PlayerThread
from AiPlayer import AiPlayer
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



#-initialize for timestamps
time.clock()
# time.sleep(1)

		
'''
this class will handle the player threads, set them up
and handle thread communication as well as set the overall game state
'''
class Game(Thread):
	def __init__(self, portID, ipAddress, gameID, maxPlayers, numAI, mode="A"):
		Thread.__init__(self) #set up the thread for this class

		#-create unique ID
		print("=-"*39)
		print("=--=|    N E W  G A M E    |=--=  " + gameID)
		print("=-"*16)
		
		#-Game Properties/Options (don't change)
		self.ID = gameID
		self.maxPlayers = maxPlayers # MAX number of players (including AI dummies)
		self.numAI = numAI           # set the number of AI players
		self.pointThreshold = 5
		self.playerWon = False       # Flag to indicate a winner
		self.handSize = 5
		self.score_mode = mode       # A-Anarchy, D-Democracy, M-Monarchy
		
		#-current game info (will change throughout game)
		self.gamePhase = 0
		self.upToDateMessage=""
		self.playerThreads = []      # list of (human) players

		#set an array of the number of play cards in use,
		self.cardsInUse = [False] * (N_FILL+1)
		self.situationCardID = 0

		#set up any AI players
		for i in range(0, numAI):
			aiP = AiPlayer(str(i), self)#create a new ai class instance
			self.playerThreads.append(aiP)#add to list.
		
		#-Network connections
		self.portID = portID
		self.ipAddress = ipAddress
		self.socket = socket.socket()
		self.socket.bind((ipAddress, portID))
		self.sock_addr = ""


	#-tells MatchMaker how many more players can fit in the game
	def needs_players(self):
		N = len(self.playerThreads)
		return (self.maxPlayers - N)


	#-Called by main thread to get info about the Game
	def getGameInfo(self):
		'''game: lobbyName portNum playerCount'''
		R = ""
		R += self.ID + " " + str(self.portID)
		R += " " + str(len(self.playerThreads))
		return R


	#this function will act as the main function for the game thread
	def run(self):
		self.setupNewPlayerListener()
		self.gameLoop()
		self.reaper()
		print("-x-x-x- reaper() complete ................")


	#-Make sure "children" threads have completed before returning to main
	def reaper(self):
		time.sleep(3)
		print("-x-x-x- begin reaping: ({})x PlayerThreads:".format(len(self.playerThreads)))
		for P in self.playerThreads:
			print("  -x- " + P.playerID)
		print("----------------------------")
		for P in self.playerThreads:
			print("-x--{}--P.join(.. ..".format(P.playerID), end=" ")
			P.join()
			time.sleep(1)
			print(".. ..)")

	#-Main Loop, drives gameplay
	def gameLoop(self):
		print("starting game loop")
		for Player in self.playerThreads:
			print(Player.toString())
		while(self.playerWon == False):
			if(self.playersPresent() == True):
				print("*************  start round  *************")
				print("*                                       *")
				
				#--? perhaps add a check_all_players_ready function ??
				#--...or should be taken care of in playersPresent()
				
				self.dealOutCards()
				self.removeLeftPlayers()
				self.playPhase()
				self.removeLeftPlayers()
				self.votePhase()
				self.removeLeftPlayers()

				self.resultPhase()
				self.removeLeftPlayers()
				self.checkHasWinner()
				self.removeLeftPlayers()
				print("*                                       *")
				print("**************  end round  **************")

				# check if another player is wanted
				#---ToDo: implement minimum players to keep playing; keep the game going, but allow MAIN thread to know we want another player
				# self.setupNewPlayerListener()
				# ^^ commented out until disconnects/player removal is working properly
		self.winnerPhase()

	def playersPresent(self):
		#--ToDo: use this function to enforce minimum players
		for i in range(self.numAI, len(self.playerThreads)):
			if (self.playerThreads[i].connected == True):
				return True
		return False

	def removeLeftPlayers(self):
		#-This function is dangerous because we're removing items from a list AS we iterate through it
		
		'''
		for player in self.playerThreads:
		for i in range(0, len(self.playerThreads)):
			if self.playerThreads[i].remove == True:
				del self.playerThreads[i]
		'''
		for player in self.playerThreads:
			if player.remove == True:
				print("--X--Get outta here!: " + player.playerID)
				#del player
				break
		# '''
	def getUpToDateMessage(self):
		return self.upToDateMessage
	#function for dealing with the voting phase
	#sends out the cards for the player to vote for, and waits for a selection
	def votePhase(self):
		print("------mode changed:  VOTE phase")

		#construct the vote phase message,
		#it has the chng: votephase, then a list of player ids followed by their selection
		msg = "votePhase:"
		for Player in self.playerThreads:
			if (Player.connected == True):
				msg = msg + " " + Player.getPlayerSelection()
				playerSelection = Player.playedCardID
				# print(playerSelection)
				self.cardsInUse[int(playerSelection)] = False

		self.sendMessageToAllPlayers(msg) #-send the message out to all players
		# self.upToDateMessage = msg
		self.setAllPlayerGameState(1)     #-set all the players gamestate to 1 indicating vote phase
		self.waitForPlayerSelection()


	#-Process votes
	def resultPhase(self):
		print("------mode changed:  RESULTS")
		if self.score_mode == "A":
			self.result_A()
		if self.score_mode == "D":
			self.result_D()

	#-Process votes ('Anarchy' mode)
	def result_A(self):
		#get the players selections
		#first distribute the points
		for Player_i in self.playerThreads:

			if (Player_i.connected == True):
				#get this players result
				vote = str(Player_i.playedCardID)
				print("player: " + Player_i.playerID + " voted for: " + Player_i.playedCardID)
				found = False
				#now loop through the players and give them their points
				for Player_j in self.playerThreads:
					if ((Player_j.playerID) == str(vote)):#did the player vote for this person?
						Player_j.incrementScore()#increment their score
						found = True
						break
				if (found == False):
					print("--!--could not find player")

		msg = "vtRslt:"
		for Player in self.playerThreads:
			if (Player.connected ==True):
				msg += " " + str(Player.playerID) + " " + str(Player.score)
		#send the vote results to the players
		self.sendMessageToAllPlayers(msg)


	#-Process votes ('Democracy' mode)
	def result_D(self):
		ballotBox = {}  #-key: cardID, val: count
		
		#-collect/tally votes
		for Player_i in self.playerThreads:
			if (Player_i.connected == True):
				#get this players result
				vote = str(Player_i.playedCardID)
				print("player: " + Player_i.playerID + " voted for: " + Player_i.playedCardID)
				if vote in ballotBox.keys():
					ballotBox[vote] += 1
				else:
					ballotBox[vote] = 1
				
		
		#-Determine highest vote count
		biggest_vote = 0
		big_winner = []  #-use list in case of ties
		# for k, v in ballotBox:
		for k in ballotBox.keys():
			v = ballotBox[k]
			if v > biggest_vote:
				biggest_vote = v
				big_winner = []       #-reset the list
				big_winner.append(k)  #-add the new biggest winner
			elif v == biggest_vote:
				big_winner.append(k)  #-add the new biggest winner
			else:
				continue

		#-Award the point to the highest voted card
		for Player_i in self.playerThreads:
			if ((Player_i.playerID) == str(vote)): #did the player vote for this person?
				Player_i.incrementScore()  #-increment their score
				Player_i.incrementScore()  #--Again because this mode is so slow...

		msg = "vtRslt:"
		for Player in self.playerThreads:
			if (Player.connected ==True):
				msg += " " + str(Player.playerID) + " " + str(Player.score)
		#send the vote results to the players
		self.sendMessageToAllPlayers(msg)
		

		
	#-check if a player has reached the pointThreshold
	def checkHasWinner(self):
		for Player in self.playerThreads:
			if Player.score >= self.pointThreshold:
				self.playerWon = True
				print("  ************************")
				print("  * *      WINNER      * *")
				print("  ************************")

	#-At least one player has reached threshold, determine winner and send to clients
	def winnerPhase(self):
		#-inform all playerThreads that the game is over, and it's time to go home
		print("  game is over, and it's time to go home")
		self.setAllPlayerGameState(9)
		msg = "winner:"
		for Player in self.playerThreads:
			if (Player.score >= self.pointThreshold):
				msg += " " + Player.playerID
		print("!WW! -- [{}]".format(msg))
		self.sendMessageToAllPlayers(msg)
		# self.upToDateMessage = msg

	#function to handle the play phase of the game
	def playPhase(self):
		print("------mode changed:  PLAY")
		#first send a message to all the other players that there is a new play phase
		#get a random situation card
		self.situationCardID = str(random.randint(0, N_SIT))
		msg = "playPhase: " + self.situationCardID
		self.sendMessageToAllPlayers(msg)
		# self.upToDateMessage = msg
		self.setAllPlayerGameState(0)

		#wait until all players have played a card
		self.waitForPlayerSelection()

		#at this point every player has a card selected(or should have)
		#so now we switch to the vote phase

		print("all players played card")


	#function to deal out the cards to the players every round
	def dealOutCards(self):
		print("-.-.- dealing out cards -.-.-")
		for Player in self.playerThreads:
			Player.dealtIn = True
			if (Player.connected == True):
				#check if this player has enough cards, if not give them cards
				self.dealOutCardsToPlayer(Player)
	def dealOutCardsToPlayer(self, Player):
		while (Player.numCardsInHand() < self.handSize):
				cardID = self.dealRandomPlayCard()#get a random card
				Player.addCard(cardID)#add that card to the players hand
				Player.sendMessage("nwCrd: " + str(cardID))#send a message to the player indicating the new card

	def dealRandomPlayCard(self):
		cardId = random.randint(0, N_FILL)
		while (self.cardsInUse[cardId] == True):
			cardId = random.randint(0, N_FILL)
		self.cardsInUse[cardId] = True
		return cardId

	#-Set up a new player object and start it's thread,
	def setupNewPlayerListener(self):
		#-First, make sure we aren't already full:
		L = len(self.playerThreads)
		N = self.maxPlayers
		if L < N:
			#-the game has fewer than max players, so print message and continue
			print(" [[[ {}  <  {} ]]]".format(L, N))
		#ATTENTION: until further notice, all games can have an infinite number of players
		#this is to avoid unnecisarry bugs that currently arise if we stop listening for more players,
		#after we hit the maximum number. (what happens when one leaves? we don't start another listener...)
		#we can discuss this after the presentation on Wendesday, until then leave this function as is.
	
		p = PlayerThread("pl_" + str(len(self.playerThreads)-1), self, self.gamePhase)
		self.playerThreads.append(p)
		print("--#-#-#--  WELCOME NEW PLAYER  --#-#-#--  :  " + p.playerID)
		self.playerThreads[len(self.playerThreads)-1].start()

	#this function(horribly named) will send player join messages for all existing players
	#and send player join messages to the new player as well.
	def newPlayerConnected(self, newPlayer):
		for Player in self.playerThreads:
			if (Player.playerID != newPlayer.playerID):#make sure this isn't the new player
				if (Player.connected == True):
					#send a join player message to the existing player
					Player.sendMessage(newPlayer.toString())
					#send joing player message to new player
					newPlayer.sendMessage(Player.toString())
		self.setupNewPlayerListener()


	#sends a message to all players telling them a player has left.
	def playerLeft(self, player):
		for p in self.playerThreads:
			if (p.connected==True):
				p.sendMessage("playerLeave: "+player.playerID)

	#a function that won't return until all players have selected a card,
	#if we wanted to implement a timer, here's where we would do it.
	def waitForPlayerSelection(self):
		while True:
			if (self.hasAllPlayersSelectedCard()==True):
				break

	#helper function that returns true if all players have played a card,
	#false otherwise
	def hasAllPlayersSelectedCard(self):
		for Player in self.playerThreads:
			if (Player.connected == True):
				if (Player.playedCard == False):
					return False
		return True

	#a helper function to set all players gamestate
	def setAllPlayerGameState(self, newState):
		self.gamePhase = newState
		for Player in self.playerThreads:
			#i'm making an assumption that if we are changing the game state we also want to reset the
			#players selections,
			Player.resetPlayerSelection()
			Player.setGamePhase(newState)

	#this function sends a uniform message to all players
	def sendMessageToAllPlayers(self, msg):
		self.upToDateMessage = msg
		for Player in self.playerThreads:
			if (Player.connected == True):
				Player.sendMessage(msg)

# host = "0.0.0.0"
# port = 11100
# G = Game(port, host, 5, 4, 5)
# G.start()
