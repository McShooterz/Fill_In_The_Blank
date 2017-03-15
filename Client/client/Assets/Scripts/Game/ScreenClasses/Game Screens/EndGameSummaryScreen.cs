using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class EndGameSummaryScreen : GamePlayScreen
{

    DynamicButton readyButton;
    Texture2D gameOver;

    public EndGameSummaryScreen()
    {

    }

    public EndGameSummaryScreen(BaseScreen prevScreen, List<int> Winners) : base(prevScreen)
    {
        gameOver = assets.gameOverTexture;
        //create ready for next screen button
        Rect temp = new Rect(Screen.width * .5f - SizeMaster.standardButtonSize.x * 0.5f, Screen.height * .6f + SizeMaster.standardButtonSize.y*2,
            SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y*2f);
        readyButton = new DynamicButton(assets, temp, "return to main menu", DyanimcButtonStyle.playCard, standardTextSize, readyCallBack, assets.playCardIdle, assets.redButtonTexture);
        readyButton.isActive = true;
        leavePlayers();
        //Move winning player to center of screen and set winner by profiles index
        //MoveWinnerToScreenCenter(winner);
        //Move winning players to center of screen and set winner
        float Indent = (Screen.width - SizeMaster.opponentSize.x * Winners.Count) / 2f;
        for (int i = 0; i < Winners.Count; i++)
        {
            PlayerIcon playerIcon = PlayerIcons[Winners[i]];
            if (playerIcon == null) continue;
            playerIcon.setPositionToValue(8f);
            playerIcon.setAsWinner(60f);
            playerIcon.setTargetPosition(new Vector2(Indent, SizeMaster.CardsY));
            Indent += SizeMaster.opponentSize.x;
        }
    }

    public override void updateCustom2()
    {

    }

    protected override void drawCustom2()
    {
        GUI.DrawTexture(new Rect(Screen.width*.295f, 0, Screen.width*.4f, Screen.width*.4f), gameOver);
        readyButton.draw();
    }

    public void readyCallBack(DynamicButton b)
    {
        network.closeConnection();
        nextScreen = new MainScreen((GamePlayScreen)this);
    }
}

