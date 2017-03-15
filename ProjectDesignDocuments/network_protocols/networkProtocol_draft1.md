This draft is just for server-client interaction in the context of a game.  
This does not include game creation or initial server interaction when a player logs in.  
End of message is defined by `\n`


# Client-to-Server:
* joined:  
`Join: profileName picID ?<color>?`
  - **profileName** : string
  - **picID** : int
  - **color** : int *(not implemented)*
* playCard:  
`ply: cardID`
* voteCard:  
`vt: cardID`
* message:  
`msg: message`
* leave:  
`bye`
* kick:  
`@#$% you server`
`getGames` called to get the server to send a list of current games
`newGame lobbyName playerLimit ScoreTarget TurnLimit` gets the server to make a new game and puts the client into it
`JoinGame: portNum` adds client to game if possible or sends back full message, lobbyName "Quick" for joining any open game


# Server-to-Client:
* joinedAcknowledged:  
`JoinAck: lobbyName phase idnum votetime playtime`
  - **lobbyName** : string
  - **phase** : int
  - **idnum** : int
  - **votetime** : int  *(-1 for none)*
  - **playtime** : int  *(-1 for none)*
* newPlayer:  
`nwPlyr: playerName picID ?<color>? points idnum`
  - **playerName** : string
  - **picID** : int
  - **color** : int *(not implemented)*
  - **points** : int
  - **idnum** : int
* sendCards:  
`nwCrd: cardID`
  - **cardID** : int
* voteCards:  
`vtCrd: cardID_list`
  - **cardID_list** : int  *(-1 for no selection)*
* vote results:  
`vtRslt: NumPoints_list`
  - **NumPoints_list** : int
* playerLeave:  
`byePlyr: playerID`
  - **playerID** : int
* phaseChange:  
`chng: phase [phase info, situation id ect]`
* message:  
`msg: plyrID "message"`
* won:  
`winner: plyrID`
* kick:  
`begone`
* error:  
`err: chng: phase [phase info, situation id ect]`
`gameFull` client tried to join a game without enough room
`gamesList lobbyName currentPlayers maxPlayers`
