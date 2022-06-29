using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;

namespace Controller {
    public class MainFlowController : MonoBehaviour {

        
        //initiators
        [SerializeField] private BaseIntroducer _modelInitiator;
        [SerializeField] private BaseIntroducer _viewInitiator;


        //locals
        private GameplayController _gameplayController;
        private UIController _uiController;
        private MainStateModel _mainStateModel;

        void Start() {

            if(_modelInitiator == null) {
                Debug.LogError("_modelInitiator is missing");
            }
            if (_viewInitiator == null) {
                Debug.LogError("_viewInitiator is missing");
            }

            try {
                //InitAll will fail on most missing refrences in the editor
                InitAll();
            }catch (Exception e) {
                Debug.LogError("Error during InitAll(): " + e.ToString());
            }

            SetState(MainStateModel.FState.PressToPlay);
        }

        private void InitAll() {

            //model introducer
            _modelInitiator.CheckDependencies();
            (_modelInitiator as ModelInitiator).Init(new MainStateModel(), new PersistentDataModel());
            
            //view introducer
            _viewInitiator.CheckDependencies();

            //allow introduction
            _modelInitiator.SetIntroductionMode(true);
            _viewInitiator.SetIntroductionMode(true);

            //assign local
            _gameplayController = new GameplayController();
            _uiController = new UIController();
            
            _mainStateModel = ModelInitiator.GetMainStateModel();

            //init
            ModelInitiator.GetSettingsConfig().Init();
            _gameplayController.Init();
            _uiController.Init();


            //add listeners
            var im = ViewInitiator.GetInputManager();
            im.OnAnyButton += StartPressed;
            _gameplayController.OnGameOver += GameOver;

            //introduction done
            _modelInitiator.SetIntroductionMode(false);
            _viewInitiator.SetIntroductionMode(false);
        }


        private void SetState(MainStateModel.FState state) {
            _mainStateModel.FlowState = state;
            _uiController.UpdateState();
            _gameplayController.UpdateState();
        }

        private void StartPressed() {
            if (_mainStateModel.FlowState == MainStateModel.FState.Gameplay) return;
            SetState(MainStateModel.FState.Gameplay);
        }

        private void GameOver() {
            SetState(MainStateModel.FState.GameOver);
        }
    }
}

