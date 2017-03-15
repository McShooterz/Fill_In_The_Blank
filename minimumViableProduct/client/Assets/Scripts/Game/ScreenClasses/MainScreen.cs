using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class MainScreen : BaseScreen
{

    public MainScreen()
    {

    }
    public MainScreen(BaseScreen prevScreen) : base(prevScreen)
    {
        init();
    }
    public MainScreen(NetworkManager n, AssetManager a,  GameState gs) : base(n, a,gs)
    {
        init();
    }
    public void init()
    {

        //create play game function
         void_function_DyanmicButton f = directConnectCallback;
        DynamicButton temp = new DynamicButton(assets, new Rect(SizeMaster.IndentButtonMiddle, SizeMaster.ScreenTitleRect.y + SizeMaster.SpacingButtonHalf * 3,
            SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y*2f),
            "Play Game",
            DyanimcButtonStyle.playCard, standardTextSize, f, assets.playCardIdle, assets.playCardPressed);
        temp.setPositionToValue(2f);
        temp.setTargetPosition(new Vector2(SizeMaster.IndentButtonMiddle, SizeMaster.ScreenTitleRect.y + SizeMaster.SpacingButtonHalf * 3));
        //temp.setAsNew(15);
        //temp.setAsWinner(10);
        drawableComponents.Add(temp);


        //create quit button
        temp = new DynamicButton(assets, new Rect(SizeMaster.IndentButtonMiddle, SizeMaster.ScreenTitleRect.y + SizeMaster.SpacingButtonHalf * 5,
            SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y * 2),
            "Quit Game",
            DyanimcButtonStyle.playCard, standardTextSize, quitGameCallback, assets.playCardIdle, assets.playCardPressed);
        drawableComponents.Add(temp);
    }
    override protected void drawCustom()
    {
        GUI.DrawTexture(SizeMaster.ScreenTitleRect, assets.titleTexture);
    }
    override public void updateCustom()
    {

    }






    public void directConnectCallback(DynamicButton b)
    {
        nextScreen = new ProfileScreen(this);
    }
    public void quitGameCallback(DynamicButton b)
    {
        Application.Quit();
    }
}

