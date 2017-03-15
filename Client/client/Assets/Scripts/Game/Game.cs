using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


//This class will be the core of our game, and the only running script,
//it will contain, an instance of a Screen, an instance of an Assets class and an instance of the Network class.
class Game : MonoBehaviour
{
    public AssetManager assets;
    public BaseScreen currentScreen;
    public NetworkManager network;

    public GUIContent content;
    Rect ScreenRect = new Rect(0, 0, Screen.width, Screen.height);
    GameState gameState;

    PlayerIcon testIcon;
    public AudioSource musicSource;
    public AudioClip music;

    int smallTextSize = (int)(Screen.height * 0.05f);
    void Start()
    {
        musicSource.playOnAwake = true;
        musicSource.PlayOneShot(music);


        Profile player = new Profile("player","", 0, 0,0, assets);
        gameState = new GameState();
        gameState.player = player;
        assets.LoadAll();



		

        /*gameState.addPlayCard(1);
        gameState.addPlayCard(2);
        gameState.addPlayCard(3);
        gameState.addPlayCard(3);
        gameState.addPlayCard(3);
        gameState.setSituationCardIndex(3);
        gameState.addVoteCard(1);
        gameState.addVoteCard(2);
        gameState.addVoteCard(3);

        Profile op1 = new Profile("timmy", 1, 0);
        gameState.addPlayer(op1);
        op1 = new Profile("timmy", 1, 0);
        gameState.addPlayer(op1); gameState.addPlayer(op1); gameState.addPlayer(op1);
        */


        content = new GUIContent();
        network = new NetworkManager(1100);
        currentScreen = new MainScreen(network, assets, gameState);
        // currentScreen = new VoteCardScreen(currentScreen);
        //currentScreen = new PlayCardScreen(currentScreen);
    }
    void Update()
    {
        
        currentScreen.update();

        if (currentScreen.hasNextScreen())
        {
            currentScreen = currentScreen.getNextScreen();
        }

    }

    void OnGUI()
    {
        currentScreen.draw();
    }
}

