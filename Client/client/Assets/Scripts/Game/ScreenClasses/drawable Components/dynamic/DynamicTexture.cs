using UnityEngine;
using System.Collections;

class DynamicTexture : DynamicDrawable
{
    bool isActive = true;
    AssetManager assetManager;
    string Text;
    float Width;
    float Height;
    float TextSize;
    Texture2D Image;

    //GUI
    GUIContent content = new GUIContent();
    GUIStyle style;
    bool StyleSet = false;

    //Winner and new flags
    Rect winnerRect;
    Rect newItemRect;
    bool isWinner = false;
    bool isNew = false;
    float winnerTimer;
    float newTimer;
    float newItemScale;
    float newItemTargetScale;
    float winnerScale;
    float winnerTargetScale;

    public DynamicTexture(AssetManager assets, Rect rect, Texture2D image) : base(new Vector2(rect.x, rect.y))
    {
        this.assetManager = assets;
        this.Width = rect.width;
        this.Height = rect.height;
        this.Text = "";
        this.TextSize = 0;
        this.Image = image;
    }

    public DynamicTexture(AssetManager assets, Rect rect, string text, int textsize, Texture2D image) : base(new Vector2(rect.x, rect.y))
    {
        this.assetManager = assets;
        this.Width = rect.width;
        this.Height = rect.height;
        this.Text = text;
        this.TextSize = textsize;
        this.Image = image;
    }

    void SetGUIStyle()
    {
        StyleSet = true;
        style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = (int)TextSize;
        style.wordWrap = true;
        style.normal.textColor = Color.white;
    }



    public override void updateDynamic()
    {
        winnerRect = new Rect(position.x, position.y - Height * .8f, Width * winnerScale, Height * winnerScale);

        newItemRect = new Rect(position.x, position.y - Height * .15f, Width * newItemScale * .30f, Height * newItemScale * .30f);

        if (isNew)
        {
            newItemTargetScale = 1;
            if (newItemScale != newItemTargetScale)
            {
                newItemScale += .05f;
                if (newItemScale > newItemTargetScale)
                    newItemScale = newItemTargetScale;
            }
            if (newTimer < Time.time)
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
        if (isWinner)
        {
            winnerTargetScale = 1;
            if (winnerScale != winnerTargetScale)
            {
                winnerScale += .05f;
                if (winnerScale > winnerTargetScale)
                    winnerScale = winnerTargetScale;
            }
            if (winnerTimer < Time.time)
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
    }



    public override void draw()
    {
        if(!StyleSet)
        {
            SetGUIStyle();
        }

        if (isActive)
        {
            GUI.depth += 1;
            Color newColor = GUI.color;
            GUI.color = new Color(ambient, ambient, ambient, 1);
            GUI.DrawTexture(new Rect(position.x, position.y, Width * scale, Height * scale), Image, ScaleMode.StretchToFill);
            if(Text != "")
                GUI.Label(new Rect(position.x, position.y, Width * scale, Height * scale), Text, style);
            GUI.color = newColor;
            drawWinnerNew();
        }

    }

    void drawWinnerNew()
    {
        GUI.DrawTexture(newItemRect, assetManager.newTexture, ScaleMode.ScaleToFit);
        GUI.DrawTexture(winnerRect, assetManager.winnerTexture, ScaleMode.ScaleToFit);
    }

    public Rect getRect()
    {
        return new Rect(position.x, position.y, Width, Height);
    }

    public void setAsNew(float time)
    {
        newTimer = Time.time + time;
        isNew = true;
    }
    public void setAsWinner(float time)
    {
        winnerTimer = Time.time + time;
        isWinner = true;
    }

    public void ChangeText(string newText)
    {
        Text = newText;
    }

    public void ChangeImage(Texture2D texture)
    {
        this.Image = texture;
    }
}
