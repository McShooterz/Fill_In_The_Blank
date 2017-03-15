using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

enum buttonStyle
{
    standard,
    highLighted,
}

delegate void void_function_Button(Button b);

delegate void void_function_DyanmicButton(DynamicButton b);
delegate void void_function_empty();
class Button : Drawable
{
    private int value;

    private GUIContent c;


    private void_function_empty function_empty_action = null;
    private void_function_Button function_button_action = null;


    AssetManager assets;
    GUIContent content;
    buttonStyle style;
    int textSize;



    Vector2 position;
    float width;
    float height;
    string text;
    public Button(Vector2 p, float w, float h, string t,  AssetManager a, buttonStyle s, int ts, void_function_empty f)
    {

        init(p, w, h, t, a, s, ts);
        function_empty_action = f;
        
    }
    public Button(Vector2 p, float w, float h, string t,  AssetManager a, buttonStyle s, int ts, void_function_Button f)
    {
        
        function_button_action = f;
        

        init(p, w, h, t, a, s, ts);
    }
    //create a non-functional button.
    public Button(Vector2 p, float w, float h, string t,  AssetManager a, buttonStyle s, int ts )
    {
        init(p, w, h, t, a, s, ts);
    }
    public void init(Vector2 p, float w, float h, string t,  AssetManager a, buttonStyle s, int ts)
    {
        assets = a;
        textSize = ts;
        position = p;
        width = w;
        height = h;
        text = t;
        content = new GUIContent();
        style = s;
    }
    public void setValue(int v)
    {
        value = v;
    }
    public int getValue() { return value; }
    override public void update() { }


    public GUIStyle getStyle()
    {
        GUIStyle s = new GUIStyle(GUI.skin.button);
        s.alignment = TextAnchor.MiddleCenter;
        s.fontSize = textSize;
        s.wordWrap = true;
        s.normal.textColor = Color.black;
        switch (style)
        {
            case buttonStyle.standard:

                break;
            case buttonStyle.highLighted:

                s = new GUIStyle(assets.GUIHighlightSkin.button);
                s.alignment = TextAnchor.MiddleCenter;
                s.fontSize = textSize;
                s.wordWrap = true;
                s.normal.textColor = Color.green;
                
                break;
        }
        return s;
    }
    public void setStyle(buttonStyle newStyle)
    {
        style = newStyle;
    }
    override public void draw()
    {
        c = new GUIContent();

        c.text = text;
        if (GUI.Button(new Rect(position.x, position.y, width, height), c, getStyle()))
        {
            if (function_button_action != null)
            {
                function_button_action(this);
            }
            else if (function_empty_action != null)
            {
                function_empty_action();
            }
            
        }
    }
}

