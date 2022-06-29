using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    [CreateAssetMenu(menuName = "Config/Level/Levelconfig")]

    public class LevelConfig : ScriptableObject {
        public WaveTiming[] Waves;
    }

    [Serializable]
    public struct WaveTiming {
        public WaveConfig Wave;
        public float GapToPrevious;
    }
}