using UnityEngine;
using System.Collections;

class CreateGameScreen : GamePlayScreen
{
    DynamicButton ChangeModeButton;
    DynamicButton ChangePlayerLimitButton;
    DynamicButton ChangeScoreTargetButton;
    DynamicButton ChangeTurnLimitButton;

    Rect ScreenTitleRect;

    int GameMode = 0;
    int PlayerLimit = 4;
    int PlayerMin = 3;
    int PlayerMax = 5;
    int ScoreTarget = 7;
    int ScoreMin = 0;
    int ScoreMax = 15;
    int TurnsLimit = 10;
    int TurnsMin = 0;
    int TurnsMax = 30;
    int TurnsChange = 2;

    public CreateGameScreen() : base() { }

    public CreateGameScreen(BaseScreen prevScreen) : base(prevScreen)
    {
        ScreenTitleRect = new Rect(Screen.width * .25f, SizeMaster.standardButtonSize.y, Screen.width * 0.5f, SizeMaster.standardButtonSize.y);

        Rect ChangeModeButtonRect = new Rect(SizeMaster.IndentButtonMiddle, ScreenTitleRect.yMax + SizeMaster.standardButtonSize.y * 1.3f, SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y * 1.3f);
        ChangeModeButton = new DynamicButton(assets, ChangeModeButtonRect, "Mode: Anarchy", DyanimcButtonStyle.standard, standardTextSize, ChangeGameMode, assets.playCardIdle, assets.playCardIdle);
        Rect ChangePlayerLimitButtonRect = new Rect(SizeMaster.IndentButtonMiddle, ChangeModeButtonRect.yMax + SizeMaster.SpacingButtonSmall, SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y * 1.3f);
        ChangePlayerLimitButton = new DynamicButton(assets, ChangePlayerLimitButtonRect, "Players: " + PlayerLimit.ToString(), DyanimcButtonStyle.standard, standardTextSize, ChangePlayerLimit, assets.playCardIdle, assets.playCardIdle);
        Rect ChangeScoreTargetButtonRect = new Rect(SizeMaster.IndentButtonMiddle, ChangePlayerLimitButtonRect.yMax + SizeMaster.SpacingButtonSmall, SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y * 1.3f);
        ChangeScoreTargetButton = new DynamicButton(assets, ChangeScoreTargetButtonRect, "Score: " + ScoreTarget.ToString(), DyanimcButtonStyle.standard, standardTextSize, ChangeScoreTarget, assets.playCardIdle, assets.playCardIdle);
        Rect ChangeTurnLimitButtonRect = new Rect(SizeMaster.IndentButtonMiddle, ChangeScoreTargetButtonRect.yMax + SizeMaster.SpacingButtonSmall, SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y * 1.3f);
        ChangeTurnLimitButton = new DynamicButton(assets, ChangeTurnLimitButtonRect, "Turns: " + TurnsLimit.ToString(), DyanimcButtonStyle.standard, standardTextSize, ChangeTurnLimit, assets.playCardIdle, assets.playCardIdle);

        Rect AcceptButtonRect = new Rect(SizeMaster.IndentButtonLeft, ChangeTurnLimitButtonRect.yMax + SizeMaster.SpacingButtonSmall, SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y);
        DynamicButton AcceptButton = new DynamicButton(assets, AcceptButtonRect, "Create", DyanimcButtonStyle.standard, standardTextSize, CreateGame, assets.greenButtonTexture, assets.greenButtonTexture);
        Rect CancelButtonRect = new Rect(SizeMaster.IndentButtonRight, AcceptButtonRect.y, SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y);
        DynamicButton CancelButton = new DynamicButton(assets, CancelButtonRect, "Cancel", DyanimcButtonStyle.standard, standardTextSize, Cancel, assets.redButtonTexture, assets.redButtonTexture);


        drawableComponents.Add(AcceptButton);
        drawableComponents.Add(CancelButton);

        DrawPlayerIcons = false;
    }

    public override void updateCustom2()
    {
        
    }

    protected override void drawCustom2()
    {
        GUI.DrawTexture(ScreenTitleRect, assets.newTexture);

        ChangeModeButton.draw();
        ChangePlayerLimitButton.draw();
        ChangeScoreTargetButton.draw();
        ChangeTurnLimitButton.draw();
    }

    void CreateGame()
    {
        nextScreen = new ConnectingScreen(this);
    }

    void Cancel()
    {
        nextScreen = new JoinGameScreen((GamePlayScreen)this);
    }

    void ChangeGameMode()
    {
        GameMode++;
        if(GameMode == 3)
        {
            GameMode = 0;
            ChangeModeButton.ChangeText("Mode: Anarchy");
        }
        if(GameMode == 1)
        {
            ChangeModeButton.ChangeText("Mode: Democracy");
        }
        if(GameMode == 2)
        {
            ChangeModeButton.ChangeText("Mode: Monarchy");
        }
    }

    void ChangePlayerLimit()
    {
        PlayerLimit++;
        if(PlayerLimit > PlayerMax)
        {
            PlayerLimit = PlayerMin;
        }
        ChangePlayerLimitButton.ChangeText("Players: " + PlayerLimit.ToString());
    }

    void ChangeScoreTarget()
    {
        ScoreTarget++;
        if (ScoreTarget > ScoreMax)
        {
            ScoreTarget = ScoreMin;
            ChangeScoreTargetButton.ChangeText("Score: Unlimited");
        }
        else
        {
            ChangeScoreTargetButton.ChangeText("Score: " + ScoreTarget.ToString());
        }
    }

    void ChangeTurnLimit()
    {
        TurnsLimit += TurnsChange;
        if (TurnsLimit > TurnsMax)
        {
            TurnsLimit = TurnsMin;
            ChangeTurnLimitButton.ChangeText("Turns: Unlimited");
        }
        else
        {
            ChangeTurnLimitButton.ChangeText("Turns: " + TurnsLimit.ToString());
        }
    }
}
