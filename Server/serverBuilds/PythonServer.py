#! /usr/bin/env python
#-----------------------------------------
#	PythonServer.py
#	cs340 team05		spring 2016
#	Mike Shoots
#	Travis Brown
#	Tyler Poff
#-----------------------------------------
#	This is the server for the "Fill in the ______" game
#	for cs340 Software Engineering group project.
#-----------------------------------------
import socket
import random
import re

#=========================================
#====== Global Variables (settings) ======

#--card files
cardfile_St = "situation_card_list.json" # statement cards
cardfile_Bl = "fill_in_card_list.json"   # blank cards

#--other options
NETWORKING = False   #--currently not implemented
SOLO_TEST  = False   # set True when testing without client

OTHER   = True
OPTIONS = True
GO_HERE = True

DELIM = "------------------------------------------"

#------------ end of global variables ---------------------


#=========================================
#=============    Classes    =============

class Player:
	def __init__(self, name, score, picId, playerid):
		self.name = name
		self.score = score
		self.picId = picId
		self.cards = []
		self.id = playerid
		self.numCards = 0
		self.playedCard = -1
		self.votedCard = -1
		
	def resetChoices(self):
		self.playedCard = -1
		self.votedCard = -1
		
	def nwPlyrString(self):
		botInfo = "nwPlyr: " + self.name + " " + str(self.picId) + " " + str(self.score) + " " + str(self.id) + "\n"
		return botInfo
		
	def addCard(self, cardId):
		self.cards.append(cardId)
		self.numCards += 1
		
	def removeCard(self, cardId):
		self.cards.remove(cardId)
		self.numCards -= 1
		
	def hasCard(self, cardId):
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
		self.aiPlayerNames = ["steve", "bob", "bill", "jones", "steve Jobs", "Bill Gates"]
		self.numFillCards = 50
		self.numSituationCards = 50
		self.numPicIds = 5
		self.fillCardsInUse = [0] * self.numFillCards
		self.numCardsPerPlayer = 5
		self.numPlayers = 4
		self.scoreLimit = 5
		self.winner = False

		self.players = []
		for i in range(3):
			nameIndex = random.randint(0, 5)
			score = 0
			picId = i
			idNum = i
			p = Player(self.aiPlayerNames[nameIndex], score, picId, idNum)
			for x in range(5):
				cardId = random.randint(0, self.numFillCards-1)
				while self.fillCardsInUse[cardId] == 1:
					cardId = random.randint(0, self.numFillCards-1)
				self.fillCardsInUse[cardId] = 1
				p.addCard(cardId)
			self.players.append(p)

		self.playerCards = []

		#-set up socket
		# '''
		self.s = socket.socket()
		host = socket.gethostname()
		port = 11000
		self.s.bind((host, port))
		# '''

	def waitConnection(self):
		self.s.listen(5)
		self.c, self.addr = self.s.accept()
		return True

	def playerJoin(self):
		'''
		Handle intial player-server interaction as follows:
		1.  client sends the server their player info
		2.  the server acknowledges, sends (current game status, id, timer info)
		'''
	
		if SOLO_TEST:
			playerData = input(" >> playerData: ")
		else:
			playerData = self.c.recv(250).decode()
		
		print("player Info: ", playerData)

		#-now acknowledge the join
		joinAck = "JoinAck: lobbyName RDY 3 -1 -1\n"
		self.c.send(joinAck.encode())  #socket_send
		self.players.append(Player("realboy", 0, 0, 3))

		return playerData

	def dealCards(self, plyr, numCardsDeal, send):
		'''Deal a specified number of cards to the player'''
		playerHand = []
		for i in range(numCardsDeal):
			cardId = random.randint(0, self.numFillCards-1)
			while self.fillCardsInUse[cardId] == 1:
				cardId = random.randint(0, self.numFillCards-1)
			self.fillCardsInUse[cardId] = 1
			plyr.addCard(cardId)
			if send == True:
				playerCardInfo = "nwCrd: "+str(cardId)
				print("sending: ", playerCardInfo)
				playerCardInfo = playerCardInfo+"\n"
				self.c.send(playerCardInfo.encode())  #socket_send
		return

	def replaceCards(self):
		for i in range(4):
			if self.players[i].numCards < self.numCardsPerPlayer:
				numCards = self.numCardsPerPlayer - self.players[i].numCards
				if (i == 3):
					self.dealCards(self.players[i], numCards, True)
				else:
					self.dealCards(self.players[i], numCards, False)
		return

	def sendOtherPlayerInfo(self):
		'''Send the other players' information to the client'''
		
		#-(AI testing) send player info for the bots
		for i in range(3):
			info = self.players[i].nwPlyrString()
			self.c.send(info.encode())  #socket_send
			print("sending: ", info)
		return

	def newRound(self):
		'''Declare new round and send new situation card'''
		phaseChange = "chng: playPhase " + str(random.randint(0, self.numSituationCards-1)) + "\n"
		self.c.send(phaseChange.encode())  #socket_send
		for i in range(4):
			self.players[i].resetChoices()
		return

	def getPlayedCards(self):
		if SOLO_TEST:
			playerCard = input(" >> playerCard: ")
		else:
			playerCardData = self.c.recv(250)
			playerCard = playerCardData.decode()

		choice = int(re.search(r'\d+', playerCard).group())
		self.fillCardsInUse[choice] = 0
		self.players[3].playedCard = choice
		self.players[3].removeCard(choice)
		return

	def getAiPlayedCards(self):
		for i in range(3):
			choice = self.players[i].cards[random.randint(0, 4)]
			self.players[i].playedCard = choice
			self.players[i].removeCard(choice)
		return


	def votePhase(self):
		msg = "chng: vtCrd: "
		for i in range(4):
			msg = msg + str(self.players[i].playedCard) + " "
		print("sending: ", msg)
		msg = msg + "\n"
		self.c.send(msg.encode())  #socket_send
		return

	def getVotedCards(self):
		if SOLO_TEST:
			playerCard = input(" >> playerCard: ")
		else:
			playerCardData = self.c.recv(250)
			playerCard = playerCardData.decode()
		
		choice = int(re.search(r'\d+', playerCard).group())
		self.players[3].votedCard = choice
		print("player voted for: ", choice)
		return

	def getAiVotedCards(self):
		for i in range(3):
			choice = random.randint(0, 3)
			while choice == i:
				choice = random.randint(0, 3)
			self.players[i].votedCard = choice
		return

	def tallyVotes(self):
		for i in range(4):
			# print("player ", i, "voted for: ", self.players[i].votedCard)
			print("player", i, "voted for:", self.players[i].votedCard)
			self.players[self.players[i].votedCard].score += 1
		return

	def voteResults(self):
		msg = "chng: vtRslt: "
		print("--------------\n-  current score:")
		
		for i in range(4):
			print(self.players[i])
			msg = msg + str(self.players[i].score) + " "
		print("-\n--------------")
		print("sending: ", msg)
		msg = msg + "\n"
		self.c.send(msg.encode())  #socket_send
		return

	def isWinner(self):
		msg = "winner: "
		winners = 0
		for i in range(4):
			if self.players[i].score >= self.scoreLimit:
				msg = msg + str(i) + " "
				winners += 1

		if winners > 0:
			msg = msg + "\n"
			self.c.send(msg.encode())  #socket_send
			return True
		return False

	def sendContinue(self):
		self.c.send("in_progress".encode())
		return

	def sendEnd(self):
		self.c.send("_ENDGAME_").encode()
		return
#------------ end of Game() class -------------------------


#=========================================
#===========    begin MAIN    ============

#-----------------------------------------
#-  Connection/"Matchmaking"

#-initialize Game object
game = Game()
print("Game intialized!")


#-connect to Player1
# if NETWORKING:
print("Waiting for a player to connect\n  ...")
game.waitConnection()
print("Ha! Found one!")


#-add Player1 to the game
print("--Welcome player [" + game.playerJoin() + "] to the table")


#-tell Player1 who else is at the table
print("sending other player info")
game.sendOtherPlayerInfo()
print("info sent\n")


#-----------------------------------------
#-  Play game

# while game.isWinner() == False:
while True:
	game.replaceCards()      #  clnt -- srvr
	print("changing mode to play mode")
	game.newRound()          #  clnt << srvr
	print("mode changed\n")

	print("waiting for player to play card")
	game.getPlayedCards()    #  clnt >> srvr
	game.getAiPlayedCards()  #  clnt -- srvr
	print("...got card from player\n")

	print("=-=-=-=-=| Vote time |=-=-=-=-=\n")
	game.votePhase()         #  clnt << srvr
	game.getVotedCards()     #  clnt >> srvr
	game.getAiVotedCards()   #  clnt -- srvr

	print("---Votes recieved, counting ... ")
	game.tallyVotes()        #  clnt -- srvr
	game.voteResults()       #  clnt << srvr
	
	# '''
	if game.isWinner():
		# game_state = "_ENDGAME_"
		# game.sendEnd()
		break
	else:
		# game.sendContinue()
		continue
	# '''

print("ending program")
