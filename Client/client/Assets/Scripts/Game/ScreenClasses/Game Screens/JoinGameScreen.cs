using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class JoinGameScreen: GamePlayScreen
{
    bool tmp = true;

    Rect ScreenTitleRect;

    //Scroll list for games to connect to
    Vector2 GamesListPosition = Vector2.zero;
    DynamicButton ConnectButton;

    public JoinGameScreen()
    {

    }
    public JoinGameScreen(BaseScreen prevScreen) : base(prevScreen)
    {

        drawableComponents.Clear();
        ScreenTitleRect = new Rect(Screen.width * .25f, -(SizeMaster.standardButtonSize.y * 0.33f), Screen.width * 0.5f, SizeMaster.standardButtonSize.y * 4f);

        GamesListWindowRect = new Rect(ScreenTitleRect.x, ScreenTitleRect.yMax + SizeMaster.standardButtonSize.y * 0.1f, Screen.width * 0.5f, Screen.height * 0.5f);
        GameListEntrySize = new Vector2(GamesListWindowRect.width * 0.975f, SizeMaster.standardButtonSize.y);
        GamesListViewRect = new Rect(0,0, GamesListWindowRect.width * 0.975f, GamesListWindowRect.height * 1.02f);

        //Buttons for deciding how user is getting into game
        Rect ConnectButtonRect = new Rect(new Vector2(SizeMaster.IndentButtonLeft, GamesListWindowRect.yMax + SizeMaster.standardButtonSize.y), SizeMaster.standardButtonSize);
        ConnectButton = new DynamicButton(assets, ConnectButtonRect, "Connect", DyanimcButtonStyle.standard, standardTextSize, ConnectToGame, assets.greenButtonTexture, assets.redButtonTexture);
        Rect RefreshListButtonRect = new Rect(new Vector2(SizeMaster.IndentButtonRight, ConnectButtonRect.y), SizeMaster.standardButtonSize);
        DynamicButton RefreshListButton = new DynamicButton(assets, RefreshListButtonRect, "Refresh", DyanimcButtonStyle.standard, standardTextSize, RefreshGameList, assets.greenButtonTexture, assets.redButtonTexture);
        Rect CreateGameButtonRect = new Rect(new Vector2(SizeMaster.IndentButtonLeft, ConnectButtonRect.yMax + SizeMaster.standardButtonSize.y * 0.5f), SizeMaster.standardButtonSize);
        DynamicButton CreateGameButton = new DynamicButton(assets, CreateGameButtonRect, "Create New", DyanimcButtonStyle.standard, standardTextSize, CreateGame, assets.greenButtonTexture, assets.redButtonTexture);
        Rect CancelButtonRect = new Rect(new Vector2(SizeMaster.IndentButtonRight, CreateGameButtonRect.y), SizeMaster.standardButtonSize);
        DynamicButton CancelButton = new DynamicButton(assets, CancelButtonRect, "Cancel", DyanimcButtonStyle.standard, standardTextSize, CancelScreen, assets.redButtonTexture, assets.greenButtonTexture);

        //Create fake test games
		
        AddGameListEntry("TestGame1 0/5", 10000);
        AddGameListEntry("TestGame2 0/5", 10010);
        AddGameListEntry("TestGame3 0/5", 10020);
        AddGameListEntry("TestGame4 0/5", 10030);
        AddGameListEntry("TestGame5 0/5", 10040);
        AddGameListEntry("TestGame6 0/5", 10050);
		
        // AddGameListEntry("TestGame1 0/5", 11000);
        // AddGameListEntry("TestGame2 0/5", 11010);
        // AddGameListEntry("TestGame3 0/5", 11020);
        // AddGameListEntry("TestGame4 0/5", 11030);
        // AddGameListEntry("TestGame5 0/5", 11040);
        // AddGameListEntry("TestGame6 0/5", 11050);
		
        //AddGameListEntry("TestGame4 0/5", 43);
        //AddGameListEntry("TestGame5 0/5", 44);
        //AddGameListEntry("TestGame6 0/5", 45);
        //AddGameListEntry("TestGame7 0/5", 46);

        drawableComponents.Add(RefreshListButton);
        drawableComponents.Add(CreateGameButton);
        drawableComponents.Add(CancelButton);

        DrawPlayerIcons = false;

        Debug.Log("creating new network manager");
        gameState.resetAll();    
    }

    public override void updateCustom2()
    {       
        
    }
    protected override void drawCustom2()
    {
        GUI.DrawTexture(ScreenTitleRect, assets.findGameTitleTexture);

        GUI.DrawTexture(GamesListWindowRect, assets.playCardIdle);
        GamesListPosition = GUI.BeginScrollView(GamesListWindowRect, GamesListPosition, GamesListViewRect);
        foreach(GamePlayScreen.GameListEntry entry in GameList)
        {
            entry.Draw(SelectedGamePort);
        }
        GUI.EndScrollView();

        if(SelectedGamePort == -1)
        {
            GUI.enabled = false;
        }
        ConnectButton.draw();
        GUI.enabled = true;
    }

    void RequestGamesFromServer()
    {
        //sendMessage("getGameList");
    }


    public void CreateGame()
    {
        nextScreen = new CreateGameScreen(this);
    }

    public void ConnectToGame()
    {
        if (SelectedGamePort != -1)
        {
            network = new NetworkManager(SelectedGamePort);
            network.connectToServer();
            nextScreen = new ConnectingScreen(this);
        }
    }

    public void RefreshGameList()
    {
        GameList.Clear();
        RequestGamesFromServer();
    }

    public void CancelScreen()
    {
        nextScreen = new ProfileScreen(this);
    }

    public new class GameListEntry
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
            if(selectedEntry == value)
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

