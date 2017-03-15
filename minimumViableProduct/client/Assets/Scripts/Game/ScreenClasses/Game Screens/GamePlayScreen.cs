using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


abstract class GamePlayScreen:BaseScreen
{

    protected List<PlayerIcon> PlayerIcons;
    public List<GameListEntry> GameList = new List<GameListEntry>();

    public GamePlayScreen() : base() { }
    public GamePlayScreen(NetworkManager n, AssetManager a,  GameState gs) : base(n, a,  gs)
    {

    }


    public GamePlayScreen(BaseScreen prevScreen):base(prevScreen.network, prevScreen.assets, prevScreen.gameState)
    {
        
        try
        {
            GamePlayScreen prev = (GamePlayScreen)prevScreen;
            Debug.Log("from gameplay screen");
            PlayerIcons = prev.PlayerIcons;
        }
        catch
        {
            Debug.Log("from base screen");
            PlayerIcons = new List<PlayerIcon>();
            addNewPlayerIcon(gameState.player);
        }
    }


    public void addNewPlayerIcon(Profile p)
    {
        float Indent = (Screen.width - SizeMaster.opponentSize.x * (PlayerIcons.Count + 1)) / 2f;

        Vector2 startPos = new Vector2(-(Indent + SizeMaster.opponentSize.x * (gameState.numPlayers() - PlayerIcons.Count * 1.25f)), SizeMaster.opponentsY);
        PlayerIcon icon = new PlayerIcon(p, startPos, assets);
        icon.setPositionToValue(8f);
        icon.setAsNew(5); 
        PlayerIcons.Add(icon);
        adjustPlayerIconPositions();

    }

    public void removePlayerIcon(int Key)
    {
        foreach(PlayerIcon icon in PlayerIcons)
        {
            if(icon.CheckProfile(gameState.getProfile(Key)))
            {
                PlayerIcons.Remove(icon);
                return;
            }
        }
    }

    public PlayerIcon GetPlayerIcon(int Key)
    {
        foreach (PlayerIcon icon in PlayerIcons)
        {
            if (icon.CheckProfile(gameState.getProfile(Key)))
            {
                return icon;
            }
        }
        return PlayerIcons[0];
    }

    public void SetPlayerIconWinner(int Key)
    {
        foreach (PlayerIcon icon in PlayerIcons)
        {
            if(icon.CheckProfile(gameState.getProfile(Key)))
            {
                icon.setAsWinner(3f);
            }
        }
    }

    public void adjustPlayerIconPositions()
    {
        float Indent = (Screen.width - SizeMaster.opponentSize.x * PlayerIcons.Count) / 2f;


        for (int i = 0; i < PlayerIcons.Count; i++)
        {
            Vector2 position = new Vector2(Indent + SizeMaster.opponentSize.x * i, SizeMaster.opponentsY);

            PlayerIcons[i].setPositionToValue(8f);
            PlayerIcons[i].setTargetPosition(position);
            
        }
    }
    public void leavePlayers()
    {
        float Indent = (Screen.width - SizeMaster.opponentSize.x * PlayerIcons.Count) / 2f;


        for (int i = 0; i < PlayerIcons.Count; i++)
        {
            Vector2 position = new Vector2(Indent + SizeMaster.opponentSize.x * i, Screen.height+SizeMaster.opponentSize.y*2);

            PlayerIcons[i].setPositionToValue(8f);
            PlayerIcons[i].setTargetPosition(position);

        }
    }

    //Used to move the winning player at the end of a game to the center of the screen
    public void MoveWinnerToScreenCenter(int WinnerIndex)
    {
        PlayerIcons[WinnerIndex].setPositionToValue(8f);
        PlayerIcons[WinnerIndex].setTargetPosition(new Vector2(Screen.width * 0.5f - SizeMaster.opponentSize.x * 0.5f, SizeMaster.CardsY));
        PlayerIcons[WinnerIndex].setAsWinner(60f);
    }

    //Takes a statement and fills it with fill, or a blank if fill is empty
    public string FormatStatement(string statement, string fill)
    {
        if(fill.Length == 0)
        {
            fill = "______";
        }
        statement = statement.Replace("[BLANK]", fill);
        return statement;
    }

    override protected void drawCustom()
    {
        drawCustom2();
        foreach (PlayerIcon p in PlayerIcons)
        {
            p.draw();
        }
    }
    protected abstract void drawCustom2();


    public override void updateCustom()
    {
        if (network.connected)
            if (network.hasMessage()&&screenLocked==false)
            {
                parseMessages(network.getMessage());
            }
        foreach(PlayerIcon p in PlayerIcons)
        {
            p.update();
        }
        
        updateCustom2();
    }
    public abstract void updateCustom2();


    public void sendMessage(string message)
    {
        message = message + "\n";
        network.SendMessage(message);
    }

    void parseMessages(string msg)
    {
        if (nextScreen == null)
        {
            Debug.Log("parsing message: " + msg);
            //Split message into parts by spaces
            string[] messageParts = msg.Split(' ');

            if (messageParts[0] == "winner:")
            {
                //Grab winner ids
                List<int> WinnerKeys = new List<int>();
                for(int i = 1; i < messageParts.Length; i++)
                {
                    WinnerKeys.Add(Int32.Parse(messageParts[i]));
                }
                nextScreen = new EndGameSummaryScreen(this, WinnerKeys);
            }
            else if (messageParts[0] == "nwCrd:") //New card
            {
                //Add new card to hand
                gameState.addPlayCard(Int32.Parse(messageParts[1]));
            }
            else if (messageParts[0] == "nwPlyr:") //New player
            {
                Profile profile = new Profile(messageParts[1], Int32.Parse(messageParts[2]), Int32.Parse(messageParts[3]), assets);
                gameState.addPlayer(Int32.Parse(messageParts[4]), profile);
                addNewPlayerIcon(profile);
            }
            else if (messageParts[0] == "JoinAck:") //Join a game
            {
                //lobbyName phase idnum votetime(-1for none) playtime(-1 for none)
                gameState.LobbyName = messageParts[1];
                //phase messageParts[2]
                gameState.playerKey = messageParts[3];
                gameState.addPlayer(Int32.Parse(messageParts[3]), gameState.player);
                //vote time messageParts[4]
                //play time messageParts[5]
            }
            else if (messageParts[0] == "byePlyr:") //Player leave
            {
                removePlayerIcon(Int32.Parse(messageParts[1]));
                gameState.removePlayer(Int32.Parse(messageParts[1]));
            }
            else if (messageParts[0] == "chng:") //Change phase
            {
                if(messageParts[1] == "playPhase") //Start play phase
                {
                    gameState.setSituationCardIndex(Int32.Parse(messageParts[2]));

                    if (this.nextScreen != null)
                    {
                        nextScreen.nextScreen = new PlayCardScreen((GamePlayScreen)this);
                    }
                    else
                        nextScreen = new PlayCardScreen((GamePlayScreen)this);
                }
                else if (messageParts[1] == "vtCrd:") //Vote cards
                {
                    gameState.resetVoteCards();

                    for (int i = 2; i < messageParts.Length; i++)
                    {
                        if (i + 1 != messageParts.Length)
                        {
                            i++;
                            gameState.addVoteCard(Int32.Parse(messageParts[i]));
                        }
                        else
                        {
                            return;
                        }
                    }
                    nextScreen = new VoteCardScreen((GamePlayScreen)this);
                }
                else if (messageParts[1] == "vtRslt:") //Voting results
                {
                    int HighScore = -1;
                    List<string> Keys = new List<string>();
                    List<int> Scores = new List<int>();
                    //Grab values from message
                    for (int i = 2; i < messageParts.Length; i++)
                    {
                        if(i + 1 != messageParts.Length)
                        {
                            i++;
                            Keys.Add(messageParts[i - 1]);
                            Scores.Add(Int32.Parse(messageParts[i]));
                        }
                        else
                        {
                            //Improper format
                            return;
                        }
                    }
                    //Increment scores
                    for(int i =0; i < Scores.Count; i++)
                    {
                        GetPlayerIcon(Int32.Parse(Keys[i])).incrementScore(Scores[i]);
                    }
                    //Determine highScore
                    foreach(int value in Scores)
                    {
                        if(value > HighScore) HighScore = value;
                    }
                    //Set player icons to winners
                    for(int i = 0; i < Scores.Count; i++)
                    {
                        if (Scores[i] == HighScore) SetPlayerIconWinner(Int32.Parse(Keys[i]));
                    }
                    nextScreen = new EndRoundSummaryScreen(this);
                }
            }
            else if (messageParts[0] == "msg:") //Chat message
            {

            }
            else if (messageParts[0] == "error:")
            {

            }
            else if (messageParts[0] == "begone") //Client player removed from game
            {

            }
            else
            {
                Debug.Log("message could not be parsed");
            }
        }


        /*
        if (nextScreen == null)
        {
            Debug.Log("parsing message: " + msg);
            if (msg[0] == 'w')//we have a winner
            {
                string sub = msg.Substring(7, 2);
                int index = Int32.Parse(sub);
                if (index == 3)
                {
                    index = 0;
                }
                else
                    index++;
                nextScreen = new EndGameSummaryScreen(this,index);

            }
            //is this a new card signal?
            if (msg[0] == 'n' &&
               msg[1] == 'w' &&
               msg[2] == 'C')
            {
                gameState.addPlayCard(Int32.Parse(msg.Substring(6)));
            }
            //new player?
            else if (msg[0] == 'n' &&
                msg[1] == 'w' &&
                msg[2] == 'P')
            {
                string playerInfo = msg.Substring(8);
                var name = playerInfo.Split(' ')[0];
                Profile p = new Profile(name, 0, 0);
                gameState.addPlayer(p);
                addNewPlayerIcon(p);
            }
            //check if this is the command to change to play mode.
            else if (msg[0 + 6] == 'p' && msg[1 + 6] == 'l')
            {
                //get the card index from the message
                int cardIndex = Int32.Parse(msg.Substring(15));
                //set this as the new situation card index
                gameState.setSituationCardIndex(cardIndex);
                //set the nextScreen to the PlayCardScreen
                Debug.Log("switching to play state");
                if (this.nextScreen!= null)
                {
                    nextScreen.nextScreen = new PlayCardScreen((GamePlayScreen)this);
                }
                else
                    nextScreen = new PlayCardScreen((GamePlayScreen)this);
            }

            //check if it's a vote
            else if (msg[0 + 6] == 'v' && msg[1 + 6] == 't' && msg[2 + 6] == 'C')
            {
                //split the string so we just get the card indecies.
                string cardPortion = msg.Substring(13);
                string tmp = cardPortion.Split('\n')[0];
                //split the string into their base elements.
                string[] elements = tmp.Split(' ');
                //reset the vote card list
                gameState.resetVoteCards();
                for (int i = 0; i < elements.Length; i++)
                {//loop through the elements
                    int x = -1;
                    //try and parse the number
                    bool isNumber = int.TryParse(elements[i], out x);
                    if (isNumber)
                    {
                        //if it was a number, add it to the card vote list.
                        gameState.addVoteCard(x);
                    }
                }
                //Swap index 0 and 3 for player
                gameState.swapVoteCards();

                nextScreen = new VoteCardScreen((GamePlayScreen)this);
            }
            //vtRslt: list of ints
            else if (msg[0 + 6] == 'v' && msg[2 + 6] == 'R')
            {
                //vote results;
                string votes = msg.Substring(7 + 6);
                string tmp = votes.Split('\n')[0];
                string[] elements = tmp.Split(' ');
                int highScore = -1;
                int highScoreIndex = 0;

                int playerscore = -1;
                int.TryParse(elements[4], out playerscore);
                profiles[0].incrementScore(playerscore);
                playerscore -= profiles[0].getScore();
                highScore = playerscore;
                highScoreIndex = 0;
                for (int i = 0; i < gameState.numPlayers(); i++)
                {
                    int score = 0;
                    bool isNumber = int.TryParse(elements[i + 1], out score);
                    if (isNumber)
                    {
                        if (score-profiles[i+1].getScore() > highScore)
                        {
                            highScore = score - profiles[i + 1].getScore();
                            highScoreIndex = i+1;
                        }
                        profiles[i+1].incrementScore(score);

                    }
                    else
                    {
                        isNumber = false;
                    }
                }
                profiles[highScoreIndex].setAsWinner(3);
                ((DynamicButton)drawableComponents[highScoreIndex]).setAsWinner(3);
                nextScreen = new EndRoundSummaryScreen(this);
            }
            else
            {
                Debug.Log("message could not be parsed");
            }
        }*/
    }

    public class GameListEntry
    {

        public GameListEntry()
        {

        }

        public void Draw()
        {

        }
    }
}

