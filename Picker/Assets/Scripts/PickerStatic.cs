using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace PickerStatic {
    public static class Randomize {
        public static System.Random random = new System.Random();

        public static int RandomInt(int min, int max, int round = 1) {
            int range = Mathf.RoundToInt((float)(max - min) / (float)round);
            if (range < 0) {
                Debug.LogError("Range smaller than 0: " + range);
                return 0;
            }

            return (random.Next(range) * round + min);
        }

        public static List<Tuple<PickerOption.OptionChoices, int>> RandomOptions(ScriptablePickerConfig config) {
            List<Tuple<PickerOption.OptionChoices, int>> optionList = new List<Tuple<PickerOption.OptionChoices, int>>();

            foreach (PickerOption.OptionChoices optionChoice in Enum.GetValues(typeof(PickerOption.OptionChoices))) {
                float prob = 0;
                switch (optionChoice) {
                    case PickerOption.OptionChoices.CoinRewards:
                        prob = config.CoinRewardsProbability;
                        break;
                    case PickerOption.OptionChoices.PickerRewards:
                        prob = config.PickerRewardsProbability;
                        break;
                    case PickerOption.OptionChoices.EndPicker:
                        prob = config.EndPickerProbability;
                        break;
                }

                int numOfChoice = Mathf.RoundToInt(config.NumOfOptions * prob);

                for (int i = 0; i < numOfChoice; i++) {
                    int optionValue = -1;
                    switch (optionChoice) {
                        case PickerOption.OptionChoices.CoinRewards:
                            optionValue = RandomInt(config.MinCoinValue, config.MaxCoinValue + config.RoundCoinAtValue, config.RoundCoinAtValue);
                            break;
                        case PickerOption.OptionChoices.PickerRewards:
                            optionValue = RandomInt(config.MinPickerValue, config.MaxPickerValue + 1);
                            break;
                    }

                    optionList.Add(Tuple.Create(optionChoice, optionValue));
                }
            }

            for (int i = 0; i < config.NumOfOptions - optionList.Count; i++) {
                optionList.Add(Tuple.Create(PickerOption.OptionChoices.CoinRewards, RandomInt(config.MinCoinValue, config.MaxCoinValue + config.RoundCoinAtValue, config.RoundCoinAtValue)));
            }

            return optionList;
        }

        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public static class LocalStorage {
        public static void SaveUser(ModelPlayer user) {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/UserRecord.LocalSaving";
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, user);
            stream.Close();
        }

        public static void LoadUserInfo(out ModelPlayer user) {
            string path = Application.persistentDataPath + "/UserRecord.LocalSaving";
            if (File.Exists(path)) {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                ModelPlayer saved = (ModelPlayer)formatter.Deserialize(stream);
                stream.Close();
                user = saved;
            } else {
                user = new ModelPlayer();
            }
        }
    }
}
