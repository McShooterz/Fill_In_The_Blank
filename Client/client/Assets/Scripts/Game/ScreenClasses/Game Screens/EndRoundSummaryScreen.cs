using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class EndRoundSummaryScreen : GamePlayScreen
{
    float readyTime;
    BaseScreen temp = null;

    DynamicButton phaseTexture;
    public EndRoundSummaryScreen()
    {

    }

    public EndRoundSummaryScreen(BaseScreen prevScreen) : base(prevScreen)
    {
        phaseTexture = new DynamicButton(assets, new Rect(Screen.width * .5f - SizeMaster.cardSize.x * .5f, -(SizeMaster.cardSize.y * 1.52f), SizeMaster.cardSize.x, SizeMaster.cardSize.x),
            "", DyanimcButtonStyle.standard, 0, assets.voteResultsTexture, assets.voteResultsTexture);
        phaseTexture.setTargetPosition(new Vector2(Screen.width * .5f - SizeMaster.cardSize.x * .3f, -(SizeMaster.cardSize.y * .35f)));
        phaseTexture.setPositionToValue(1.2f);
        //Add situation
        screenLocked = true;
        drawableComponents = ((GamePlayScreen)prevScreen).drawableComponents;
        VoteCards = ((GamePlayScreen)prevScreen).VoteCards;

        readyTime = Time.time + 4;
    }

    public override void updateCustom2()
    {
        foreach (DynamicButton card in VoteCards)
        {
            card.update();
        }
        phaseTexture.update();
        if (readyTime < Time.time)
        {
            if (temp != null)
            {
                nextScreen = temp;
            }
            screenLocked = false;
        }
        else
        {
            if (readyTime - 1.5f < Time.time)
            {

                phaseTexture.setTargetPosition(new Vector2(Screen.width * .5f - SizeMaster.cardSize.x * .5f, -(SizeMaster.cardSize.y * 1.52f)));
                cardsToExitPosition();
                ((DynamicButton)drawableComponents[drawableComponents.Count-1]).setTargetPosition(new Vector2(SizeMaster.StatementRect.x, -SizeMaster.StatementRect.height));

                ((DynamicButton)drawableComponents[drawableComponents.Count - 1]).setPositionToValue(2f);
            }
            if (nextScreen != null)
            {
                temp = nextScreen;
                nextScreen = null;
            }
            
        }
    }

    protected override void drawCustom2()
    {
        phaseTexture.draw();
        foreach (DynamicButton card in VoteCards)
        {
            card.draw();
        }
    }


    //Card moving Functions *****************************************************************************************************************************************************************************
    public void cardsToExitPosition()
    {
        float Indent = (Screen.width - SizeMaster.cardSize.x * VoteCards.Count) / 2f;
        for (int i = 0; i < VoteCards.Count; i++)
        {
            VoteCards[i].setTargetPosition(new Vector2(Indent + SizeMaster.cardSize.x * i, Screen.height + SizeMaster.cardSize.y * (VoteCards.Count - i)));
            VoteCards[i].setPositionToValue(12f);
            VoteCards[i].setTargetAmbient(1f);
            VoteCards[i].setCallback();
        }
    }
}
