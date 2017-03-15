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
        int numCards = gameState.getNumVoteCards();
        for (int i = 0; i < numCards; i++)
        {
            Rect card = new Rect((Indent + SizeMaster.cardSize.x * i+1), Screen.height + SizeMaster.cardSize.y * (gameState.getNumVoteCards() - i), SizeMaster.cardSize.x, SizeMaster.cardSize.y);
            string cardText = "";
            cardText = assets.GetPlayCardText(gameState.getVoteCardIndex(i));

            DynamicButton voteCard = new DynamicButton(assets, card, cardText, DyanimcButtonStyle.playCard, standardTextSize, cardCallback, assets.playCardIdle, assets.playCardPressed);
            voteCard.value = i;
            voteCard.setAsNew(5);

            //Add to lists for better management
            VoteCards.Add(voteCard);
            if(gameState.getVoteCardPlayer(i) != gameState.player.getPlayerId())
            {
                OpponentVoteCards.Add(voteCard);
            }
        }

        //Order Vote cards list
        OrderVoteCards();
        ScrambleOpponentCards();

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
        foreach(DynamicButton card in VoteCards)
        {
            card.update();
        }
        if (cardConfirmed==true)
        {
            bool cardsMoved = true;
            foreach(DynamicButton card in OpponentVoteCards)
            {
                if (card.atTargetPosition() == false)
                {
                    cardsMoved = false;
                }
            }
            if (cardsMoved == true)
            {
                string playerId = gameState.getVoteCardPlayer(selectedCard);
                sendMessage("vt: " + playerId + "\n");
                cardConfirmed = false;
            }
        }
    }
    protected override void drawCustom2()
    {
        phaseTexture.draw();
        confirmButton.draw();
        declineButton.draw();
        foreach (DynamicButton card in VoteCards)
        {
            card.draw();
        }
    }

    void OrderVoteCards()
    {
        DynamicButton temp = null;
        for(int i = 0; i < PlayerIcons.Count; i++)
        {
            string playerID = PlayerIcons[i].getPlayerID();
            for(int k = i + 1; k < VoteCards.Count; k++)
            {
                if(gameState.getVoteCardPlayer(VoteCards[k].getValue()) == playerID)
                {
                    temp = VoteCards[i];
                    VoteCards[i] = VoteCards[k];
                    VoteCards[k] = temp;
                }
            }
        }
    }

    void ScrambleOpponentCards()
    {
        DynamicButton temp = null;
        for(int i = 0; i < OpponentVoteCards.Count; i++)
        {
            int Random = UnityEngine.Random.Range(0, OpponentVoteCards.Count);
            temp = OpponentVoteCards[i];
            OpponentVoteCards[i] = OpponentVoteCards[Random];
            OpponentVoteCards[Random] = temp;
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
        // cardsToExitPosition();
        RevealPlayerVoteCard();
        confirmButton.isActive = false;
        declineButton.isActive = false;
        phaseTexture.setTargetPosition(new Vector2(Screen.width * .5f - SizeMaster.cardSize.x * .5f, -(SizeMaster.cardSize.y * 1.52f)));
        screenLocked = true;
        //((DynamicButton)drawableComponents[selectedCard]).setTargetPosition(new Vector2(0, 0));      
    }
    public void playCardCallback()
    {
        //sendMessage("vt: " + selectedCard+"\n");
        cardConfirmed = true;
    }

    //Card moving functions **************************************************************************************************************************************************************************
    public void CardsToNormalPositions()
    {
        float Indent = (Screen.width - SizeMaster.cardSize.x * OpponentVoteCards.Count) / 2f;
        for (int i = 0; i < OpponentVoteCards.Count; i++)
        {
            OpponentVoteCards[i].setTargetPosition(new Vector2(Indent + SizeMaster.cardSize.x * i, SizeMaster.CardsY));
            OpponentVoteCards[i].setPositionToValue(12f);
            OpponentVoteCards[i].setTargetAmbient(1);
            OpponentVoteCards[i].setCallback(cardCallback);
        }
    }
    public void RevealPlayerVoteCard()
    {
        float Indent = (Screen.width - SizeMaster.cardSize.x * VoteCards.Count) / 2f;
        for (int i = 0; i < VoteCards.Count; i++)
        {
            VoteCards[i].setTargetPosition(new Vector2(Indent + SizeMaster.cardSize.x * i, SizeMaster.CardsY));
            VoteCards[i].setPositionToValue(12f);
            VoteCards[i].setTargetAmbient(1);
            VoteCards[i].setCallback(cardCallback);
        }
    } 
    public void CardsToFoldedPositions()
    {
        for (int i = 0; i < OpponentVoteCards.Count; i++)
        {
            OpponentVoteCards[i].setTargetPosition(new Vector2(Screen.width - (OpponentVoteCards.Count - i) * SizeMaster.cardSize.x * 0.2f, SizeMaster.CardsY));
            OpponentVoteCards[i].setPositionToValue(12f);
            OpponentVoteCards[i].setTargetAmbient(.5f);
            OpponentVoteCards[i].setCallback();
        }
    }
}

