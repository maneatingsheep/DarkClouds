using Controller;
using System;
using UnityEngine;

namespace Model {
    [CreateAssetMenu(menuName = "Config/Gameplay/SettingsConfig")]
    public class SettingsConfig : ScriptableObject {
        public void Init() {
            HalfGameAreaWidth = GameAreaWidth / 2f;
        }


        [HideInInspector]
        public float HalfGameAreaWidth;

        [Header("Movement")]
        public float NormalAccelStr;
        public float StrongAccelStr;
        public float MaxHorizSpeed;
        public float HardSteerMaxHorizSpeed;

        [Header("Gameplay")]
        public float WorldSpeedGame;
        public float GameAreaWidth;
        public float GameAreaHeight;
        public BaseWeapon StartingWeapon;


        [Header("Look And Feel")]
        public float SpawnDistance;
        public float EnemyDestructionDist;
        public float NegativeDistOnStart;

        [Header("Control")]
        public KeyCode Right;
        public KeyCode Left;
        public KeyCode SecondaryRight;
        public KeyCode SecondaryLeft;
        public bool AllowHardSteer;
        public float LongPressTime;

        [Header("Level")]
        public LevelConfig[] Levels;
        

        void OnValidate() {
            GameObject sizeRef = GameObject.Find("BGSizeRef");
            if (sizeRef) {
                sizeRef.transform.localScale = new Vector2(GameAreaWidth, GameAreaHeight);
                sizeRef.transform.localPosition = new Vector2(0, GameAreaHeight / 2f);
            }
        }

    }
}