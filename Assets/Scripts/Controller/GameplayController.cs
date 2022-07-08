using Model;
using System;
using UnityEngine;
using View;

namespace Controller {
    public class GameplayController {
        
        private MainStateModel _mainStateModel;
        private PlayerView _playerView;
        private SettingsConfig _settings;
        private Ticker _ticker;
        private GameElementController _gameElementController;

        internal void Init() {
            
            //assign local references
            //model
            _mainStateModel = ModelInitiator.GetMainStateModel();
            _settings = ModelInitiator.GetSettingsConfig();
            _ticker = ModelInitiator.Getticker();

            _gameElementController = new GameElementController();

            _playerView = ViewInitiator.GetPlayerView();

            //init controllers
            _gameElementController.Init();

            _gameElementController.OnAllEnemiesDespawned += OnAllEnemiesDespawned;


            _playerView.Init();
            var im = ViewInitiator.GetInputManager();
            im.Init();

            //add listeners
            _ticker.Ontick = Tick;
            _playerView.OnWeaponHit = OnEnemyHit;

            _playerView.AddWeapon(_settings.StartingWeapon);
        }

        internal void UpdateState() {
            switch (_mainStateModel.FlowState) {
                case MainStateModel.FState.GameOver: break;
                case MainStateModel.FState.PressToPlay:
                    _gameElementController.Reset();
                    _playerView.ResetView();
                    break;
                case MainStateModel.FState.Gameplay:
                    _mainStateModel.ShipProgress = _settings.NegativeDistOnStart;
                    _playerView.StartGame();
                    _gameElementController.StartGame();
                    break;
            }
        }

        private void Tick(float deltaTime) {
            float distProg = deltaTime * _settings.WorldSpeedGame;

            _mainStateModel.ShipProgress += distProg;

            _gameElementController.Tick(distProg, deltaTime);

            _playerView.Tick(deltaTime);

        }

        private void OnEnemyHit(BaseWeapon weapon, GameObject target, ParticleCollisionEvent collision) {

            int damage = weapon.Config.DamagePerHit;
            _gameElementController.OnEnemyHit(target, damage);
        }

        private void OnAllEnemiesDespawned() {
            OnGameOver();
        }


        public Action OnGameOver {
            //expose the event to higher controller
            get { return _playerView.OnGameOver; }
            set { _playerView.OnGameOver = value; }
        }

    }
}
