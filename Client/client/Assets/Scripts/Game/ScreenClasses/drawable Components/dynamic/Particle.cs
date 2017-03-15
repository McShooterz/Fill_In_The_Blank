using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


enum ParticleKillCondition
{
    time,
    position
}

class Particle:DynamicDrawable
{
    bool killParticle;
    ParticleKillCondition killCondition;
    private float toKillTime;
    Texture2D texture;
    float width, height;


    public Particle (Vector2 p, float w, float h, Texture2D t,  float killTime) : base(p)
    {
        texture = t;
        killParticle = false;
        toKillTime = Time.time + killTime;
        killCondition = ParticleKillCondition.time;
        width = w;
        height = h;
    }
    public Particle(Vector2 p, float w, float h, Texture2D t, Vector2 targetPos) : base(p)
    {
        texture = t;
        killParticle = false;
        setTargetPosition(targetPos);
        killCondition = ParticleKillCondition.position;
        width = w;
        height = h;
    }
    public bool getKillParticle()
    {
        return killParticle;
    }
    public override void updateDynamic()
    {
        switch (killCondition)
        {
            case ParticleKillCondition.time:
                if (Time.time > toKillTime)
                    killParticle = true;
                break;
            case ParticleKillCondition.position:
                if (atTargetPosition())
                    killParticle = true;
                break;
        }
    }

    public override void draw()
    {
        Color newColor = GUI.color;
        newColor.a = ambient;
        GUI.color = newColor;
        newColor.a = 1;
        GUI.DrawTexture(new Rect(position.x, position.y, width, height), texture, ScaleMode.StretchToFill);

        GUI.color = newColor;
    }
}

