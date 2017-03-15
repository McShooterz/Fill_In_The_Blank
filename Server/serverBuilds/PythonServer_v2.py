#! /usr/bin/env python
#-----------------------------------------
#	PythonServer_v2.py
#	cs340 team05		spring 2016
#	Mike Shoots
#	Travis Brown
#	Tyler Poff
#-----------------------------------------
#	This is the server for the "Fill in the ______" game
#	for cs340 Software Engineering group project.
#------------
#	This is v2, using the second iteration of our
#	communication protocol. This protocol aims to
#	create more "checkpoints" to help improve
#	consistency of game states between the client
#	and server.
#-----------------------------------------
import socket
import sys
import random
import re

#=========================================
#====== Global Variables (settings) ======

#--card files
N_SIT  =  72-1  # number of situation cards
N_FILL = 184-1  # number of fill cards

#--other options
NETWORKING = False   #--currently not implemented
SOLO_TEST  = False   # set True when testing without client  (incomplete)
SHOW_COMM  = True    # echo ALL communication to console
TESTING    = False    # used for testing options

OTHER   = True
OPTIONS = True
GO_HERE = True

#------------ end of global variables ---------------------


#=========================================
#=============    Classes    =============

class Player:
	def __init__(self, name, score, picId, playerid):
		self.name = name
		self.picId = picId
		self.id = playerid
		self.score = score
		self.resetChoices()
		self.resetHand()
		
	def resetChoices(self):
		self.playedCard = -1
		self.votedCard = -1
		
	def resetHand(self):
		self.cards = []
		self.numCards = len(self.cards)
		
	def resetAll(self):
		self.resetHand()
		self.score = 0
		
	def nwPlyrString(self):
		botInfo = "nwPlyr: " + self.name + " " + str(self.picId) + " " + str(self.score) + " " + str(self.id)
		return botInfo
		
	def addCard(self, cardId):
		self.cards.append(cardId)
		self.numCards = len(self.cards)
		
	def removeCard(self, cardId):
		self.cards.remove(cardId)
		self.numCards = len(self.cards)
		
	def hasCard(self, cardId):
		'''UNUSED'''
		count = self.cards.count(cardId)
		if coutn == 0:
			return False
		else:
			return True
			
	def __str__(self):
		R = "{:2d}  {}".format(self.score, self.name)
		return R
	
#------------ end of Player() class -----------------------

class Game:
	def __init__(self):
		self.aiPlayerNames = ["Lenny", "Carl", "Billy", "Steve Jobs", "Bill Gates", "Linus", "Charlie", "Snoopy"]
		# self.numPicIds = 5
		self.CardsInUse = [False] * (N_FILL+1)
		self.handSize = 5
		if (TESTING) : self.scoreLimit = 3
		else : self.scoreLimit = 5
		self.winner = False
		self.players = []

		#-add AI players
		self.numPlayers = 4
		# self.numPlayers = random.randint(3,5)
		# print("#^#^#^#^# Let's play a game with {}x dummies".format(self.numPlayers))
		self.initializeAiPlayers(self.numPlayers-1)

		#-set up socket (single connection for 1p vs AI)
		# '''
		self.s = socket.socket()
		host = socket.gethostname()
		port = 11000
		self.s.bind((host, port))
		# '''

	#-----------------------------------------
	#-  Connection functions
		
	def my_send(self, M):
		'''
		wrapper for socket's send function
		
		Currently just prints message before appending EOL
		Can be expanded to check syntax, etc
		'''
		if (SHOW_COMM) : print("<<< [{}]".format(M))
		if M[-1] == "\n":
			print("--< already appended")
		else:
			M += "\n"
		self.c.send(M.encode())  #socket_send
		return
		
	def my_recv(self):
		'''
		wrapper for socket's recv function
		
		allows to check for and handle important errors,
		currently used for 'timed out'
		'''
		try:
			message = self.c.recv(512).decode()
		except Exception as ex:
			template = "An exception of type [{0}] occured.\nArguments:\n  {1!r}"
			ex_msg = template.format(type(ex).__name__, ex.args)
			print("--><--"*7)
			print(ex_msg)
			print("=-><-="*7)
			print("\n--  End Program   --")
			sys.exit()
		message = message.rstrip("\n")
		if (SHOW_COMM) : print(">>> [{}]".format(message))
		return message
	
	def waitConnection(self):
		if (TESTING) : self.s.settimeout(60.0)
		try:
			self.s.listen(5)
			self.c, self.addr = self.s.accept()
		except Exception as ex:
			template = "An exception of type [{0}] occured.\nArguments:\n  {1!r}"
			ex_msg = template.format(type(ex).__name__, ex.args)
			print("--><--"*7)
			print(ex_msg)
			print("=-><-="*7)
			print("\n--  End Program   --")
			sys.exit()
		if (TESTING) : self.c.settimeout(20.0)
		else : self.c.settimeout(120.0)
		return True

	def playerJoin(self):
		'''
		recieve playerInfo from client, acknowledge and add to game
		
		Handle intial player-server interaction as follows:
		1.  client sends the server their player info
		2.  the server acknowledges, sends (current game status, id, timer info)   
		  (?id=plyrId?)
		'''
		if SOLO_TEST:
			playerData = input(" >> playerData: ")
		else:
			playerData = self.my_recv()
		print("player Info: ", playerData)

		#-now acknowledge the join
		joinAck = "JoinAck: lobbyName RDY 3 -1 -1"
		self.my_send(joinAck)
		self.players.append(Player("realboy", 0, 0, 3))

		return playerData

	#-----------------------------------------
	#-  Game: Driver functions

	def Connect_Single(self):
		'''intialize connections for single player mode'''
		print("-o-"*15)
		
		#-connect to Player1
		print("Waiting for a player to connect\n  ...")
		game.waitConnection()
		print("Ha! Caught one!")

		#-add Player1 to the game
		game.playerJoin()

		#-tell Player1 who else is at the table
		print(" .. sending other player info")
		game.sendOtherPlayerInfo()
		print("... info sent\n")		
		print("-o-"*15)
		return

	def New_Game(self):
		'''intialize a new game'''
		print("=-"*39)
		print("=--=|    N E W  G A M E    |=--=")
		print("=-"*16)
		
		for P in self.players:
			P.resetAll()
		for C in self.CardsInUse:
			C = False
		self.Connect_Single()
		return

	def Play_Game(self):
		'''Play through an instance of a game'''
		while True:
			game.replaceCards()      #  clnt -- srvr
			# print("changing mode to play mode")
			print("------mode changed:", end=" ") # --> PLAY mode
			game.newRound()          #  clnt << srvr

			game.getPlayedCards()    #  clnt >> srvr
			game.getAiPlayedCards()  #  clnt -- srvr
			print(" >>> got card from player")

			print("------mode changed:", end=" ") # --> VOTE mode
			game.votePhase()         #  clnt << srvr
			game.getVotes()     #  clnt >> srvr
			game.getAiVotes()   #  clnt -- srvr

			print("~~~Votes recieved, counting ... ")
			game.tallyVotes()        #  clnt -- srvr
			game.voteResults()       #  clnt << srvr
			
			# '''
			if game.isWinner():
				break
			else:
				continue
			# '''
		return

	#-----------------------------------------
	#-  Game: Overhead/general

	def initializeAiPlayers(self, numAi):
		for i in range(numAi):
			print("++++ adding AI player #{}".format(i+1))
			score = 0
			picId = i
			idNum = i
			name = random.choice(self.aiPlayerNames)
			self.aiPlayerNames.remove(name)
			p = Player(name, score, picId, idNum)
			self.players.append(p)
		return

	def sendOtherPlayerInfo(self):
		'''Send the other players' information to the client'''
		
		#-(AI testing) send player info for the bots
		for i in range(3):
			info = self.players[i].nwPlyrString()
			self.my_send(info)
		return

	def dealCards(self, plyr, numCardsDeal, send):
		'''Deal a specified number of cards to the player'''
		for i in range(numCardsDeal):
			cardId = random.randint(0, N_FILL)
			while self.CardsInUse[cardId]:
				cardId = random.randint(0, N_FILL)
			self.CardsInUse[cardId] = True
			plyr.addCard(cardId)
			if send == True:
				playerCardInfo = "nwCrd: " + str(cardId)
				self.my_send(playerCardInfo)
		return

	def replaceCards(self):
		for i in range(4):
			if self.players[i].numCards < self.handSize:
				numCards = self.handSize - self.players[i].numCards
				if (i == 3):
					#-DO send player's cards
					self.dealCards(self.players[i], numCards, True)
				else:
					#-do NOT send AI's cards
					self.dealCards(self.players[i], numCards, False)
		return

	def isWinner(self):
		msg = "winner:"
		n_winners = 0
		winner_list = []
		for i in range(4):
			if self.players[i].score >= self.scoreLimit:
				msg += " " + str(i)
				n_winners += 1
				winner_list.append(self.players[i])

		if n_winners > 0:
			self.my_send(msg)
			print("====Winners!~!!" + "="*42)
			for P in winner_list:
				print("-+---> [" + str(P) + "]")
			return True
		return False

	#-----------------------------------------
	#-  Game: Client interaction

	def newRound(self):
		'''Declare new round and send new situation card'''
		print("PLAY------------------")
		phaseChange = "chng: playPhase " + str(random.randint(0, N_SIT))
		self.my_send(phaseChange)
		for i in range(4):
			self.players[i].resetChoices()
		return

	def getPlayedCards(self):
		if SOLO_TEST:
			playerCard = input(" >> playerCard: ")
		else:
			playerCard = self.my_recv()

		choice = int(re.search(r'\d+', playerCard).group())
		self.CardsInUse[choice] = 0
		self.players[3].playedCard = choice
		self.players[3].removeCard(choice)
		return

	def getAiPlayedCards(self):
		for i in range(3):
			choice = random.choice(self.players[i].cards)
			self.players[i].playedCard = choice
			self.players[i].removeCard(choice)
		return


	def votePhase(self):
		print("VOTE------------------")
		msg = "chng: vtCrd:"
		for i in range(4):
			msg += " " + str(self.players[i].playedCard)
		self.my_send(msg)
		return

	def getVotes(self):
		if SOLO_TEST:
			playerVote = input(" >> playerVote: ")
		else:
			playerVote = self.my_recv()
		
		choice = int(re.search(r'\d+', playerVote).group())
		self.players[3].votedCard = choice
		print("  player voted for: #{}".format(choice))
		return

	def getAiVotes(self):
		for i in range(3):
			choice = random.randint(0, 3)
			#-Can't vote for yourself
			while choice == i:
				choice = random.randint(0, 3)
			self.players[i].votedCard = choice
		return

	def tallyVotes(self):
		for i in range(4):
			v = self.players[i].votedCard
			print("  AI_player ({}) voted for: {}".format(i, v))
			self.players[v].score += 1
		return

	def voteResults(self):
		msg = "chng: vtRslt:"
		print("********************\n*  current score:\n*")
		for i in range(4):
			print(self.players[i])
			msg += " " + str(self.players[i].score)
		print("*\n********************")
		self.my_send(msg)
		return
	
#------------ end of Game() class -------------------------


#=========================================
#===========    begin MAIN    ============

#-----------------------------------------
#-  Connection/"Matchmaking"

#-initialize Game object
game = Game()
print("Game intialized!")

# game.Connect_Single()

#-----------------------------------------
#-  Play game

run_server = True
while run_server:
	# game.Connect_Single()
	game.New_Game()
	game.Play_Game()
	print("\n--  End of Game   --\n")

print("\n --!!--  run_server = False  --!!--\n")
