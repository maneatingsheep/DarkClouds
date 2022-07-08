using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {
    abstract public class BaseEnemyView : MonoBehaviour {
        
        internal BaseEnemyConfig Config;
        public BaseEnemyModel Model;

        protected bool _isActive;

        virtual internal void Init() {

        }

        virtual internal void ResetView() {

        }

        virtual internal void SetActive(bool isActive) {
            gameObject.SetActive(isActive);
            _isActive = isActive;
        }

        abstract internal Transform GetTransform();
    }
}

