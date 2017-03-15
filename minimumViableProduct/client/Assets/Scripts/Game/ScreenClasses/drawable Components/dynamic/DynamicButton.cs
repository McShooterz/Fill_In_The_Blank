using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

enum DyanimcButtonStyle
{
    standard,
    playCard,
    voteCard,
}
class DynamicButton:DynamicDrawable
{
    public bool isActive;
    public int value;

    private void_function_empty function_empty_action = null;
    private void_function_DyanmicButton function_button_action = null;
    GUIContent content;
    GUIStyle style;

    DyanimcButtonStyle styleType;
    float textSize;
 
    float width;
    float height;
  

    string text;

    Texture2D idleButtonTexture;
    Texture2D pressedButtonTexture;

    AssetManager assets;


    Texture2D newItemTexture;
    Texture2D winnerTexture;
    bool isWinner = false;
    bool isNew = false;

    float isWinnerDisplay;
    float isNewDisplay;



    Rect winnerRect;
    Rect newItemRect;

    float newItemScale;
    float newItemTargetScale;
    float winnerScale;
    float winnerTargetScale;

    public DynamicButton(AssetManager a, Rect r, string t, DyanimcButtonStyle s, int ts, Texture2D idle, Texture2D pressed) : base(new Vector2(r.x, r.y))
    {
        assets = a;
        init(r, t, s, ts, idle, pressed);
    }
    public DynamicButton(AssetManager a, Rect r, string t,  DyanimcButtonStyle s, int ts, void_function_empty f, Texture2D idle, Texture2D pressed):base(new Vector2(r.x, r.y))
    {
        assets = a;
        function_empty_action = f;
        init(r, t, s, ts, idle, pressed);
    }
    public DynamicButton(AssetManager a, Rect r, string t, DyanimcButtonStyle s, int ts, void_function_DyanmicButton f, Texture2D idle, Texture2D pressed) : base(new Vector2(r.x, r.y))
    {
        assets = a;
        function_button_action = f;
        init(r, t,s,ts,idle,pressed);
    }
    public void init(Rect r, string t, DyanimcButtonStyle s, int ts, Texture2D idle, Texture2D pressed)
    {
        winnerScale = 0;
        newItemScale = 0;
        winnerTargetScale = 0;
        newItemTargetScale = 0;

        newItemTexture = assets.newTexture;
        winnerTexture = assets.winnerTexture;



        content = new GUIContent();
        content.text = text;
        style = null;
        idleButtonTexture = idle;
        pressedButtonTexture = pressed;
        width = r.width;
        height = r.height;
        text = t;
        textSize = ts;

        isActive = true;
        styleType = s;

    }
    public void initStyle()
    {
        
        style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = (int)textSize;
        style.wordWrap = true;
        style.normal.textColor = Color.white;
        if (styleType == DyanimcButtonStyle.voteCard)
            style.normal.textColor = Color.black;

    }



    public override void updateDynamic()
    {
        winnerRect = new Rect(position.x, position.y-height*.8f, width*winnerScale, height*winnerScale);

        newItemRect  = new Rect(position.x, position.y-height*.15f, width*newItemScale*.30f, height*newItemScale*.30f);
    }
    public Rect getRect()
    {
        return new Rect(position.x, position.y, width, height);
    }
    void drawWinnerNew()
    {
        if (isNew)
        {
            newItemTargetScale = 1;
            if (newItemScale != newItemTargetScale)
            {
                newItemScale += .05f;
                if (newItemScale > newItemTargetScale)
                    newItemScale = newItemTargetScale;
            }
            if (isNewDisplay < Time.time)
                isNew = false;
        }
        else
        {
            newItemTargetScale = 0;
            if (newItemScale != newItemTargetScale)
            {
                newItemScale -= .05f;
                if (newItemScale < newItemTargetScale)
                    newItemScale = newItemTargetScale;
            }
        }

        GUI.DrawTexture(newItemRect, newItemTexture, ScaleMode.ScaleToFit);



        if (isWinner)
        {
            winnerTargetScale = 1;
            if (winnerScale != winnerTargetScale)
            {
                winnerScale += .05f;
                if (winnerScale > winnerTargetScale)
                    winnerScale = winnerTargetScale;
            }
            if (isWinnerDisplay < Time.time)
                isWinner = false;
        }
        else
        {
            winnerTargetScale = 0;
            if (winnerScale != winnerTargetScale)
            {
                winnerScale -= .05f;
                if (winnerScale < winnerTargetScale)
                    winnerScale = winnerTargetScale;
            }
        }

        GUI.DrawTexture(winnerRect, winnerTexture, ScaleMode.ScaleToFit);
    }

    public override void draw()
    {
        
        if (isActive)
        {
            GUI.depth += 1;
            initStyle();
            Color newColor = GUI.color;
            GUI.color = new Color(ambient, ambient, ambient, 1);

            //Rect temp = new Rect(position.x*.85f, position.y*.95f, width * scale * 1.25f, height * scale * 1.45f);
            Rect temp = new Rect(position.x, position.y, width * scale, height * scale);
            if (GUI.Button(new Rect(position.x, position.y, width * scale, height * scale), content, style))
            {
                assets.PlayButtonClicked();
                if (function_button_action != null)
                {
                    GUI.DrawTexture(temp, pressedButtonTexture, ScaleMode.StretchToFill);
                    function_button_action(this);
                }
                else if (function_empty_action != null)
                {
                    GUI.DrawTexture(temp, pressedButtonTexture, ScaleMode.StretchToFill);
                    function_empty_action();
                }

            }
            else {
                GUI.DrawTexture(temp, idleButtonTexture, ScaleMode.StretchToFill);
            }
            GUI.Label(new Rect(position.x, position.y, width * scale, height * scale), text, style);

            GUI.color = newColor;
            draw2();
            drawWinnerNew();
        }

    }
    
    public void draw2()
    {

    }

    public void setAsNew(float time)
    {
        isNewDisplay = Time.time + time;
        isNew = true;
    }
    public void setAsWinner(float time)
    {
        isWinnerDisplay = Time.time + time;
        isWinner = true;
    }

    public void ChangeText(string newText)
    {
        text = newText;
    }

    //getters and setters for the callback function
    public void setCallback()
    {
        function_empty_action = null;
        function_button_action = null;
    }
    public void setCallback(void_function_empty f)
    {
        function_empty_action = f;
        function_button_action = null;
    }
    public void setCallback(void_function_DyanmicButton f)
    {
        function_empty_action = null;
        function_button_action = f;
    }

    public float getWidth()
    {
        return width;
    }
    public float getHeight()
    {
        return height;
    }
    
}

