using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

delegate void messageCallback(string[]words);
struct messageCallbackStruct
{
    public messageCallbackStruct(string messageHeader_c, messageCallback callback_c)
    {
        messageHeader = messageHeader_c;
        callback = callback_c;
    }
    public string messageHeader;
    public messageCallback callback;
}

abstract class GamePlayScreen:BaseScreen
{

    protected messageCallbackStruct[] messageCallbacks;

    public bool DrawPlayerIcons = true;
    protected List<PlayerIcon> PlayerIcons;
    public List<GameListEntry> GameList = new List<GameListEntry>();
    public Rect GamesListWindowRect;
    public Rect GamesListViewRect;
    public Vector2 GameListEntrySize;
    public int SelectedGamePort = -1;

    public List<DynamicButton> VoteCards = new List<DynamicButton>();
    public List<DynamicButton> OpponentVoteCards = new List<DynamicButton>();

    public GamePlayScreen() : base() { }
    public GamePlayScreen(NetworkManager n, AssetManager a,  GameState gs) : base(n, a,  gs)
    {

    }


    public GamePlayScreen(BaseScreen prevScreen):base(prevScreen.network, prevScreen.assets, prevScreen.gameState)
    {
        messageCallbacks = new messageCallbackStruct[9];
        int x = 0;
        messageCallbacks[x++] = new messageCallbackStruct("nwPlyr:", handleNewPlayerMessage);
        messageCallbacks[x++] = new messageCallbackStruct("playerLeave:", handleLeavePlayerMessage);
        messageCallbacks[x++] = new messageCallbackStruct("JoinAck:", handleJoinAckMessage);
        messageCallbacks[x++] = new messageCallbackStruct("playPhase:", handlePlayPhaseMessage);
        messageCallbacks[x++] = new messageCallbackStruct("votePhase:", handleVotePhaseMessage);
        messageCallbacks[x++] = new messageCallbackStruct("vtRslt:", handleVoteResultMessage);
        messageCallbacks[x++] = new messageCallbackStruct("winner:", handleWinnerMessage);
        messageCallbacks[x++] = new messageCallbackStruct("nwCrd:", handleNewCardMessage);
        messageCallbacks[x++] = new messageCallbackStruct("game:", handleGameInfo);
        try
        {
            GamePlayScreen prev = (GamePlayScreen)prevScreen;
            PlayerIcons = prev.PlayerIcons;
        }
        catch
        {

            PlayerIcons = new List<PlayerIcon>();
            addNewPlayerIcon(gameState.player);
        }
    }
    
    public void handleNewPlayerMessage(string[] words)
    {
        Debug.Log("handling new player message");
        string playerName = words[1];
        string playerProfileIconIdString = words[2];
        int playerProfileIconId = -1;
        if(!Int32.TryParse(playerProfileIconIdString, out playerProfileIconId)){
            Debug.Log("could not parse player profile iconId");
            playerProfileIconId = 0;
        }
        string playerScoreString = words[3];
        int playerScore = 0;
        if (!Int32.TryParse(playerScoreString, out playerScore))
        {
            Debug.Log("could not parse player score");
            playerScore = 0;
        }
        string playerID = words[4];
        Profile player = new Profile(playerName, playerID, playerProfileIconId, 0,playerScore, assets);
        addNewPlayerIcon(player);

    }
    public void handleLeavePlayerMessage(string[] words)
    {
        Debug.Log("handle player leaving message");
        removePlayerIcon(words[1]);
    }
    public void handleJoinAckMessage(string [] words)
    {
        Debug.Log("handling join ack message");
        string playerId = words[1];
        gameState.player.setProfileID(playerId);

        


    }
    
    public void handleNewCardMessage(string[] words)
    {
        Debug.Log("handling new card message");
        string cardIdString = words[1];
        int cardId = -1;
        if (!Int32.TryParse(cardIdString, out cardId))
        {
            Debug.Log("could not parse card id");
        }
        else
            gameState.addPlayCard(cardId);

    }
    public void handlePlayPhaseMessage(string[] words)
    {
        Debug.Log("hadnling play phase message");
        string situationCardIdString = words[1];
        int situationCardId = -1;
        if(!Int32.TryParse(situationCardIdString, out situationCardId))
        {
            Debug.Log("could not parse situation card");
            situationCardId = 0;
        }
        gameState.setSituationCardIndex(situationCardId);
        nextScreen = new PlayCardScreen((GamePlayScreen)this);

    }
    public void handleVotePhaseMessage(string[] words)
    {
        Debug.Log("handling vote phase message:");
        gameState.resetVoteCards();
        for (int i =1; i< words.Length; i+=2)
        {
            string playerId = words[i];
            string cardIdString = words[i + 1];
            int cardId = -1;
            if (!Int32.TryParse(cardIdString, out cardId))
            {
                Debug.Log("could not parse vote card");
                cardId  = 0;
            }
            gameState.addVoteCard(cardId, playerId);

        }
        nextScreen = new VoteCardScreen((GamePlayScreen)this);
    }
    public void handleVoteResultMessage(string[] words)
    {
        Debug.Log("handling vote result message");
        int HighScore = -1;
        int winner = 0;//only one winner, boo for the rest

        for (int i =1; i+1 < words.Length; i+=2)
        {
            string scoreString = words[i + 1];
            string playerId = words[i];
            int newScore = 0;
            Int32.TryParse(scoreString, out newScore);
            Debug.Log(scoreString + " " + playerId);
            //now find the player this should go to
            bool foundPlayer = false;
            Debug.Log("num players: " + gameState.numPlayers());
            for (int x =0; x < PlayerIcons.Count; x++)
            {
                if (PlayerIcons[x].getPlayerID()== playerId)
                {
                    
                    foundPlayer = true;
                    int oldScore = PlayerIcons[x].getScore();
                    int scoreDifference = newScore - oldScore;
                    if (scoreDifference > HighScore)
                    {
                        HighScore = scoreDifference;
                        winner = x;
                    }
                    PlayerIcons[x].incrementScore(newScore);


                    break;
                }
            }
            if (foundPlayer == false)
                Debug.Log("could not find player");


        }
        //now we update who won, and who did not
        PlayerIcons[winner].setAsWinner(3);


        nextScreen = new EndRoundSummaryScreen(this);
    }
    public void handleWinnerMessage(string[] words)
    {
        Debug.Log("handling winner Message");

        List<int> WinnerKeys = new List<int>();
        for (int i = 1; i < words.Length; i++)
        {
            String id = words[i];
            bool foundPlayer = false;
            //now find the index of that player
            for (int x = 0; x < PlayerIcons.Count; x++)
            {
                if (PlayerIcons[x].getPlayerID() == id)
                {
                    foundPlayer = true;
                    Debug.Log("adding player as winner: " + PlayerIcons[x].getPlayerID() + " " + id);
                    WinnerKeys.Add(x);
                }
            }
            if (!foundPlayer)
            {
                Debug.Log("could not find winner");
            }
        }

        nextScreen = new EndGameSummaryScreen(this, WinnerKeys);
    }
    
    public void handleGameInfo(string[] words)
    {
        string LobbyName = words[1];
        int port = -1;
        Int32.TryParse(words[2], out port);
        if(port == -1)
        {
            Debug.Log("Failed to get a valid port");
            return;
        }
        int PlayerCount = -1;
        Int32.TryParse(words[2], out PlayerCount);
        if (PlayerCount == -1)
        {
            Debug.Log("Failed to get a valid player count");
            return;
        }
        int MaxPlayers = 5;
        AddGameListEntry(LobbyName + PlayerCount.ToString() + "/" + MaxPlayers.ToString(), port);
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

    
    public void removePlayerIcon(string ID)
    {
        foreach(PlayerIcon icon in PlayerIcons)
        {
            if (icon.getPlayerID()== ID)
            {
                PlayerIcons.Remove(icon);
                break;
            }
        }
        adjustPlayerIconPositions();
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

    public void SelectGamePort(int value)
    {
        SelectedGamePort = value;
    }

    public void AddGameListEntry(string name, int port)
    {
        GameList.Add(new GameListEntry(new Rect(0, GameList.Count * GameListEntrySize.y, GameListEntrySize.x, GameListEntrySize.y), name, port, assets.redButtonTexture, assets.greenButtonTexture, SelectGamePort, assets));
        GamesListViewRect.height = Mathf.Max(GamesListWindowRect.height * 1.02f, GameListEntrySize.y * GameList.Count);
    }

    override protected void drawCustom()
    {
        drawCustom2();
        if (DrawPlayerIcons)
        {
            foreach (PlayerIcon p in PlayerIcons)
            {
                p.draw();
            }
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
        Debug.Log("Parsing Message: " + msg);
        string[] words = msg.Split(' ');
        bool foundMessage = false;
        for (int i =0; i<messageCallbacks.Length; i++)
        {
            if (words[0] == messageCallbacks[i].messageHeader)
            {
                foundMessage = true;
                
                messageCallbacks[i].callback(words);
                break;
            }
        }
        if (foundMessage== false)
        {
            Debug.Log("could not parse message: " + msg);
        }
       
    }

    

    public class GameListEntry
    {
        Rect rect;
        string text;
        Texture2D backGround;
        Texture2D highLighted;
        int value;
        public delegate void selectGame(int val);
        public selectGame SelectGame;
        AssetManager assetManager;
        GUIStyle style = new GUIStyle();

        public GameListEntry(Rect trect, string stext, int val, Texture2D norm, Texture2D highlight, selectGame callBack, AssetManager assets)
        {
            this.rect = trect;
            this.text = stext;
            this.value = val;
            this.backGround = norm;
            this.highLighted = highlight;
            this.SelectGame = callBack;
            this.assetManager = assets;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = SizeMaster.standardTextSize;
        }

        public void Draw(int selectedEntry)
        {
            if (selectedEntry == value)
            {
                GUI.DrawTexture(rect, highLighted, ScaleMode.StretchToFill);
                if (GUI.Button(rect, text, style))
                {
                    SelectGame(-1);
                    assetManager.PlayButtonClicked();
                }
            }
            else
            {
                GUI.DrawTexture(rect, backGround, ScaleMode.StretchToFill);
                if (GUI.Button(rect, text, style))
                {
                    SelectGame(value);
                    assetManager.PlayButtonClicked();
                }
            }
        }
    }
}

