
Proposal Report: *Travis's notes*
===============


### Technology: Server, Python, Flask
* Server  

	While the user interface for the game is implemented on the phone client, the software that drives the game is included in the server. The server has two responsibilities: connect the players, and manage the game. Since this is a turn-based card game, the server-side calculations don't involve any physics, graphics, or any other complicated calculations. This piece of software will be implemented in Python to allow for rapid prototyping and flexible development. Python should be able to handle the required tasks without a significant performance decrease.

* Flask/networking  

	One of the reasons Python was chosen for the server implementation are the vast multitude of choices for web implementation. The Flask microframework provides the basic functionality of a web server. Flask was chosen over alternatives such as Django and Twisted for it's small footprint and ease of use. Flask has a socketsIO library to implement the websockets that will communicate with the client's .Net sockets. Communication via sockets will allow communication channels to remain open and allow the gameplay to flow smoothly without having to re-establish connections.  
	The server will have a process that is constantly listening for clients to connect. When a new connection is found, the server will get identification information from the client. Initially, this will just be a username and profile image id, but future plans could involve password credentials to retrieve profile information from a server-side database. Once the server has an identity matched with a connection, it will pass that information to the matchmaker. Initially, the matchmaker will be a part of the connection process but could be split into a separate process if we need to provide more complicated matchmaking.

* Game: class interactions, game flow, etc  

	The game will consist of four primary classes. There will be a Profile class, a Table class, a Player class, and a simple Deck class. The Deck class is a wrapper for a list of cards that includes a function to distribute a random card. The profile will contain all information about a player that exists outside of a game. This includes a username and a profile image, and could be upgraded to include a password and other permanent data such as match history and friend list. The player class contains player data that exists in-game, such as their hand and current score. This class will include functions to allow the player to draw and play cards as well as cast votes. The Game class will represent a game in progress. It will have a list of players, the card decks, and any game options that affect gameplay.


### Personal bio
Travis has some experience programming in Python, but not on the same scale as this project. Most of his experience has been using regular expressions and manipulating lists. He has a basic understanding of networking and has implemented a bare-bones chat server written in C. The Flask framework and SocketIO library are both completely new for Travis.


## Minimum Viable Product

Our minimum viable product will consist of a single game mode with predetermined options. The client will open to a Main Menu with two options: Start Game and Exit. Upon selecting Start Game, the client will connect to the server to play a single game with "AI" opponents. When the game ends, the client will return to the main menu to start another game. Only one player will be able to access the server. This should be sufficient to get an idea of whaat the finished product will be like, and will give us a solid foundation to build upon.

## Release product

Our initial release will build upon our minimum product. The Main Menu will have two additional buttons: Login and Options. Login will allow the player to chose a username and profile icon and Options will allow players to turn music and sound on or off. The server will be able to accomidate multiple concurrent games, and will have a way to replace players that aren't participating. When joining a game, there will be an option for a timed game in addition to the standard game mode. This initial release version will also include minimal sprites for the background, intoduction/splash screen and player icons

