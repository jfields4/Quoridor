using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOptions : MonoBehaviour
{
    public Button bt_character_1, bt_character_2, bt_character_3, bt_character_4, bt_dificult_1, bt_dificult_2, bt_first_1, bt_first_2;
    private ColorBlock theColor, defaultColorGreen, defaultColor, theColorDificult1, theColorDificult2, defaultColorRed;
    private Color orange = new Color(1f, 0.5829f, 0f);
    private Color green = new Color(0f, 0.8867924f, 0.09454501f);
    private Color red = new Color(0.9622642f, 0f, 0f);

    public GameSettings options;

    void Start()
    {
        bt_character_1.onClick.AddListener(SelectCharacter1);
        bt_character_2.onClick.AddListener(SelectCharacter2);
        bt_character_3.onClick.AddListener(SelectCharacter3);
        bt_character_4.onClick.AddListener(SelectCharacter4);
        bt_dificult_1.onClick.AddListener(SelectDifficulty1);
        bt_dificult_2.onClick.AddListener(SelectDifficulty2);
        bt_first_1.onClick.AddListener(SelectFirst1);
        bt_first_2.onClick.AddListener(SelectFirst2);


        theColor = bt_character_1.colors;
        theColorDificult1 = bt_dificult_1.colors;
        theColorDificult2 = bt_dificult_2.colors;
        defaultColor = bt_character_1.colors;

        defaultColorGreen = bt_dificult_1.colors;
        defaultColorRed = bt_dificult_2.colors;

        options = GameObject.FindObjectOfType<GameSettings>();
    }

    public void ResetCharacterColors()
    {
        bt_character_1.colors = defaultColor;
        bt_character_2.colors = defaultColor;
        bt_character_3.colors = defaultColor;
        bt_character_4.colors = defaultColor;
    }

    public void ResetFirstColors()
    {
        bt_first_1.colors = defaultColorGreen;
        bt_first_2.colors = defaultColorRed;
    }

    public void ResetDifficultyColors()
    {
        bt_dificult_1.colors = defaultColorGreen;
        bt_dificult_2.colors = defaultColorRed;
    }

    public void SelectCharacter1()
    {
        ResetCharacterColors();
        theColor.normalColor = orange;
        bt_character_1.colors = theColor;
        AppController.Character = 0;
    }

    public void SelectCharacter2()
    {
        ResetCharacterColors();
        theColor.normalColor = orange;
        bt_character_2.colors = theColor;
        AppController.Character = 1;
    }

    public void SelectCharacter3()
    {
        ResetCharacterColors();
        theColor.normalColor = orange;
        bt_character_3.colors = theColor;
        AppController.Character = 2;
    }

    public void SelectCharacter4()
    {
        ResetCharacterColors();
        theColor.normalColor = orange;
        bt_character_4.colors = theColor;
        AppController.Character = 3;
    }

    public void SelectDifficulty1()
    {
        ResetDifficultyColors();
        theColor.normalColor = green;
        bt_dificult_1.colors = theColor;

        options.SetAIToEasy();
        Debug.Log("AI Hard option: " + options.AIHard);
    }

    public void SelectDifficulty2()
    {
        ResetDifficultyColors();
        theColor.normalColor = orange;
        bt_dificult_2.colors = theColor;

        options.SetAIToHard();
        Debug.Log("AI Hard option: " + options.AIHard);
    }

    public void SelectFirst1()
    {
        ResetFirstColors();
        theColor.normalColor = orange;
        bt_first_1.colors = theColor;
    }

    public void SelectFirst2()
    {
        ResetFirstColors();
        theColor.normalColor = orange;
        bt_first_2.colors = theColor;
    }
}
