using Model;
using System;
using UnityEngine;
using View;

namespace Controller {
    public class GameplayController {
        
        private MainStateModel _mainStateModel;
        private GameplayView _gameplayView;
        private SettingsConfig _settings;
        private Ticker _ticker;
        private GameElementController _obstacleController;

        internal void Init() {
            
            //assign local references
            //model
            _mainStateModel = ModelInitiator.GetMainStateModel();
            _settings = ModelInitiator.GetSettingsConfig();
            _ticker = ModelInitiator.Getticker();

            _obstacleController = new GameElementController();

            _gameplayView = ViewInitiator.GetGameplayView();

            //init controllers
            _obstacleController.Init();
            _gameplayView.Init();
            var im = ViewInitiator.GetInputManager();
            im.Init();

            //add listeners
            _ticker.Ontick = Tick;
            _gameplayView.OnHit = OnObstacleHit;

            _gameplayView.AddWeapon(_settings.StartingWeapon);
        }

        internal void UpdateState() {
            switch (_mainStateModel.FlowState) {
                case MainStateModel.FState.GameOver: break;
                case MainStateModel.FState.PressToPlay:
                    _obstacleController.Reset();
                    _gameplayView.ResetView();
                    break;
                case MainStateModel.FState.Gameplay:
                    _mainStateModel.ShipProgress = _settings.NegativeDistOnStart;
                    _gameplayView.StartGame();
                    _obstacleController.StartGame();
                    break;
            }
        }

        private void Tick(float deltaTime) {
            float distProg = deltaTime * _settings.WorldSpeedGame;

            _mainStateModel.ShipProgress += distProg;

            _obstacleController.Tick(distProg, deltaTime);

            if (_mainStateModel.FlowState == MainStateModel.FState.Gameplay) {
                _gameplayView.Tick(deltaTime);
            }

        }

        private void OnObstacleHit(BaseWeapon weapon, GameObject target, ParticleCollisionEvent collision) {

            int damage = weapon.Config.DamagePerHit;

            _obstacleController.OnEnemyHit(target, damage);
        }


       


        

        public Action OnGameOver {
            //expose the event to higher controller
            get { return _gameplayView.OnGameOver; }
            set { _gameplayView.OnGameOver = value; }
        }

    }
}
