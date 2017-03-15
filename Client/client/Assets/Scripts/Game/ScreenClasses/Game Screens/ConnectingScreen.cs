using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ConnectingScreen : GamePlayScreen
{
    public ConnectingScreen()
    {

    }
    public ConnectingScreen(BaseScreen prevScreen) : base(prevScreen)
    {
        Rect ScreenTitleRect = new Rect(Screen.width * .25f, SizeMaster.standardButtonSize.y, Screen.width * 0.5f, SizeMaster.standardButtonSize.y);

        drawableComponents.Add(new Text("connecting...", new Vector2(ScreenTitleRect.x, ScreenTitleRect.y), ScreenTitleRect.width, ScreenTitleRect.height, textStyle.title));

        DrawPlayerIcons = false;
    }
    bool tmp = true;
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
