using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class VoteCardScreen : GamePlayScreen
{
    int selectedCard = -1;
    DynamicButton confirmButton;
    DynamicButton declineButton;
    bool cardConfirmed = false;
    DynamicButton situationCard;

    DynamicButton phaseTexture;
    public VoteCardScreen()
    {

    }
    public VoteCardScreen(BaseScreen prevScreen) : base(prevScreen)
    {
        phaseTexture = new DynamicButton(assets, new Rect(Screen.width * .5f - SizeMaster.cardSize.x * .5f, -(SizeMaster.cardSize.y * 1.52f), SizeMaster.cardSize.x, SizeMaster.cardSize.x),
            "", DyanimcButtonStyle.standard, 0, assets.voteForACardTexture, assets.voteForACardTexture);
        phaseTexture.setTargetPosition(new Vector2(Screen.width * .5f - SizeMaster.cardSize.x * .3f, -(SizeMaster.cardSize.y * .45f)));
        phaseTexture.setPositionToValue(1.2f);
        //add the play cards
        float Indent = (Screen.width - SizeMaster.cardSize.x * gameState.getNumVoteCards()) / 2f;
        for (int i = 0; i < gameState.getNumPlayCards(); i++)
        {

            Rect card = new Rect((Indent + SizeMaster.cardSize.x * i), Screen.height + SizeMaster.cardSize.y * (gameState.getNumVoteCards() - i), SizeMaster.cardSize.x, SizeMaster.cardSize.y);
            string cardText = assets.GetPlayCardText(gameState.getVoteCardIndex(i));

            // drawableComponents.Add(cardButton);


            DynamicButton b = new DynamicButton(assets, card, cardText, DyanimcButtonStyle.playCard, standardTextSize, cardCallback, assets.playCardIdle, assets.playCardPressed);
            b.value = i;
            b.setAsNew(5);
            drawableComponents.Add(b);
        }

        CardsToNormalPositions();

        //add the situation card

        situationCard = new DynamicButton(assets, SizeMaster.StatementRect, assets.GetSituationCardText(gameState.getSituationCardIndex()), DyanimcButtonStyle.voteCard, standardTextSize,
            assets.situationCardTexture, assets.situationCardTexture);
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
        if (screenLocked == true)
        {
            if (phaseTexture.atTargetPosition())
                screenLocked = false;
        }
        phaseTexture.update();
        confirmButton.update();
        declineButton.update();
        if (cardConfirmed==true)
        {
            bool cardsMoved = true;
            for (int i =0; i < gameState.getNumVoteCards(); i++)
            {
                if (((DynamicButton)drawableComponents[i]).atTargetPosition()== false)
                {
                    cardsMoved = false;
                }
            }
            if (cardsMoved == true)
            {
                sendMessage("vt: " + selectedCard + "\n");
                cardConfirmed = false;
            }
        }
    }
    protected override void drawCustom2()
    {
        phaseTexture.draw();
        confirmButton.draw();
        declineButton.draw();
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
        //Change statement to reflect selected fill card
        situationCard.ChangeText(FormatStatement(assets.GetSituationCardText(gameState.getSituationCardIndex()), assets.GetPlayCardText(gameState.getVoteCardIndex(selectedCard))));
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
        playCardCallback();
        cardsToExitPosition();
        CardsToNormalPositions();
        confirmButton.isActive = false;
        declineButton.isActive = false;
        phaseTexture.setTargetPosition(new Vector2(Screen.width * .5f - SizeMaster.cardSize.x * .5f, -(SizeMaster.cardSize.y * 1.52f)));
        screenLocked = true;
        //((DynamicButton)drawableComponents[selectedCard]).setTargetPosition(new Vector2(0, 0));

        //Show player vote card
        RevealPlayerVoteCard();
    }
    public void playCardCallback()
    {
        //sendMessage("vt: " + selectedCard+"\n");
        cardConfirmed = true;
    }

    //Card moving functions **************************************************************************************************************************************************************************
    public void CardsToNormalPositions()
    {
        float Indent = (Screen.width - SizeMaster.cardSize.x * (PlayerIcons.Count - 1)) / 2f;
        for (int i = 0; i < gameState.getNumVoteCards(); i++)
        {
            //Hide players vote card
            if (i == 3)
            {
                ((DynamicButton)drawableComponents[i]).setTargetAmbient(0);
                ((DynamicButton)drawableComponents[i]).setAmbientToValue(8f);
                ((DynamicButton)drawableComponents[i]).setCallback(cardCallback);
            }
            else
            {
                Rect card = new Rect(Indent + SizeMaster.cardSize.x * i, SizeMaster.CardsY, SizeMaster.cardSize.x, SizeMaster.cardSize.y);
                ((DynamicButton)drawableComponents[i]).setTargetPosition(new Vector2(card.x, card.y));
                ((DynamicButton)drawableComponents[i]).setPositionToValue(8f);
                ((DynamicButton)drawableComponents[i]).setTargetAmbient(1);
                ((DynamicButton)drawableComponents[i]).setCallback(cardCallback);
            }
        }
    }

    public void RevealPlayerVoteCard()
    {
        float Indent = (Screen.width - SizeMaster.cardSize.x * gameState.getNumPlayCards()) / 2f;
        Rect card = new Rect(Indent, SizeMaster.CardsY, SizeMaster.cardSize.x, SizeMaster.cardSize.y);
        ((DynamicButton)drawableComponents[0]).setTargetPosition(new Vector2(card.x, card.y));
        ((DynamicButton)drawableComponents[0]).setPositionToValue(8f);
        ((DynamicButton)drawableComponents[0]).setTargetAmbient(1);
        ((DynamicButton)drawableComponents[0]).setCallback();
    } 

    public void CardsToFoldedPositions()
    {

        float Indent = (Screen.width - SizeMaster.cardSize.x * gameState.getNumPlayCards()) / 2f;
        float x = Screen.width;
        for (int i = 0; i < gameState.getNumVoteCards(); i++)
        {
            x = Screen.width - ((DynamicButton)drawableComponents[i]).getWidth() * (gameState.getNumPlayCards() - i) * .25f;
            Rect card = new Rect(x, SizeMaster.CardsY, SizeMaster.cardSize.x, SizeMaster.cardSize.y);
            ((DynamicButton)drawableComponents[i]).setTargetPosition(new Vector2(card.x, card.y));
            ((DynamicButton)drawableComponents[i]).setPositionToValue(8f);

            ((DynamicButton)drawableComponents[i]).setTargetAmbient(.5f);
            ((DynamicButton)drawableComponents[i]).setCallback();
        }
    }
    public void cardsToExitPosition()
    {

        float Indent = (Screen.width - SizeMaster.cardSize.x * gameState.getNumPlayCards()) / 2f;
        for (int i = 0; i < gameState.getNumVoteCards(); i++)
        {
            Rect card = new Rect((Indent + SizeMaster.cardSize.x * i), Screen.height + SizeMaster.cardSize.y * (gameState.getNumPlayCards() - i), SizeMaster.cardSize.x, SizeMaster.cardSize.y);
            ((DynamicButton)drawableComponents[i]).setTargetPosition(new Vector2(card.x, card.y));
            ((DynamicButton)drawableComponents[i]).setPositionToValue(8f);

            ((DynamicButton)drawableComponents[i]).setTargetAmbient(1f);
            ((DynamicButton)drawableComponents[i]).setCallback();
        }
    }
}

