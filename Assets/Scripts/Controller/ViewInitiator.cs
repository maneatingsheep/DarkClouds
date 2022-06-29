using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;

namespace Model {

    public class ViewInitiator : BaseIntroducer {
        [SerializeField] private UIView _uIView;
        [SerializeField] private GameplayView _gameplayView;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private GameElementView _obstacleView;

        //static ref
        public static ViewInitiator Instance;

        void Awake() {
            Instance = this;
        }

        override internal void CheckDependencies() {
            bool allHere = true;
            allHere &= _uIView != null;
            allHere &= _gameplayView != null;
            allHere &= _inputManager != null;


            if (!allHere) {
                throw new Exception("missing view refs");
            }
        }

        public static UIView GetUIView() {
            Instance.CheckAccess();
            return Instance._uIView;
        }

        public static GameplayView GetGameplayView() {
            Instance.CheckAccess();
            return Instance._gameplayView;
        }

        public static InputManager GetInputManager() {
            Instance.CheckAccess();
            return Instance._inputManager;
        }

        public static GameElementView GetObstacleView() {
            Instance.CheckAccess();
            return Instance._obstacleView;
        }
        
        
    }
}