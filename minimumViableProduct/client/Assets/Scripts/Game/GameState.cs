using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class GameState
{
    public Profile player;
    public string playerKey = "";
    //List<Profile> otherPlayers;
    Dictionary<int, Profile> Players = new Dictionary<int, Profile>();

    List<int> playCardIndcies;
    List<int> voteCardIndicies;

    int situationCardIndex;

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
        voteCardIndicies = new List<int>();
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
        return voteCardIndicies[i];
    }
    public int getNumVoteCards()
    {
        return voteCardIndicies.Count;
    }

    public void addVoteCard(int i)
    {
        voteCardIndicies.Add(i);
    }
    public void removeVoteCard(int i)
    {
        if (voteCardIndicies.Remove(i) == false)//try and remove it as a value
        {
            //try and remove it as an index to a value
            if (i < voteCardIndicies.Count)
                voteCardIndicies.Remove(voteCardIndicies[i]);
        }
    }

    //Swap index of first and last vote cards for player
    public void swapVoteCards()
    {
        int temp = voteCardIndicies[3];
        voteCardIndicies[3] = voteCardIndicies[0];
        voteCardIndicies[0] = temp;
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

