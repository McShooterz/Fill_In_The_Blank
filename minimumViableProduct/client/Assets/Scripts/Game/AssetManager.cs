using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AssetManager : MonoBehaviour
{

    public Texture2D menuIconTexture;
    public Texture2D gameOverTexture;

    public List<Texture2D> particleTextures = new List<Texture2D>();

    public Texture2D menuTexture;

    public Texture2D playACardTexture;
    public Texture2D voteForACardTexture;
    public Texture2D voteResultsTexture;

    public Texture2D plusOneTexture;

    public Texture2D newTexture;
    public Texture2D winnerTexture;

    public Texture2D titleTexture;
    public Texture2D situationCardTexture;

    public Texture2D greenButtonTexture;
    public Texture2D redButtonTexture;

    public Texture2D playCardIdle;
    public Texture2D playCardPressed;


    //Assets
    public TextAsset FillCardList;
    public TextAsset StatementList;

    public Texture2D LoadingScreenImage;
    public Texture2D BackgroundImage;
    public Texture2D PopupBackgroundImage;

    public Texture2D menuIcon;

    public Texture2D QuitIcon;
    public Texture2D OptionsIcon;
    public Texture2D EditIcon;
    public Texture2D CheckboxIcon;
    public Texture2D PhasePlayIcon;
    public Texture2D PhaseVoteIcon;
    public Texture2D PhaseSummeryIcon;

    public List<Texture2D> ProfileIcons = new List<Texture2D>();
    public List<Color> ProfileColors = new List<Color>();

    public AudioClip ButtonClickSound;

    public AudioSource audioSource;
    public AudioSource musicSource;

    public GUISkin GUIHighlightSkin;


    List<string> FillCards = new List<string>();
    List<string> Statements = new List<string>();

    //styles
    bool loadedStyles = false;

    // Use this for initialization
    void Start()
    {

        LoadAll();

    }



    // Update is called once per frame
    void Update()
    {

    }

    public void PlayButtonClicked()
    {
        audioSource.PlayOneShot(ButtonClickSound, 1f);
    }

    public void ChangeMusicVolume(float f)
    {
        musicSource.volume = f;
    }

    public string GetPlayCardText(int index)
    {
        return FillCards[index];
    }
    public string GetSituationCardText(int index)
    {
        return Statements[index];
    }

    public string GetFormattedStatement(int index, int FillIndex)
    {
        return Statements[index];
    }

    public void LoadAll()
    {
        loadFillCards();
        loadStatements();
    }

    void loadFillCards()
    {
        string[] FileLines = FillCardList.text.Split('\n');
        foreach (string s in FileLines)
        {
            FillCards.Add(s);
        }
    }

    void loadStatements()
    {
        string[] FileLines = StatementList.text.Split('\n');
        foreach (string s in FileLines)
        {
            Statements.Add(s);
        }
    }
}
