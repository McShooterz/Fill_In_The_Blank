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
        CardsToNormalPositions();

        readyTime = Time.time + 4;
    }

    public override void updateCustom2()
    {

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
    }


    //Card moving Functions *****************************************************************************************************************************************************************************
    public void CardsToNormalPositions()
    {
        float Indent = (Screen.width - SizeMaster.cardSize.x * gameState.getNumPlayCards()) / 2f;
        for (int i = 0; i < gameState.getNumVoteCards(); i++)
        {

            Rect card = new Rect(Indent + SizeMaster.cardSize.x * i, SizeMaster.CardsY, SizeMaster.cardSize.x, SizeMaster.cardSize.y);
            ((DynamicButton)drawableComponents[i]).setTargetPosition(new Vector2(card.x, card.y));
            ((DynamicButton)drawableComponents[i]).setPositionToValue(24f);
            ((DynamicButton)drawableComponents[i]).setTargetAmbient(1);
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
