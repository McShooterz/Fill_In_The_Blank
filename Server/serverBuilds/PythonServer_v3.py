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

#=========================================
#====== Global Variables (settings) ======

#--card files
N_SIT  =  72-1  # number of situation cards
N_FILL = 184-1  # number of fill cards

#--other options
NETWORKING = False   #--currently not implemented
SOLO_TEST  = False   # set True when testing without client  (incomplete)
SHOW_COMM  = True    # echo ALL communication to console
TESTING    = True    # used for testing options

aiPlayerNames = ["Lenny", "Carl", "Billy", "Steve_Jobs", "Bill_Gates", "Linus", "Charlie", "Snoopy"]

OTHER   = True
OPTIONS = True
GO_HERE = True

#------------ end of global variables ---------------------



#----| RegEx for parsing Client messages
RE_Join = re.compile(r'Join: (\w+) (\d+)')




#=========================================
#=============    Classes    =============

class Player:
	def __init__(self, name, score, picId, playerid):
		self.name = name
		self.score = score
		self.picId = picId
		self.id = playerid
		
		self.isAI = False
		self.resetChoices()
		self.resetHand()
		
	def resetChoices(self):
		self.playedCard = -1
		self.votedCard = -1
		
	def resetHand(self):
		self.cards = []
		
	def numCards(self):
		return len(self.cards)
		
	def resetAll(self):
		self.resetHand()
		self.score = 0
		
	def addCard(self, cardId):
		self.cards.append(cardId)
		
	def removeCard(self, cardId):
		self.cards.remove(cardId)
		
	def nwPlyrString(self):
		'''create string to send new player message to Client'''
		botInfo = "nwPlyr: " + self.name + " " + str(self.picId) + " " + str(self.score) + " " + str(self.id)
		return botInfo
			
	def __str__(self):
		'''printing format used for testing/debugging'''
		R = "{:2d}  {}".format(self.score, self.name)
		return R
	
#------------ end of Player() class -----------------------

class Game:
	def __init__(self):
		# self.numPicIds = 5
		# self.winner = False
		self.CardsInUse = [False] * (N_FILL+1)
		self.handSize = 5
		if (TESTING) : self.scoreLimit = 5
		else : self.scoreLimit = 5
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
		
	def my_recv(self, flag=0):
		'''
		wrapper for socket's recv function
		
		allows to check for and handle important errors,
		currently handles 'timed out' errors by exiting program
		future: will use 'flag' to indicate an error (1) or No Error (0)
		
		'''
		try:
			message = self.c.recv(512).decode()
		except Exception as ex:
			template = "An exception of type [{0}] occured.\nArguments:\n  {1!r}"
			ex_msg = template.format(type(ex).__name__, ex.args)
			print("--><--"*7)
			print(ex_msg)
			print("=-><-="*7)
			flag += 1  #-must CHANGE value, a re-assigning a value will create a different variable (yay Python)  (-!!-)
			print("\n--  End Program   --")
			if (TESTING) : sys.exit()
			else : message = "---TIMED_OUT---"
		message = message.rstrip("\n")
		if (SHOW_COMM) : print(">>> [{}]".format(message))
		return message
	
	def waitConnection(self):
		'''listen for a client to connect, then accept connection'''
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
			error_flag = 0
			playerData = self.my_recv(error_flag)
		print("player Info: ", playerData)

		#-process player data from client and add to players[] list
		matchObj = RE_Join.match(playerData)
		player_name = matchObj.group(1)
		pic_id = matchObj.group(2)
		# self.players.append(Player("realboy", 0, 0, 3))
		self.players.append(Player(player_name, 0, int(pic_id)+1, 3))
		
		#-send confirmation to client to acknowledge the join
		joinAck = "JoinAck: lobbyName RDY 3 -1 -1"
		self.my_send(joinAck)

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
		if not game.playerJoin():
			#-Player did not connect
			return False
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

		#-tell Player1 who else is at the table
		print(" .. sending other player info")
		game.sendOtherPlayerInfo()
		print("... info sent\n")		
		print("-o-"*15)
		return

	def Play_Game(self):
		'''Play through an instance of a game'''
		while True:
			game.replaceCards()      #  clnt >> srvr

			#--> PLAY mode
			print("------mode changed:", end=" ")
			game.newRound()          #  clnt << srvr
			game.getPlayedCards()    #  clnt >> srvr
			print(" >>> got card from player")

			#--> VOTE mode
			print("------mode changed:", end=" ")
			game.votePhase()    #  clnt << srvr
			game.getVotes()     #  clnt >> srvr

			print("~~~Votes recieved, counting ... ")
			game.voteResults()  #  clnt << srvr
			
			if game.isWinner():
				break
			else:
				continue
		return

	#-----------------------------------------
	#-  Game: Overhead/general

	def initializeAiPlayers(self, numAi):
		'''create a specified quantity of AI players and add to players[] list'''
		for i in range(numAi):
			print("++++ adding AI player #{}".format(i+1))
			score = 0
			picId = i
			idNum = i
			name = random.choice(aiPlayerNames)
			aiPlayerNames.remove(name)
			p = Player(name, score, picId, idNum)
			p.isAI = True
			self.players.append(p)
		return

	def sendOtherPlayerInfo(self):
		'''Send the other players' information to the client'''
		
		#-(AI testing) send player info for the bots
		for i in range(3):
			info = self.players[i].nwPlyrString()
			self.my_send(info)    # my_send--specific
		return

	def replaceCards(self):
		'''replenish all players' hands'''
		for i in range(4):
			player_N_cards = self.players[i].numCards()
			if player_N_cards < self.handSize:
				numCards = self.handSize - player_N_cards
				self.dealCards(self.players[i], numCards, True)    # my_send--specific
				'''
				if (i == 3):
					#-DO send player's cards
					self.dealCards(self.players[i], numCards, True)    # my_send--specific
				else:
					#-do NOT send AI's cards
					self.dealCards(self.players[i], numCards, False)
				'''
		return

	def dealCards(self, plyr, numCardsDeal, send):
		'''Deal a specified number of cards to the player'''
		if plyr.isAI:
			send = False
		else:
			send = True
		for i in range(numCardsDeal):
			cardId = random.randint(0, N_FILL)
			while self.CardsInUse[cardId]:
				cardId = random.randint(0, N_FILL)
			self.CardsInUse[cardId] = True
			plyr.addCard(cardId)
			# if send == True:
			if not plyr.isAI:
				playerCardInfo = "nwCrd: " + str(cardId)
				self.my_send(playerCardInfo)    # my_send--specific
		return

	def isWinner(self):
		msg = "winner:"
		n_winners = 0
		highest_score = 0
		winner_list = []
		
		#-determine highest score
		for i in range(4):
			highest_score = max(self.players[i].score, highest_score)
			'''
			if self.players[i].score >= self.scoreLimit:
				# msg += " " + str(i)
				n_winners += 1
			'''

		#-if the highest score has reached the scoreLimit threshold, determine winner(s) and send to client(s)
		if highest_score >= self.scoreLimit:
		# if n_winners > 0:
			for i in range(4):
				if self.players[i].score == highest_score:
					msg += " " + str(i)
					winner_list.append(self.players[i])

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
			error_flag = 0
			playerCard = self.my_recv(error_flag)

		pl_play = int(re.search(r'\d+', playerCard).group())
		self.CardsInUse[pl_play] = False
		self.players[3].playedCard = pl_play
		self.players[3].removeCard(pl_play)
		
		#-getAiPlayedCards
		for i in range(3):
			AI_play = random.choice(self.players[i].cards)
			self.players[i].playedCard = AI_play
			self.players[i].removeCard(AI_play)
		return

	def votePhase(self):
		print("VOTE------------------")
		msg = "chng: vtCrd:"
		for i in range(4):
			msg += " " + str(self.players[i].id) + " " + str(self.players[i].playedCard)
		self.my_send(msg)    # my_send--specific--??
		return

	def getVotes(self):
		if SOLO_TEST:
			playerVote = input(" >> playerVote: ")
		else:
			error_flag = 0
			playerVote = self.my_recv(error_flag)
		
		pl_vote = int(re.search(r'\d+', playerVote).group())
		self.players[3].votedCard = pl_vote
		print("  player voted for: #{}".format(pl_vote))
		
		#-getAiVotes
		for i in range(3):
			AI_vote = random.randint(0, 3)
			#-Can't vote for yourself
			while AI_vote == i:
				AI_vote = random.randint(0, 3)
			self.players[i].votedCard = AI_vote
		return

	def voteResults(self):
		'''count votes, update scores, and tell the client(s) the new scores'''
		for i in range(4):
			v = self.players[i].votedCard
			print("  AI_player ({}) voted for: {}".format(i, v))
			self.players[v].score += 1
		
		msg = "chng: vtRslt:"
		print("********************\n*  current score:\n*")
		for i in range(4):
			print(self.players[i])
			msg += " " + str(self.players[i].id) + " " + str(self.players[i].score)
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

#-----------------------------------------
#-  Play game

run_server = True
while run_server:
	game.New_Game()
	game.Play_Game()
	print("\n--  End of Game   --\n")

print("\n --!!--  run_server = False  --!!--\n")
