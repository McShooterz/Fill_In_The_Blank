using UnityEngine;
using System.Collections;

public class Profile
{
    string Name;
    int PicID;
    int ColorID;
    public int score = 0;
    string profileID;
    
    public Profile(string s, string id, int i,int c,int ss, AssetManager assets)
    {
        this.Name = s;
        profileID = id;
		this.PicID = i % assets.ProfileIcons.Count;
		this.ColorID = c % assets.ProfileColors.Count;
        score = ss;
    }

    public string getPlayerId()
    {
        return profileID;
    }
    public void setProfileID(string newId)
    {
        profileID = newId;
    }
	
	public string toString(){
		string s = "Join: "+Name+" "+score;
		return s;
	}

    public string GetName()
    {
        return this.Name;
    }

    public int GetPicID()
    {
        return this.PicID;
    }

    public int GetColorID()
    {
        return this.ColorID;
    }

    public void SetName(string name)
    {
        this.Name = name;
    }

    public void SetIcon(int i, AssetManager assets)
    {
        this.PicID = i % assets.ProfileIcons.Count;
    }

    public void SetColor(int c, AssetManager assets)
    {
        this.ColorID = c % assets.ProfileColors.Count;
    }

    public int GetScore()
    {
        return this.score;
    }
}
