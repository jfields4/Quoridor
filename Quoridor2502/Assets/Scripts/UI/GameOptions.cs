using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOptions : MonoBehaviour
{
    public Button bt_character_1, bt_character_2, bt_character_3, bt_dificult_1, bt_dificult_2, bt_dificult_3;
    private ColorBlock theColor, defaultColorGreen, defaultColor, theColorDificult1, theColorDificult3, defaultColorRed;
    private Color orange = new Color(1f, 0.5829f, 0f);
    private Color green = new Color(0f, 0.8867924f, 0.09454501f);
    private Color red = new Color(0.9622642f, 0f, 0f);

    void Start()
    {
        bt_character_1.onClick.AddListener(SelectCharacter1);
        bt_character_2.onClick.AddListener(SelectCharacter2);
        bt_character_3.onClick.AddListener(SelectCharacter3);
        bt_dificult_1.onClick.AddListener(SelectDifficulty1);
        bt_dificult_2.onClick.AddListener(SelectDifficulty2);
        bt_dificult_3.onClick.AddListener(SelectDifficulty3);
        theColor = bt_character_1.colors;
        theColorDificult1 = bt_dificult_1.colors;
        theColorDificult3 = bt_dificult_3.colors;
        defaultColor = bt_character_1.colors;
        defaultColorGreen = bt_dificult_1.colors;
        defaultColorRed = bt_dificult_3.colors;

    }

    public void ResetCharacterColors() {
        bt_character_1.colors = defaultColor;
        bt_character_2.colors = defaultColor;
        bt_character_3.colors = defaultColor;
    }

    public void ResetDifficultyColors()
    {
        bt_dificult_1.colors = defaultColorGreen;
        bt_dificult_2.colors = defaultColor;
        bt_dificult_3.colors = defaultColorRed;
    }

    public void SelectCharacter1() {
        ResetCharacterColors();
        theColor.normalColor = orange;
        bt_character_1.colors = theColor;
    }

    public void SelectCharacter2()
    {
        ResetCharacterColors();
        theColor.normalColor = orange;
        bt_character_2.colors = theColor;
    }

    public void SelectCharacter3()
    {
        ResetCharacterColors();
        theColor.normalColor = orange;
        bt_character_3.colors = theColor;
    }
    public void SelectDifficulty1()
    {
        ResetDifficultyColors();
        theColorDificult1.normalColor = green;
        bt_dificult_1.colors = theColorDificult1;
    }

    public void SelectDifficulty2()
    {
        ResetDifficultyColors();
        theColor.normalColor = orange;
        bt_dificult_2.colors = theColor;
    }

    public void SelectDifficulty3()
    {
        ResetDifficultyColors();
        theColorDificult3.normalColor = red;
        bt_dificult_3.colors = theColorDificult3;
    }

}
