using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    
    [CreateAssetMenu(menuName = "Config/Enemy/Path")]
    public class PathConfig : ScriptableObject {

        public MidPointConfig[] PathDots;
        public float[] PathDistances;
    }


    [Serializable]
    public struct MidPointConfig {

        public Vector2 PointPos;
        public Vector2 InHandlePos;
        public Vector2 OutHandlePos;
        


    }
}