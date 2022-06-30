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

        public enum MovementType { Static, PathInOut, PathInLoop }
        public enum GroupType { Single, DelayedGroup }


        //types
        public MovementType MovType;
        public GroupType GrpType;

        //group params
        public int GroupSize;
        public float GroupSpawnDelay;
        public float StaticHorizPos;

        public float PathTime;

    }
}