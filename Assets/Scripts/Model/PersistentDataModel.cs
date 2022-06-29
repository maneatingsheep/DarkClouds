using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    public class PersistentDataModel {
        readonly public string HIGH_SCORE_KEY = "HighScoreKey";

        public void SaveDataInt(string key, int val) {
            try {
                PlayerPrefs.SetInt(key, val);
            } catch (Exception e) {
                Debug.LogError("Exception during write of key " + key + ": " + e.Message);
            }
        }

        public int GetDataInt(string key, int def) {

            int res = def;
            try {
                if (PlayerPrefs.HasKey(key)) {
                    res = PlayerPrefs.GetInt(key);
                }
            } catch (Exception e) {
                Debug.LogError("Exception during read of key " + key + ": " + e.Message);
            }

            return res;
        }
    }
}