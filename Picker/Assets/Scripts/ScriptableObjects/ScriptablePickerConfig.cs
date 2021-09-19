using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickerConfig", menuName = "Custom Scriptable Object/Create Picker Config", order = 0)]
public class ScriptablePickerConfig : ScriptableObject {
    [Header("Picker Settings")]
    [SerializeField]
    private int numOfOptions = 9;

    [Header("Coin Rewards")]
    [SerializeField]
    private bool enableCoinRewards = true;
    [SerializeField]
    private float coinWeighting = 0.6f;
    [SerializeField]
    private int minCoinValue = 1, maxCoinValue = 10, roundCoinAt = 1;

    [Header("Picker Rewards")]
    [SerializeField]
    private bool enablePickerRewards = true;
    [SerializeField]
    private float pickerWeighting = 0.3f;
    [SerializeField]
    private int minPickerValue = 1, maxPickerValue = 3;

    [Header("End Picker")]
    [SerializeField]
    private bool enableEndPicker = true;
    [SerializeField]
    private float endPickerWeighting = 0.1f;

    #region Properties
    // Limited to 9 options due to UI constraint
    public int NumOfOptions {
        get { return Mathf.Min(numOfOptions, 9); }
    }

    public float WeightSum {
        get {
            float sum = 0;
            sum += enableCoinRewards ? coinWeighting : 0;
            sum += enablePickerRewards ? pickerWeighting : 0;
            sum += enableEndPicker ? endPickerWeighting : 0;
            return sum;
        }
    }

    public float CoinRewardsProbability {
        get { return enableCoinRewards ? coinWeighting / WeightSum : 0; }
    }

    public float PickerRewardsProbability {
        get { return enablePickerRewards ? pickerWeighting / WeightSum : 0; }
    }

    public float EndPickerProbability {
        get { return enableEndPicker ? endPickerWeighting / WeightSum : 0; }
    }

    public int MinCoinValue {
        get { return minCoinValue; }
    }

    public int MaxCoinValue {
        get { return maxCoinValue; }
    }

    public int RoundCoinAtValue {
        get { return roundCoinAt; }
    }

    public int MinPickerValue {
        get { return minPickerValue; }
    }

    public int MaxPickerValue {
        get { return maxPickerValue; }
    }
    #endregion
}

