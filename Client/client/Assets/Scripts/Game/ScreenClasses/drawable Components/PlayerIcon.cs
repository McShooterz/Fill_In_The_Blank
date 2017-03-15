using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PlayerIcon:DynamicDrawable
{
    List<Particle> particles = new List<Particle>();

    Profile profile;

    GUIContent content;
    AssetManager assets;

    Rect NameBox;
    Rect IconBox; 
    Rect ScoreBox;
    Rect winnerRect;
    Rect newItemRect;
    Rect KingCrownRect;


    Texture2D newItemTexture;
    Texture2D winnerTexture;
    Texture2D nameBoxTex = new Texture2D(1, 1);
    bool isWinner = false;
    bool isNew = false;
    bool isKing = false;

    float isWinnerDisplay;
    float isNewDisplay;


    float newItemScale;
    float newItemTargetScale;
    float winnerScale;
    float winnerTargetScale;

    int actualScore = 0;

    float nextIncrement=0;

    

    
    public PlayerIcon(Profile p, Vector2 pos,  AssetManager a):base(pos)
    {
        content = new GUIContent();
        assets = a;
        profile = p;
        position = pos;
        init();
        winnerScale = 0;
        newItemScale = 0;
        winnerTargetScale = 0;
        newItemTargetScale = 0;
        actualScore = p.GetScore();
    }
    public void init()
    {
       

        Vector2 standardButtonSize = new Vector2(Screen.width * 0.37f, Screen.height * 0.066f);
        Vector2 opponentSize = new Vector2(Screen.width * 0.19f, Screen.height * 0.054f);
        float opponentsY = Screen.height - standardButtonSize.y * 3.6f;
        float IconSize = Screen.height * 0.075f;
        float ScoreSize = 0;



        NameBox = new Rect(position.x, position.y, opponentSize.x, opponentSize.y);
        IconBox = new Rect(NameBox.x + opponentSize.x / 2 - IconSize / 2, opponentsY - IconSize, IconSize, IconSize);
        ScoreBox = new Rect(NameBox.x + opponentSize.x - ScoreSize * 2.5f, IconBox.y, ScoreSize, ScoreSize);
        KingCrownRect = new Rect(IconBox.x, IconBox.y - IconSize / 2, IconSize, IconSize);
        scale = 1;
        ambient = 1;
        scaleToValue = .005f;
        ambientToValue = .0001f;
        positionToValue = 1;
        targetScale = scale;
        targetPosition = position;
        targetAmbient = ambient;

        newItemTexture = assets.newTexture;
        winnerTexture = assets.winnerTexture;
        isWinner = false;
        isNew = false;

    }
    public string getPlayerID()
    {
        return profile.getPlayerId();
    }
    public void reCalcBoxes()
    {
        Vector2 standardButtonSize = new Vector2(Screen.width * 0.37f, Screen.height * 0.066f);
        Vector2 opponentSize = new Vector2(Screen.width * 0.19f, Screen.height * 0.054f);
        float opponentsY = Screen.height - standardButtonSize.y * 3.6f;
        float IconSize = Screen.height * 0.075f;
        float ScoreSize = 0;



        NameBox = new Rect(position.x, position.y, opponentSize.x*scale, opponentSize.y*scale);
        IconBox = new Rect(NameBox.x + opponentSize.x / 2 - IconSize / 2, NameBox.y - IconSize*scale, IconSize*scale, IconSize*scale);
        ScoreBox = new Rect(NameBox.x + opponentSize.x - ScoreSize * 2.5f, IconBox.y, ScoreSize*scale, ScoreSize*scale);
        KingCrownRect.position = new Vector2(IconBox.x, IconBox.y - IconSize / 2);

        winnerRect = new Rect(NameBox.x, (NameBox.y - opponentSize.y * 3f*winnerScale), NameBox.width * winnerScale, NameBox.height * winnerScale);
        
        
        newItemRect = new Rect(NameBox.x-IconSize*newItemScale, (NameBox.y - opponentSize.y * 2f*newItemScale), NameBox.width * newItemScale, NameBox.height * newItemScale);
    }
    public override void updateDynamic()
    {
        updateScore();
        foreach (Particle p in particles)
        {
            p.update();
        }
    }

    public override void draw()
    {
        reCalcBoxes();
        int smallTextSize = (int)(Screen.height * 0.05f);
        int tinyTextSize = (int)(Screen.height * 0.0188f);

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = (int)smallTextSize;
        style.wordWrap = true;
        style.normal.textColor = Color.white;

        GUIStyle ScoreStyle= new GUIStyle();
        ScoreStyle.alignment = TextAnchor.MiddleCenter;
        ScoreStyle.fontSize = smallTextSize;
        ScoreStyle.normal.textColor = Color.white;


        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.black;

        //draw the user icon and name box, apply player color
		GUI.color = assets.ProfileColors[profile.GetColorID()];
        GUI.DrawTexture(IconBox, assets.particleTextures[1], ScaleMode.StretchToFill);
        content.text = profile.GetName();
        GUI.DrawTexture(NameBox, nameBoxTex, ScaleMode.StretchToFill);
        GUI.color = Color.white;
        GUI.DrawTexture(IconBox, assets.ProfileIcons[profile.GetPicID()]);
        GUI.Label(NameBox, profile.GetName(), style);

        //Draw the kings crown
        if(isKing)
        {
            GUI.color = Color.yellow;
            GUI.DrawTexture(KingCrownRect, assets.ProfileIcons[10]);
            GUI.color = Color.white;
        }

        //draw the score
        content.image = null;
        content.text = profile.GetScore().ToString();
        GUI.Box(ScoreBox, content, ScoreStyle);

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
        drawParticles();
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

    public void setAsKing(bool trueOrFalse)
    {
        isKing = trueOrFalse;
    }

    public void setScore(int newScore)
    {
        profile.score =newScore;
    }
    public int getScore()
    {
        return profile.score;
    }

    public void updateScore()
    {
        if (profile.GetScore() != actualScore)
        {
            if (Time.time > nextIncrement)
            {
                nextIncrement = Time.time + .3f;
                profile.score++;
            }
            Particle p = new Particle(new Vector2(ScoreBox.x, ScoreBox.y), ScoreBox.width, ScoreBox.width, assets.plusOneTexture, 1);
            p.setTargetPosition(new Vector2(Screen.width*.5f, Screen.height*.5f));
            p.setPositionToValue(.5f);
            p.setAmbient(1);
            p.setTargetAmbient(1); 
            particles.Add(p);
        }
    }

    public void drawParticles()
    {
        foreach (Particle p in particles)
        {
            p.draw();
        }
        for (int i = 0; i < particles.Count; i++)
        {
            if (particles[i].getKillParticle())
            {
                particles.Remove(particles[i]);
                i--;
            }
        }
        
    }

    public void incrementScore(int newScore)
    {
        actualScore = newScore;
    }

    public bool CheckProfile(Profile profile)
    {
        return this.profile == profile;
    }
}

