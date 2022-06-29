using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Model {
    public abstract class BaseIntroducer : MonoBehaviour {

        private bool _allowstaticAccess;


        internal abstract void CheckDependencies();

        internal void SetIntroductionMode(bool val) {
            _allowstaticAccess = val;
        }


        internal void CheckAccess() {
            if (!_allowstaticAccess) {
                throw new Exception("can't introduce model");
            }
        }
    }
}