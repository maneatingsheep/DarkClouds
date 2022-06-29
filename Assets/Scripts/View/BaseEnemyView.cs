using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {
    public class BaseEnemyView : MonoBehaviour {
        internal BaseEnemyConfig Config;
        public BaseEnemyModel Model;

        internal void SetActive(bool isActive) {
            gameObject.SetActive(isActive);
        }
    }
}

