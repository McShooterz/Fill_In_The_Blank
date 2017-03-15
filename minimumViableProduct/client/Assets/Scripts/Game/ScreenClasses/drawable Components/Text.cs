using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

enum textStyle
{
    title,
    standard
}
class Text:Drawable
{
    private string text;
    private Vector2 position;
    private float width, height;
    textStyle textStyle;
    public Text(string t, Vector2 pos, float w, float h, textStyle s)
    {
        textStyle = s;
        text = t;
        position = pos;
        width = w;
        height = h;

    }
    public override void update()
    {
        
    }

    protected GUIStyle getStyle()
    {
        int titleTextSize = (int)(Screen.height * 0.076f);
        GUIStyle style = new GUIStyle();
        switch (textStyle)
        {
            case textStyle.title:
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.black;
                style.fontSize = titleTextSize;
                break;
        }
        
        
        return style;
    }
    public override void draw()
    {
        
        GUI.Label(new Rect(position.x, position.y, width, height), text, getStyle());
    }
}

