using DG.Tweening;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {
    public class InputManager : MonoBehaviour {
        public enum SteerDir {None, Left,Right };
        public Action<SteerDir> OnSteer;
        public Action OnLongPress;
        public Action OnAnyButton;


        private SteerDir _direction;
        private SettingsConfig _settings;
        private MainStateModel _mainStateModel;
        private Tween _longPressTween;

        public void Init() {
            _settings = ModelInitiator.GetSettingsConfig();
            _mainStateModel = ModelInitiator.GetMainStateModel();
        }


        void Update() {

            if (_mainStateModel.FlowState == MainStateModel.FState.Gameplay) {

                bool rightButtonDown = Input.GetKeyDown(_settings.Right) || Input.GetKeyDown(_settings.SecondaryRight);
                bool leftButtonDown = Input.GetKeyDown(_settings.Left) || Input.GetKeyDown(_settings.SecondaryLeft);
                bool rightButtonUp = Input.GetKeyUp(_settings.Right) || Input.GetKeyUp(_settings.SecondaryRight);
                bool leftButtonUp = Input.GetKeyUp(_settings.Left) || Input.GetKeyUp(_settings.SecondaryLeft);

                if (rightButtonDown) {
                    _direction = SteerDir.Right;
                    OnSteer(_direction);
                    StartLongPressTimer();
                }

                if (leftButtonDown) {
                    _direction = SteerDir.Left;
                    OnSteer(_direction);
                    StartLongPressTimer();
                }

                if (_direction == SteerDir.Right) {
                    if (rightButtonUp) {
                        _direction = SteerDir.None;
                        OnSteer(_direction);
                        StopLongPressTimer();
                    }
                } else {
                    if (leftButtonUp) {
                        _direction = SteerDir.None;
                        OnSteer(_direction);
                        StopLongPressTimer();
                    }
                }

            } else {
                if (Input.anyKeyDown) {
                    OnAnyButton();
                }
            }
            
        }

        private void StopLongPressTimer() {
            if (_longPressTween != null) {
                _longPressTween.Kill();
            }
        }

        private void StartLongPressTimer() {
            StopLongPressTimer();
            _longPressTween =  DOVirtual.DelayedCall(_settings.LongPressTime, LongPressTimerComplete);
        }

        private void LongPressTimerComplete() {
            OnLongPress();
        }
    }
}