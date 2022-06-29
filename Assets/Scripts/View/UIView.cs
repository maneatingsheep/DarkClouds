using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View {

    [RequireComponent(typeof(Animator))]
    public class UIView : MonoBehaviour {

        [SerializeField] private Text _DebugText;

        private MainStateModel _mainStateModel;
        private SettingsConfig _settings;

        private Animator _animator;

        internal void Init() {
            _mainStateModel = ModelInitiator.GetMainStateModel();
            _settings = ModelInitiator.GetSettingsConfig();
            _animator = GetComponent<Animator>();
        }

        internal void UpdateState() {
            switch (_mainStateModel.FlowState) {
                case MainStateModel.FState.PressToPlay:
                    _animator.SetInteger("State", 0);
                    break;
                case MainStateModel.FState.Gameplay:
                    _animator.SetInteger("State", 1);
                    break;
                case MainStateModel.FState.GameOver:
                    _animator.SetInteger("State", 2);
                    break;
            }

        }

        void Update() {
            if (_mainStateModel.FlowState == MainStateModel.FState.Gameplay) {
                //_DebugText.text = "";
            }

        }
    }
}
