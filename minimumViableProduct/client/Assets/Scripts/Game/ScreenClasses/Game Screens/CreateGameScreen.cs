using UnityEngine;
using System.Collections;

class CreateGameScreen : GamePlayScreen
{

    public CreateGameScreen() : base() { }

    public CreateGameScreen(BaseScreen prevScreen) : base(prevScreen)
    {
        DynamicButton AcceptButton = new DynamicButton(assets, new Rect(), "Accept", DyanimcButtonStyle.standard, standardTextSize, assets.greenButtonTexture, assets.greenButtonTexture);
        DynamicButton CancelButton = new DynamicButton(assets, new Rect(), "Cancel", DyanimcButtonStyle.standard, standardTextSize, assets.redButtonTexture, assets.redButtonTexture);
        DynamicButton ChangeModeButton = new DynamicButton(assets, new Rect(), "Mode", DyanimcButtonStyle.standard, standardTextSize, assets.greenButtonTexture, assets.greenButtonTexture);
        DynamicButton ChangePlayerLimitButton = new DynamicButton(assets, new Rect(), "Players", DyanimcButtonStyle.standard, standardTextSize, assets.greenButtonTexture, assets.greenButtonTexture);
        DynamicButton ChangeScoreTargetButton = new DynamicButton(assets, new Rect(), "Score", DyanimcButtonStyle.standard, standardTextSize, assets.greenButtonTexture, assets.greenButtonTexture);
        DynamicButton ChangeTurnLimitButton = new DynamicButton(assets, new Rect(), "Turns", DyanimcButtonStyle.standard, standardTextSize, assets.greenButtonTexture, assets.greenButtonTexture);



        drawableComponents.Add(AcceptButton);
        drawableComponents.Add(CancelButton);
        drawableComponents.Add(ChangeModeButton);
        drawableComponents.Add(ChangePlayerLimitButton);
        drawableComponents.Add(ChangeScoreTargetButton);
        drawableComponents.Add(ChangeTurnLimitButton);
    }

    public override void updateCustom2()
    {

    }

    protected override void drawCustom2()
    {

    }

    void CreateGame()
    {

    }

    void Cancel()
    {

    }

    void ChangeGameMode()
    {

    }

    void ChangePlayerLimit()
    {

    }

    void ChangeScoreTarget()
    {

    }

    void ChangeTurnLimit()
    {

    }
}
