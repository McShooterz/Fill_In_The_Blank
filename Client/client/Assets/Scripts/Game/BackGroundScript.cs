using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class BackGroundScript : MonoBehaviour
{
    public Texture2D backgroundImage;
    public List<Particle> particles;
    public List<Texture2D> particleTextures = new List<Texture2D>();
    Rect ScreenRect = new Rect(0, 0, Screen.width, Screen.height);
    int maxParticles = 15;
    void Start()
    {
        UnityEngine.Random.seed=(int)Time.time;
        particles = new List<Particle>();
    }

    void Update()
    {
        if (particles.Count < maxParticles)
        {
            int randomTexture = (int)UnityEngine.Random.Range(0, particleTextures.Count);
            float randomSize = (UnityEngine.Random.Range(20, 100));

            int side = UnityEngine.Random.Range(-1, 1);
            float x = 0;
            if (side == -1)
            {
                x = -randomSize;
            }
            else
            {
                x = ScreenRect.width + randomSize;
            }
            Vector2 startPos = new Vector2(x, UnityEngine.Random.Range(0, ScreenRect.height));

            if (side == -1)
            {
                x = ScreenRect.width + randomSize;
            }
            else
            {
                x = -randomSize;
            }
            Vector2 endPos = new Vector2(x, UnityEngine.Random.Range(0, ScreenRect.height));


            

            Particle p = new Particle(startPos, randomSize, randomSize,
                particleTextures[randomTexture], endPos);
            p.setPositionToValue(randomSize*.1f);
            float ambient = UnityEngine.Random.Range(.15f, .5f);
            p.setAmbient(ambient);
            p.setTargetAmbient(ambient);
            particles.Add(p);
            
        }
        foreach (Particle p in particles)
            p.update();

    }

    void OnGUI()
    {
        GUI.depth = 50;
        GUI.DrawTexture(ScreenRect, backgroundImage, ScaleMode.StretchToFill);
        foreach (Particle p in particles)
        {
            p.draw();            
        }
        for (int i =0; i < particles.Count; i++)
        {
            if (particles[i].getKillParticle())
            {
                particles.Remove(particles[i]);
                i--;
            }
        }
    }
}

