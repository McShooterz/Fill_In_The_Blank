using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
enum textboxStyle
{
    standard,
}

class TextBox:Drawable
{
    GUIContent content;
    string text;
    Rect rect;
    textboxStyle style;
    public TextBox(float x, float y, float width, float height,  string t, textboxStyle s)
    {
        content = new GUIContent();
        rect = new Rect(x, y, width, height);
        text = t;
        style = s;
    }
    public override void update()
    {
        
    }
    public GUIStyle getStyle()
    {
        GUIStyle s = new GUIStyle(GUI.skin.button);
        switch (style)
        {
            case textboxStyle.standard:

                s.alignment = TextAnchor.MiddleCenter;
                s.fontSize = (int)(Screen.height * 0.0225f);
                s.normal.textColor = Color.black;
                //Text Misc
                s.richText = true;
                break;
        }
        return s;
    }
    public override void draw()
    {
        content.text = text;
        GUI.Box(rect, content, getStyle());
    }

}

