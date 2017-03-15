﻿using UnityEngine;
using System.Collections;

class ProfileScreen : BaseScreen
{
    //Screen needs buttons for changing Icon, color; a text field for name, and a sample playerIcon that changes when the buttons and text field are used

    string ProfileName = "";
    Rect NameField;

    public ProfileScreen()
    {

    }
    public ProfileScreen(BaseScreen prevScreen) : base(prevScreen)
    {
        init();
    }
    public ProfileScreen(NetworkManager n, AssetManager a, GameState gs) : base(n, a,gs)
    {
        init();
    }

    public void init()
    {
        Vector2 IconLocation = new Vector2(Screen.width / 2f - SizeMaster.opponentSize.x / 2f, Screen.height / 2f - SizeMaster.opponentSize.y / 2f);
        NameField = new Rect(SizeMaster.IndentButtonRight, IconLocation.y + SizeMaster.opponentSize.y * 1.25f, SizeMaster.opponentSize.x, SizeMaster.standardButtonSize.y);
        Rect NameButtonRect = new Rect(SizeMaster.IndentButtonLeft, NameField.y, SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y);
        Rect IconButtonRect = new Rect(SizeMaster.IndentButtonLeft, NameButtonRect.y + SizeMaster.SpacingButtonHalf, SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y);
        Rect ColorButtonRect = new Rect(SizeMaster.IndentButtonRight, IconButtonRect.y, SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y);
        Rect AcceptButtonRect = new Rect(SizeMaster.IndentButtonLeft, ColorButtonRect.y + SizeMaster.SpacingButtonHalf, SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y);
        Rect CancelButtonRect = new Rect(SizeMaster.IndentButtonRight, AcceptButtonRect.y, SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y);

        //Setup playerIcon
        PlayerIcon Icon = new PlayerIcon(gameState.player, IconLocation, assets);
        drawableComponents.Add(Icon);
        DynamicButton Button = new DynamicButton(assets, NameButtonRect, "Update Name", DyanimcButtonStyle.standard, standardTextSize, ChangeProfileName, assets.greenButtonTexture, assets.greenButtonTexture);
        drawableComponents.Add(Button);
        Button = new DynamicButton(assets, IconButtonRect, "Change Icon", DyanimcButtonStyle.standard, standardTextSize, ChangeProfileIcon, assets.greenButtonTexture, assets.greenButtonTexture);
        drawableComponents.Add(Button);
        Button = new DynamicButton(assets, ColorButtonRect, "Change Color", DyanimcButtonStyle.standard, standardTextSize, ChangeProfileColor, assets.greenButtonTexture, assets.greenButtonTexture);
        drawableComponents.Add(Button);
        Button = new DynamicButton(assets, AcceptButtonRect, "Accept", DyanimcButtonStyle.standard, standardTextSize, AcceptProfile, assets.greenButtonTexture, assets.greenButtonTexture);
        drawableComponents.Add(Button);
        Button = new DynamicButton(assets, CancelButtonRect, "Cancel", DyanimcButtonStyle.standard, standardTextSize, Cancel, assets.redButtonTexture, assets.redButtonTexture);
        drawableComponents.Add(Button);

        ProfileName = gameState.player.GetName();
    }
    override protected void drawCustom()
    {
        if (ProfileName.Length > 10)
        {
            ProfileName = ProfileName.Remove(10);
        }
        ProfileName = GUI.TextField(NameField, ProfileName, 12);
    }
    override public void updateCustom()
    {

    }

    void ChangeProfileName()
    {
        if (ProfileName == "")
        {
            ProfileName = "Player";
        }
        gameState.player.SetName(ProfileName);
    }

    void ChangeProfileIcon()
    {
        gameState.player.SetIcon(gameState.player.GetPicID() + 1, assets);
    }

    void ChangeProfileColor()
    {
        gameState.player.SetColor(gameState.player.GetColorID() + 1, assets);
    }

    void AcceptProfile()
    {
        nextScreen = new JoinGameScreen(this);
    }

    void Cancel()
    {
        nextScreen = new MainScreen(this);
    }
}