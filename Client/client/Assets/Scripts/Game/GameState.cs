using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


struct voteCardStruct
{
    public voteCardStruct(int i, string s)
    {
        cardId = i;
        playerId = s;
    }
    public int cardId;
    public string playerId;
}

class GameState
{
    public Profile player;
    public string playerKey = "";
    //List<Profile> otherPlayers;
    Dictionary<int, Profile> Players = new Dictionary<int, Profile>();

    List<int> playCardIndcies;
    
    List<voteCardStruct> voteCards;

    int situationCardIndex;

    public static float MusicVolume = 1f;
    public static float EffectVolume = 1f;

    public string LobbyName = "";

    public GameState()
    {
        reset();
    }
    public void reset()
    {
        resetOtherPlayers();
        resetPlayCards();
        resetVoteCards();      
    }

    public void setSituationCardIndex(int i)
    {
        situationCardIndex = i;
    }
    public int getSituationCardIndex()
    {
        return situationCardIndex;
    }
    public void resetAll()
    {
        resetOtherPlayers();
        resetPlayCards();
        resetVoteCards();
    }
    public void resetOtherPlayers()
    {
        Players.Clear();
    }
    public void resetPlayCards()
    {
        playCardIndcies = new List<int>();
    }
    public void resetVoteCards()
    {
        voteCards = new List<voteCardStruct>();
    }


    public void addPlayCard(int i)
    {
        playCardIndcies.Add(i);
    }
    public void removePlayCard(int i)
    {
        if (playCardIndcies.Remove(i)== false)//try and remove it as a value
        {
            //try and remove it as an index to a value
            if (i<playCardIndcies.Count)
                 playCardIndcies.Remove(playCardIndcies[i]);
        }
    }

    public int getPlayCardIndex(int i)
    {
        return playCardIndcies[i];
    }
    public int getNumPlayCards()
    {
        return playCardIndcies.Count;
    }


    public int getVoteCardIndex(int i)
    {
        

         return voteCards[i].cardId;      
    }
    public string getVoteCardPlayer(int i)
    {
        return voteCards[i].playerId;
    }
    public int getNumVoteCards()
    {
        return voteCards.Count;
    }

    public void addVoteCard(int i, string s)
    {
        voteCards.Add(new voteCardStruct(i, s));
    }
    public void removeVoteCard(int i)
    {
        for (int x = 0; x< voteCards.Count; x++)
        {
            if (voteCards[x].cardId == i)
            {
                voteCards.Remove(voteCards[x]);
                break;
            }
        }
       
    }

    

    public void addPlayer(int key, Profile p)
    {
        Players.Add(key, p);
    }
    public void removePlayer(int key)
    {
        Players.Remove(key);
    }
    public void removePlayer(Profile profile)
    {
        foreach(KeyValuePair<int, Profile> keyVal in Players)
        {
            if(profile == keyVal.Value)
            {
                Players.Remove(keyVal.Key);
                return;
            }
        }
    }

    public int numPlayers()
    {
        return Players.Count;
    }
    public Profile getProfile(int key)
    {
        return Players[key];
    }


}

