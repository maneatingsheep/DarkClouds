using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    [CreateAssetMenu(menuName = "Config/Level/Levelconfig")]

    public class LevelConfig : ScriptableObject {
        public WaveConfig[] Waves;
    }

    [Serializable]
    public class WaveConfig {
        public BaseEnemyConfig Enemy;
        public PathConfig Path;
        public float GapToPrevious;

        public enum GroupType { Single, DelayedGroup }

        public GroupType GrpType;

        //group params
        public int GroupSize;
        public float GroupSpawnDelay;

        //static params
        public float StaticHorizPos;

        public float LifeTime;

    }
}