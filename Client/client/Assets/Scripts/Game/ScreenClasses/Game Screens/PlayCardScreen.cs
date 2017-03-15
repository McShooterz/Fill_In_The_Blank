using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class PlayCardScreen : GamePlayScreen
{
    static int numTurn = 0;
    int selectedCard = -1;
    DynamicButton confirmButton;
    DynamicButton declineButton;
    bool cardConfirmed = false;
    DynamicButton situationCard;
    DynamicButton phaseTexture;
    List<DynamicButton> PlayCards = new List<DynamicButton>();

    public PlayCardScreen()
    {

    }
    public PlayCardScreen(BaseScreen prevScreen) : base(prevScreen)
    {
        phaseTexture = new DynamicButton(assets, new Rect(Screen.width*.5f-SizeMaster.cardSize.x*.5f, -(SizeMaster.cardSize.y*1.52f), SizeMaster.cardSize.x, SizeMaster.cardSize.x),
            "", DyanimcButtonStyle.standard, 0, assets.playACardTexture, assets.playACardTexture);
        phaseTexture.setTargetPosition(new Vector2(Screen.width * .5f - SizeMaster.cardSize.x * .3f, -(SizeMaster.cardSize.y * .45f)));
        phaseTexture.setPositionToValue(1.2f);
        //add the play cards
        float Indent = (Screen.width - SizeMaster.cardSize.x * gameState.getNumPlayCards()) / 2f;
        for (int i = 0; i < gameState.getNumPlayCards(); i++)
        {
            Rect card = new Rect((Indent + SizeMaster.cardSize.x * i), Screen.height + SizeMaster.cardSize.y*(gameState.getNumPlayCards()- i), SizeMaster.cardSize.x, SizeMaster.cardSize.y);
            string cardText = assets.GetPlayCardText(gameState.getPlayCardIndex(i));         
           // drawableComponents.Add(cardButton);
            DynamicButton PlayerCard = new DynamicButton(assets, card, cardText, DyanimcButtonStyle.playCard, standardTextSize,cardCallback, assets.playCardIdle, assets.playCardPressed);
            PlayerCard.value = i;
            if (numTurn==0)
            {
                PlayerCard.setAsNew(10);
            }
            else
            {
                if (i == gameState.getNumPlayCards() - 1)
                    PlayerCard.setAsNew(10);
            }
            PlayCards.Add(PlayerCard);       
        }

        DrawPlayerIcons = true;
        numTurn++;
        CardsToNormalPositions();

        //add the situation card
        
        situationCard = new DynamicButton(assets,new Rect( SizeMaster.StatementRect.x, -SizeMaster.StatementRect.height, SizeMaster.StatementRect.width,SizeMaster.StatementRect.height), assets.GetSituationCardText(gameState.getSituationCardIndex()), DyanimcButtonStyle.voteCard, standardTextSize,
            assets.situationCardTexture, assets.situationCardTexture);
        situationCard.setTargetPosition(new Vector2(SizeMaster.StatementRect.x, SizeMaster.StatementRect.y));
        situationCard.setAsNew(10);
        situationCard.setPositionToValue(2f);
        situationCard.ChangeText(FormatStatement(assets.GetSituationCardText(gameState.getSituationCardIndex()), ""));
        drawableComponents.Add(situationCard);

        
        //create the confirm and deny buttons
        Rect temp = new Rect(Screen.width * .5f - SizeMaster.standardButtonSize.x, Screen.height * .5f + SizeMaster.standardButtonSize.y,
            SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y);
        confirmButton = new DynamicButton(assets, temp, "confirm", DyanimcButtonStyle.playCard, standardTextSize, confirmCallBack, assets.greenButtonTexture, assets.greenButtonTexture);
        confirmButton.isActive = false;

        temp = new Rect(Screen.width * .5f, Screen.height * .5f + SizeMaster.standardButtonSize.y,
            SizeMaster.standardButtonSize.x, SizeMaster.standardButtonSize.y);
        declineButton = new DynamicButton(assets, temp, "decline", DyanimcButtonStyle.playCard, standardTextSize, declineCallBack, assets.redButtonTexture, assets.redButtonTexture);
        declineButton.isActive = false;


    }

    public override void updateCustom2()
    {
        if (screenLocked== true)
        {
            if (((DynamicButton)drawableComponents[selectedCard]).atTargetPosition())
            {
                screenLocked = false;
            }
        }
        phaseTexture.update();
        confirmButton.update();
        declineButton.update();
        foreach(DynamicButton card in PlayCards)
        {
            card.update();
        }
        if (cardConfirmed == true)
        {
            bool cardsMoved = true;
            foreach (DynamicButton card in PlayCards)
            {
                if (card.atTargetPosition() == false)
                {
                    cardsMoved = false;
                }
            }
            if (cardsMoved == true)
            {
                playCardCallback();
            }
        }
    }
    protected override void drawCustom2()
    {
        phaseTexture.draw();
        confirmButton.draw();
        declineButton.draw();
        foreach (DynamicButton card in PlayCards)
        {
            card.draw();
        }
    }


    //Button Callback functions ************************************************************************************************************************************************************************
    public void cardCallback(DynamicButton b)
    {
        CardsToFoldedPositions();
        b.setTargetPosition(new Vector2(Screen.width * .5f - SizeMaster.cardSize.x * 0.5f, SizeMaster.CardsY));
        b.setTargetAmbient(1);
        confirmButton.isActive = true;
        declineButton.isActive = true;
        selectedCard = b.value;
        situationCard.ChangeText(FormatStatement(assets.GetSituationCardText(gameState.getSituationCardIndex()), assets.GetPlayCardText(gameState.getPlayCardIndex(selectedCard))));
    }

    public void declineCallBack(DynamicButton b)
    {

        confirmButton.isActive = false;
        declineButton.isActive = false;
        CardsToNormalPositions();
        selectedCard = -1;
        situationCard.ChangeText(FormatStatement(assets.GetSituationCardText(gameState.getSituationCardIndex()), ""));
    }

    public void confirmCallBack(DynamicButton b)
    {
        cardConfirmed = true;
        cardsToExitPosition();
        confirmButton.isActive = false;
        declineButton.isActive = false;

        ((DynamicButton)drawableComponents[selectedCard]).setTargetPosition(new Vector2(Screen.width * .5f, -SizeMaster.cardSize.y));




        phaseTexture.setTargetPosition(new Vector2(Screen.width * .5f - SizeMaster.cardSize.x * .5f, -(SizeMaster.cardSize.y * 1.52f)));
        phaseTexture.setPositionToValue(1.2f);
        setScreenLock(true);
    }
    public void playCardCallback()
    {
        sendMessage("ply: " +
            gameState.getPlayCardIndex(selectedCard));
        gameState.removePlayCard(gameState.getPlayCardIndex(selectedCard));
        cardConfirmed = false;
    }

    //Card Moving Functions **********************************************************************************************************************************************************************
    public void CardsToNormalPositions()
    {
        float Indent = (Screen.width - SizeMaster.cardSize.x * PlayCards.Count) / 2f;
        for (int i = 0; i < PlayCards.Count; i++)
        {
            PlayCards[i].setTargetPosition(new Vector2(Indent + SizeMaster.cardSize.x * i, SizeMaster.CardsY));
            PlayCards[i].setPositionToValue(12f);
            PlayCards[i].setTargetAmbient(1);
            PlayCards[i].setCallback(cardCallback);
        }
    }

    public void CardsToFoldedPositions()
    {
        for (int i = 0; i < PlayCards.Count; i++)
        {
            PlayCards[i].setTargetPosition(new Vector2(Screen.width - (PlayCards.Count - i) * SizeMaster.cardSize.x * 0.2f, SizeMaster.CardsY));
            PlayCards[i].setPositionToValue(12f);
            PlayCards[i].setTargetAmbient(.5f);
            PlayCards[i].setCallback();
        }
    }

    public void cardsToExitPosition()
    {
        float Indent = (Screen.width - SizeMaster.cardSize.x * PlayCards.Count) / 2f;
        for (int i = 0; i < PlayCards.Count; i++)
        {
            PlayCards[i].setTargetPosition(new Vector2(Indent + SizeMaster.cardSize.x * i, Screen.height + SizeMaster.cardSize.y * (gameState.getNumPlayCards() - i)));
            PlayCards[i].setPositionToValue(12f);
            PlayCards[i].setTargetAmbient(1f);
            PlayCards[i].setCallback();
        }
    }
}

