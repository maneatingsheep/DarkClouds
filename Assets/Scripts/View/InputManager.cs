using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {
    public class InputManager : MonoBehaviour {
        
        public Action OnAnyButton;


        private SettingsConfig _settings;
        private MainStateModel _mainStateModel;

        public void Init() {
            _settings = ModelInitiator.GetSettingsConfig();
            _mainStateModel = ModelInitiator.GetMainStateModel();
        }


        void Update() {

            if (_mainStateModel.FlowState == MainStateModel.FState.Gameplay) {

                

            } else {
                if (Input.anyKeyDown) {
                    OnAnyButton();
                }
            }
            
        }

        
    }
}