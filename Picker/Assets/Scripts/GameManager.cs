using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PickerStatic;

public class GameManager : MonoBehaviour {
    private static GameManager instance;
    public static GameManager Instance {
        get { return instance; }
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private AudioSystem audioSystem;

    #region Picker Game Config
    [Header("Config")]
    [SerializeField]
    private ScriptablePickerConfig config;
    [SerializeField]
    private int numOfPickAvailable = 2;
    #endregion

    #region Picker Options
    [Header("Options")]
    [SerializeField]
    private GameObject pickerOptionPrefab;
    [SerializeField]
    private Transform pickerOptionsParent;
    #endregion

    #region Current Game State
    public enum GameStatus {
        Intro,
        InGame,
        Summary,
        Outro
    }
    private GameStatus currentStatus = GameStatus.Intro;
    public GameStatus CurrentStatus {
        get { return currentStatus; }
    }

    private ModelPlayer currentPlayer;
    #endregion

    #region Intro
    [Header("Intro")]
    [SerializeField]
    private GameObject menuCanvas;
    [SerializeField]
    private Text playerCoinText;
    #endregion

    #region InGame
    [Header("InGame")]
    [SerializeField]
    private GameObject inGameCanvas;
    [SerializeField]
    private Text numOfPickText;

    private List<GameObject> pickerOptionObjs;
    public List<GameObject> PickerOptionObjs {
        get { return pickerOptionObjs; }
    }
    private List<Tuple<PickerOption.OptionChoices, int>> pickerOptions;
    private int numOfPickRemains;
    private int totalWin;
    #endregion

    #region Summary
    [Header("Summary")]
    [SerializeField]
    private GameObject summaryCanvas;
    [SerializeField]
    private Text totalWinText;
    [SerializeField]
    private AudioClip revealRestSound;
    #endregion

    #region Outro
    [Header("Outro")]
    [SerializeField]
    private GameObject outroCanvas;
    #endregion

    public void Start() {
        audioSystem = AudioSystem.Instance;
        if (!audioSystem) {
            Debug.LogError("Cannot find AudioSystem!");
        }

        currentStatus = GameStatus.Intro;

        menuCanvas.SetActive(true);
        inGameCanvas.SetActive(false);
        summaryCanvas.SetActive(false);
        outroCanvas.SetActive(false);

        LocalStorage.LoadUserInfo(out currentPlayer);
    }

    private void Update() {
        if (currentPlayer != null) {
            playerCoinText.text = currentPlayer.Coins.ToString();
        }

        numOfPickText.text = numOfPickRemains + (numOfPickRemains > 0 ? " Picks" : " Pick");
        totalWinText.text = totalWin.ToString();
    }

    public void StartGame() {
        currentStatus = GameStatus.InGame;

        menuCanvas.SetActive(false);
        inGameCanvas.SetActive(true);
        summaryCanvas.SetActive(false);
        outroCanvas.SetActive(false);

        totalWin = 0;
        numOfPickRemains = numOfPickAvailable;
        pickerOptions = Randomize.RandomOptions(config);
        pickerOptions.Shuffle();

        pickerOptionObjs = new List<GameObject>();
        for (int i = 0; i < config.NumOfOptions; i++) {
            GameObject pickerOptionObj = Instantiate(pickerOptionPrefab, pickerOptionsParent);
            PickerOption pickerOption = pickerOptionObj.GetComponent<PickerOption>();
            if (pickerOption) {
                pickerOption.SetUpOption(pickerOptions[i].Item1, pickerOptions[i].Item2);
            } else {
                Debug.LogError("Picker Option prefab should have the PickerOption component.");
            }
            pickerOptionObjs.Add(pickerOptionObj);
        }
    }

    public void AddCoins(int increment) {
        if (currentStatus != GameStatus.InGame) { return; }

        currentPlayer.AddCoins(increment);
        totalWin += increment;
    }

    public void AddNumOfPick(int increment) {
        if (currentStatus != GameStatus.InGame) { return; }

        numOfPickRemains += increment;

        if (numOfPickRemains <= 0) {
            StartCoroutine(EndPickerGame());
        } else {
            bool allChosen = true;
            foreach (GameObject obj in pickerOptionObjs) {
                if (!obj.GetComponent<PickerOption>().Chosen) {
                    allChosen = false;
                    break;
                }
            }

            if (allChosen) {
                StartCoroutine(EndPickerGame());
            }
        }
    }

    public IEnumerator EndPickerGame() {
        currentStatus = GameStatus.Summary;

        foreach (GameObject obj in pickerOptionObjs) {
            obj.GetComponent<PickerOption>().ChooseOption();
        }
        audioSystem.PlaySoundEffect(revealRestSound);

        LocalStorage.SaveUser(currentPlayer);

        yield return new WaitForSeconds(2f);

        summaryCanvas.SetActive(true);
    }

    public void GoToOutro() {
        currentStatus = GameStatus.Outro;

        outroCanvas.SetActive(true);
        foreach (GameObject obj in pickerOptionObjs) {
            Destroy(obj);
        }
        pickerOptionObjs.Clear();
        pickerOptions.Clear();
    }
}
