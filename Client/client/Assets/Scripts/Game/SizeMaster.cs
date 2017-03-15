using UnityEngine;
using System.Collections;

public class SizeMaster : MonoBehaviour
{
    public static Vector2 standardButtonSize;
    public static float IndentButtonMiddle;
    public static float SpacingButtonHalf;
    public static Rect ScreenTitleRect;
    public static Vector2 opponentSize;

    public static Vector2 cardSize;
    public static float IndentSmall;
    public static float IndentButtonLeft;
    public static float SpacingSmall;
    public static float IndentButtonRight;
    public static float SpacingButtonSmall;
    public static Rect PhaseIconRect;
    public static Rect TurnCountRect;
    public static Rect StatementRect;
    public static float opponentsY;
    public static float IconButtonSize;

    public static float CardsY;
    public static int standardTextSize;

    void Start()
    {
        cardSize = new Vector2(Screen.width * 0.195f, Screen.height * 0.21f);
        standardButtonSize = new Vector2(Screen.width * 0.37f, Screen.height * 0.066f);
        opponentSize = new Vector2(Screen.width * 0.19f, Screen.height * 0.054f);
        IconButtonSize = Screen.width * 0.05f;

        IndentButtonMiddle = Screen.width / 2 - standardButtonSize.x / 2;
        SpacingButtonHalf = standardButtonSize.y * 1.5f;
        IndentSmall = Screen.width * 0.08f;
        IndentButtonLeft = Screen.width / 2 - standardButtonSize.x - IndentSmall / 2;
        SpacingSmall = standardButtonSize.y * 0.17f;
        IndentButtonRight = Screen.width / 2 + IndentSmall / 2;
        SpacingButtonSmall = standardButtonSize.y * 1.25f;

        ScreenTitleRect = new Rect(Screen.width * .25f, standardButtonSize.y, Screen.width * 0.5f, Screen.width * 0.5f);
        PhaseIconRect = new Rect(Screen.width / 2 - standardButtonSize.y / 2, 0, standardButtonSize.y, standardButtonSize.y);
        TurnCountRect = new Rect(IndentButtonLeft, PhaseIconRect.y + SpacingButtonSmall, standardButtonSize.x, standardButtonSize.y);
        StatementRect = new Rect(Screen.width * 0.05f, TurnCountRect.y + SpacingButtonSmall, Screen.width - Screen.width * 0.1f, standardButtonSize.y * 2.25f);

        CardsY = StatementRect.y + StatementRect.height + standardButtonSize.y * 0.5f;
        opponentsY = Screen.height - SizeMaster.standardButtonSize.y * 3.6f;
        standardTextSize = (int)(Screen.height * 0.032f);
    }

}
