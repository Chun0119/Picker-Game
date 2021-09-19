using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PickerStatic;

[RequireComponent(typeof(Animator), typeof(Collider2D), typeof(EventTrigger))]
public class PickerOption : MonoBehaviour {
    private GameManager gameManager;
    private AudioSystem audioSystem;

    private void Awake() {
        gameManager = GameManager.Instance;
        audioSystem = AudioSystem.Instance;

        if (!gameManager) {
            Debug.LogError("Cannot find GameManager!");
        }
    }

    public enum OptionChoices {
        CoinRewards,
        PickerRewards,
        EndPicker
    }

    private bool chosen;
    public bool Chosen {
        get { return chosen; }
    }
    private OptionChoices option;
    private int optionValue = -1;

    [SerializeField]
    private SpriteRenderer optionImage;
    [SerializeField]
    private Sprite[] optionIcons;
    [SerializeField]
    private Text optionText;
    [SerializeField]
    private AudioClip clickedSound;

    public void SetUpOption(OptionChoices option, int optionValue) {
        this.option = option;
        this.optionValue = optionValue;

        switch (option) {
            case OptionChoices.CoinRewards:
                optionText.text = "+ " + optionValue;
                break;
            case OptionChoices.PickerRewards:
                optionText.text = "+" + optionValue + " Pick" + (optionValue > 1 ? "s" : string.Empty);
                break;
            case OptionChoices.EndPicker:
                optionText.text = "Game Over";
                break;
        }

        optionImage.sprite = optionIcons[Randomize.RandomInt(0, optionIcons.Length - 1)];
    }

    public void ChooseOption() {
        if (chosen) { return; }

        if (gameManager.CurrentStatus == GameManager.GameStatus.InGame) {
            audioSystem.PlaySoundEffect(clickedSound);
        }

        GetComponent<Animator>().SetBool("IsChosen", true);
        foreach (GameObject obj in gameManager.PickerOptionObjs) {
            obj.GetComponent<Collider2D>().enabled = false;
        }
        chosen = true;
    }

    private void ChosenOption() {
        if (gameManager.CurrentStatus != GameManager.GameStatus.InGame) { return; }

        switch (option) {
            case OptionChoices.CoinRewards:
                gameManager.AddCoins(optionValue);
                break;
            case OptionChoices.PickerRewards:
                gameManager.AddNumOfPick(optionValue);
                break;
            case OptionChoices.EndPicker:
                StartCoroutine(gameManager.EndPickerGame());
                break;
        }

        foreach (GameObject obj in gameManager.PickerOptionObjs) {
            obj.GetComponent<Collider2D>().enabled = true;
        }
        gameManager.AddNumOfPick(-1);
    }
}
