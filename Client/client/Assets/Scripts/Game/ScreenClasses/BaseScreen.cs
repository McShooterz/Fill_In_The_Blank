using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


abstract class BaseScreen
{
    public bool screenLocked = false;

    public BaseScreen nextScreen;
    public NetworkManager network;
    public List<Drawable> drawableComponents;
    public AssetManager assets;
    protected int standardTextSize;

    Rect ScreenRect = new Rect(0, 0, Screen.width, Screen.height);

    public GameState gameState;

    DynamicButton menuIcon;
    DynamicButton OptionsIconButton;

    DynamicTexture PopupBackground;
    DynamicTexture MenuTitleImage;
    static Rect menuOpenPosition = new Rect(Screen.width * .25f, Screen.height * .1f, Screen.width * .45f, Screen.height * .85f);
    static Rect menuClosedPosition = new Rect(menuOpenPosition.x, -menuOpenPosition.height, menuOpenPosition.width, menuOpenPosition.height);
    DynamicButton toMainMenu;
    DynamicButton ExitGame;
    DynamicButton closeMenu;

    DynamicTexture OptionsTitleImage;
    DynamicButton MusicVolumeButton;
    DynamicButton EffectVolumeButton;
    DynamicButton CloseOptionsPopupButton;

    DynamicTexture MusicVolumeIcon;
    DynamicTexture EffectVolumeIcon;

    bool menuUp = false;
    bool isOptionsOpen = false;

    int smallTextSize = (int)(Screen.height * 0.05f);
    public BaseScreen() { }
    public BaseScreen(BaseScreen prevScreen)
    {
        drawableComponents = new List<Drawable>();
        standardTextSize = (int)(Screen.height * 0.032f);
        gameState = prevScreen.gameState;
        network = prevScreen.network;
        assets = prevScreen.assets;
        nextScreen = null;
        initMenu();
    }
    public BaseScreen(NetworkManager n, AssetManager a, GameState gs)
    {
        drawableComponents = new List<Drawable>();
        standardTextSize = (int)(Screen.height * 0.032f);
        gameState = gs;
        network = n;
        assets = a;
        nextScreen = null;
        Particle p = new Particle(Vector2.zero, 20, 20, assets.BackgroundImage, 12);
        p.setAmbient(.5f);
        p.setTargetPosition(new Vector2(50, 50));
        p.setPositionToValue(5f);
        initMenu();
    }

    public void initMenu()
    {
        Rect OptionsButtonRect = new Rect(Screen.width - SizeMaster.IconButtonSize, 0, SizeMaster.IconButtonSize, SizeMaster.IconButtonSize);

        menuIcon = new DynamicButton(assets, new Rect(0, 0, Screen.width * .05f, Screen.width * .05f), "menu", DyanimcButtonStyle.standard, 1,
            bringUpMenu, assets.menuIconTexture, assets.redButtonTexture);
        PopupBackground = new DynamicTexture(assets, menuClosedPosition, assets.situationCardTexture);
        MenuTitleImage = new DynamicTexture(assets, new Rect(PopupBackground.getRect().x, PopupBackground.getRect().y, PopupBackground.getRect().width, PopupBackground.getRect().height * 0.7f), assets.menuTexture);
        OptionsTitleImage = new DynamicTexture(assets, new Rect(PopupBackground.getRect().x, PopupBackground.getRect().y, PopupBackground.getRect().width, PopupBackground.getRect().height * 0.3f), assets.optionsTexture);
        OptionsIconButton = new DynamicButton(assets, OptionsButtonRect, "", DyanimcButtonStyle.standard, 0, OpenOptionsPopup, assets.OptionsIcon, assets.OptionsIcon);
        Rect temp = menuClosedPosition;
        temp.y += Screen.height * .25f;
        temp.x += Screen.width * .05f;
        temp.height = Screen.height * .15f;
        temp.width = Screen.width * .35f;


        toMainMenu = new DynamicButton(assets, temp, "Main Menu", DyanimcButtonStyle.standard, smallTextSize, toMainMenuCallback, assets.playCardIdle, assets.redButtonTexture);
        temp.y = menuClosedPosition.y + Screen.height * .25f * 1.75f;




        ExitGame = new DynamicButton(assets, temp, "Exit Game", DyanimcButtonStyle.standard, smallTextSize, quiteGame, assets.playCardIdle, assets.redButtonTexture);
        temp.y = menuClosedPosition.y + Screen.height * .25f * 2.5f;

        closeMenu = new DynamicButton(assets, temp, "Close Menu", DyanimcButtonStyle.standard, smallTextSize, closeUpMenu, assets.playCardIdle, assets.redButtonTexture);

        MusicVolumeButton = new DynamicButton(assets, new Rect(menuClosedPosition.x, menuClosedPosition.y + Screen.height * .25f, menuClosedPosition.width * 0.65f, Screen.height * .15f), "Music", DyanimcButtonStyle.standard, smallTextSize, changeMusicVolume, assets.playCardIdle, assets.redButtonTexture);
        MusicVolumeIcon = new DynamicTexture(assets, new Rect(MusicVolumeButton.getRect().xMax, MusicVolumeButton.getRect().y, menuClosedPosition.width * 0.25f, Screen.height * .15f), assets.VolumeMax);
        EffectVolumeButton = new DynamicButton(assets, new Rect(menuClosedPosition.x, menuClosedPosition.y + Screen.height * .45f, menuClosedPosition.width * 0.65f, Screen.height * .15f), "Effects", DyanimcButtonStyle.standard, smallTextSize, changeEffectsVolume, assets.playCardIdle, assets.redButtonTexture);
        EffectVolumeIcon = new DynamicTexture(assets, new Rect(EffectVolumeButton.getRect().xMax, EffectVolumeButton.getRect().y, menuClosedPosition.width * 0.25f, Screen.height * .15f), assets.VolumeMax);
        CloseOptionsPopupButton = new DynamicButton(assets, new Rect(Screen.width / 2 - Screen.width * .35f / 2, menuClosedPosition.y + Screen.height * .65f, Screen.width * .35f, Screen.height * .15f), "Close", DyanimcButtonStyle.standard, smallTextSize, CloseOptionsPopup, assets.playCardIdle, assets.redButtonTexture);    
    }

    public void toMainMenuCallback(DynamicButton b)
    {
        if (network.connected)
            network.closeConnection();
        nextScreen = new MainScreen(this);
    }

    public void closeUpMenu()
    {
        menuUp = false;
        PopupBackground.setTargetPosition(new Vector2(menuClosedPosition.x, menuClosedPosition.y));
        PopupBackground.setPositionToValue(15f);

        MenuTitleImage.setTargetPosition(new Vector2(menuClosedPosition.x, menuClosedPosition.y));
        MenuTitleImage.setPositionToValue(15f);

        //temp.x = temp.x + Screen.width * .5f;
        toMainMenu.setTargetPosition(new Vector2(menuClosedPosition.x + Screen.width * .05f, menuClosedPosition.y + Screen.height * .25f));
        toMainMenu.setPositionToValue(15f);

        ExitGame.setTargetPosition(new Vector2(menuClosedPosition.x + Screen.width * .05f, menuClosedPosition.y + Screen.height * .25f * 1.75f));
        ExitGame.setPositionToValue(15f);

        closeMenu.setTargetPosition(new Vector2(menuClosedPosition.x + Screen.width * .05f, menuClosedPosition.y + Screen.height * .25f * 2.5f));
        closeMenu.setPositionToValue(15f);
    }
    public void bringUpMenu()
    {
        menuUp = true;
        PopupBackground.setTargetPosition(new Vector2(menuOpenPosition.x, menuOpenPosition.y));
        PopupBackground.setPositionToValue(15f);

        MenuTitleImage.setTargetPosition(new Vector2(menuOpenPosition.x, menuOpenPosition.y));
        MenuTitleImage.setPositionToValue(15f);

        //temp.x = temp.x + Screen.width * .5f;
        toMainMenu.setTargetPosition(new Vector2(menuOpenPosition.x + Screen.width * .05f, menuOpenPosition.y + Screen.height * .25f));
        toMainMenu.setPositionToValue(15f);

        ExitGame.setTargetPosition(new Vector2(menuOpenPosition.x + Screen.width * .05f, menuOpenPosition.y + Screen.height * .25f * 1.75f));
        ExitGame.setPositionToValue(15f);

        closeMenu.setTargetPosition(new Vector2(menuOpenPosition.x + Screen.width * .05f, menuOpenPosition.y + Screen.height * .25f * 2.5f));
        closeMenu.setPositionToValue(15f);
    }

    public void OpenOptionsPopup()
    {
        isOptionsOpen = true;
        PopupBackground.setTargetPosition(new Vector2(menuOpenPosition.x, menuOpenPosition.y));
        PopupBackground.setPositionToValue(15f);

        OptionsTitleImage.setTargetPosition(new Vector2(menuOpenPosition.x, menuOpenPosition.y));
        OptionsTitleImage.setPositionToValue(15f);

        MusicVolumeButton.setTargetPosition(new Vector2(menuOpenPosition.x, menuOpenPosition.y + Screen.height * .25f));
        MusicVolumeButton.setPositionToValue(15f);

        MusicVolumeIcon.setTargetPosition(new Vector2(MusicVolumeIcon.getRect().x, menuOpenPosition.y + Screen.height * .25f));
        MusicVolumeIcon.setPositionToValue(15f);

        EffectVolumeButton.setTargetPosition(new Vector2(menuOpenPosition.x, menuOpenPosition.y + Screen.height * .25f * 1.75f));
        EffectVolumeButton.setPositionToValue(15f);

        EffectVolumeIcon.setTargetPosition(new Vector2(EffectVolumeIcon.getRect().x, menuOpenPosition.y + Screen.height * .25f * 1.75f));
        EffectVolumeIcon.setPositionToValue(15f);

        CloseOptionsPopupButton.setTargetPosition(new Vector2(menuOpenPosition.x + Screen.width * .05f, menuOpenPosition.y + Screen.height * .25f * 2.5f));
        CloseOptionsPopupButton.setPositionToValue(15f);
    }

    public void CloseOptionsPopup()
    {
        isOptionsOpen = false;
        PopupBackground.setTargetPosition(new Vector2(menuClosedPosition.x, menuClosedPosition.y));
        PopupBackground.setPositionToValue(15f);

        OptionsTitleImage.setTargetPosition(new Vector2(menuClosedPosition.x, menuClosedPosition.y));
        OptionsTitleImage.setPositionToValue(15f);

        MusicVolumeButton.setTargetPosition(new Vector2(menuOpenPosition.x, menuClosedPosition.y + Screen.height * .25f));
        MusicVolumeButton.setPositionToValue(15f);

        MusicVolumeIcon.setTargetPosition(new Vector2(MusicVolumeIcon.getRect().x, menuClosedPosition.y + Screen.height * .25f));
        MusicVolumeIcon.setPositionToValue(15f);

        EffectVolumeButton.setTargetPosition(new Vector2(menuOpenPosition.x, menuClosedPosition.y + Screen.height * .25f * 1.75f));
        EffectVolumeButton.setPositionToValue(15f);

        EffectVolumeIcon.setTargetPosition(new Vector2(EffectVolumeIcon.getRect().x, menuClosedPosition.y + Screen.height * .25f * 1.75f));
        EffectVolumeIcon.setPositionToValue(15f);

        CloseOptionsPopupButton.setTargetPosition(new Vector2(menuOpenPosition.x + Screen.width * .05f, menuClosedPosition.y + Screen.height * .25f *2.5f));
        CloseOptionsPopupButton.setPositionToValue(15f);
    }

    public void setScreenLock(bool l)
    {
        screenLocked = l;
    }
    public void update()
    {
        menuIcon.update();
        foreach (Drawable b in drawableComponents)
        {
            b.update();
        }
        updateCustom();
        PopupBackground.update();
        toMainMenu.update();
        ExitGame.update();
        closeMenu.update();

        MenuTitleImage.update();
        OptionsTitleImage.update();
        MusicVolumeButton.update();
        MusicVolumeIcon.update();
        EffectVolumeButton.update();
        EffectVolumeIcon.update();
        CloseOptionsPopupButton.update();


        //Use escape key to close menu popup
        if ((menuUp || isOptionsOpen) && Input.GetKeyDown(KeyCode.Escape))
        {
            closeUpMenu();
            CloseOptionsPopup();
        }
    }
    public abstract void updateCustom();
    public void draw()
    {
        if (menuUp || isOptionsOpen)
            GUI.enabled = false;
        menuIcon.draw();
        OptionsIconButton.draw();
        GUI.depth = 1;
        drawButtons();
        drawCustom();

        PopupBackground.draw();
        MenuTitleImage.draw();
        OptionsTitleImage.draw();

        GUI.enabled = true;
        toMainMenu.draw();
        ExitGame.draw();
        closeMenu.draw();

        MusicVolumeButton.draw();
        MusicVolumeIcon.draw();
        EffectVolumeButton.draw();
        EffectVolumeIcon.draw();
        CloseOptionsPopupButton.draw();
    }

    public void quiteGame(DynamicButton b)
    {
        Application.Quit();
    }
    protected void drawButtons()
    {
        foreach (Drawable b in drawableComponents)
        {
            b.draw();
        }
    }
    protected abstract void drawCustom();
    public bool hasNextScreen()
    {
        if (screenLocked == false)
        {
            if (nextScreen == null)
                return false;
            return true;
        }
        return false;
    }
    public BaseScreen getNextScreen()
    {
        return nextScreen;
    }

    void changeMusicVolume()
    {
        if(GameState.MusicVolume == 1f)
        {
            GameState.MusicVolume = 0;           
            MusicVolumeIcon.ChangeImage(assets.VolumeMute);
        }
        else if(GameState.MusicVolume == 0f)
        {
            GameState.MusicVolume = 0.33f;
            MusicVolumeIcon.ChangeImage(assets.VolumeLow);
        }
        else if(GameState.MusicVolume == 0.33f)
        {
            GameState.MusicVolume = 0.66f;
            MusicVolumeIcon.ChangeImage(assets.VolumeMedium);
        }
        else if(GameState.MusicVolume == 0.66f)
        {
            GameState.MusicVolume = 1f;
            MusicVolumeIcon.ChangeImage(assets.VolumeMax);
        }

        assets.ChangeMusicVolume(GameState.MusicVolume);
    }

    void changeEffectsVolume()
    {
        if (GameState.EffectVolume == 1f)
        {
            GameState.EffectVolume = 0;
            EffectVolumeIcon.ChangeImage(assets.VolumeMute);
        }
        else if (GameState.EffectVolume == 0f)
        {
            GameState.EffectVolume = 0.33f;
            EffectVolumeIcon.ChangeImage(assets.VolumeLow);
        }
        else if (GameState.EffectVolume == 0.33f)
        {
            GameState.EffectVolume = 0.66f;
            EffectVolumeIcon.ChangeImage(assets.VolumeMedium);
        }
        else if (GameState.EffectVolume == 0.66f)
        {
            GameState.EffectVolume = 1f;
            EffectVolumeIcon.ChangeImage(assets.VolumeMax);
        }

        assets.ChangeMusicVolume(GameState.MusicVolume);
    }
}

