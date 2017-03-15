using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class JoinGameScreen: GamePlayScreen
{
    bool tmp = true;

    //Scroll list for games to connect to
    Rect GamesListWindowRect;
    Rect GamesListViewRect;

    public JoinGameScreen()
    {

    }
    public JoinGameScreen(BaseScreen prevScreen) : base(prevScreen)
    {
        //Buttons for deciding how user is getting into game
        DynamicButton QuickMatchButton = new DynamicButton(assets, new Rect(), "Quick Match", DyanimcButtonStyle.standard, standardTextSize, assets.greenButtonTexture, assets.greenButtonTexture);
        DynamicButton CreateGameButton = new DynamicButton(assets, new Rect(), "Create New", DyanimcButtonStyle.standard, standardTextSize, assets.greenButtonTexture, assets.greenButtonTexture);
        DynamicButton ConnectButton = new DynamicButton(assets, new Rect(), "Connect", DyanimcButtonStyle.standard, standardTextSize, assets.greenButtonTexture, assets.greenButtonTexture);

        GamesListWindowRect = new Rect();
        GamesListViewRect = new Rect();

        Rect ScreenTitleRect = new Rect(Screen.width * .25f, SizeMaster.standardButtonSize.y, Screen.width * 0.5f, SizeMaster.standardButtonSize.y);

        drawableComponents.Add(QuickMatchButton);
        drawableComponents.Add(CreateGameButton);
        drawableComponents.Add(ConnectButton);

        drawableComponents.Add(new Text("connecting...", new Vector2(ScreenTitleRect.x, ScreenTitleRect.y), ScreenTitleRect.width, ScreenTitleRect.height, textStyle.title));

        network.connectToServer();

    }

    public override void updateCustom2()
    {
        if (tmp)
        {
            if (network.connected == false)
            {
                Debug.Log("connection unsuccessful");
                nextScreen = new MainScreen(this);
                tmp = false;
            }
            else
            {
                sendMessage(gameState.player.toString());
                Debug.Log("connected");
                tmp = false;
            }
        }
    }
    protected override void drawCustom2()
    {

    }
}

