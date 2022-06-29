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
        public Vector2 PlayerStartPos;
        

        [Header("Gameplay")]
        public float WorldSpeedGame;
        public float GameAreaWidth;
        public BaseWeapon StartingWeapon;


        [Header("Look And Feel")]
        public float SpawnDistance;
        public float EnemyDestructionDist;
        public float NegativeDistOnStart;


        [Header("Level")]
        public LevelConfig[] Levels;

        

        /*void OnValidate() {
            
        }*/

    }
}