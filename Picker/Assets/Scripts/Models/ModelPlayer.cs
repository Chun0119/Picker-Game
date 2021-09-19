using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModelPlayer {
    private int coins;

    public ModelPlayer() {
        coins = 0;
    }

    public ModelPlayer(int coins) {
        this.coins = coins;
    }

    public int Coins {
        get { return coins; }
    }

    public void AddCoins(int increment) {
        coins += increment;
    }
}
