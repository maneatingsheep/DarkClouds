using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    
    [CreateAssetMenu(menuName = "Config/Enemy/Wave")]
    public class WaveConfig : ScriptableObject {

        public BaseEnemyConfig Enemy;

        public enum MovementType { Static, PathInOut, PathInLoop }
        public enum GroupType { Single, DelayedGroup }


        //types
        public MovementType MovType;
        public GroupType GrpType;

        //group params
        public int GroupSize;
        public float GroupSpawnDelay;

        //path params
        public PathConfig Path;

        //static params
        public float StaticHorizPos;
    }

    [Serializable]
    public struct PathConfig {

        public MidPointConfig[] PathDots;
        public float[] PathDistances;
        public float PathTime;


    }

    [Serializable]
    public struct MidPointConfig {

        public Vector2 PointPos;
        public Vector2 InHandlePos;
        public Vector2 OutHandlePos;
        


    }
}