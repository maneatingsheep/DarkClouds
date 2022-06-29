using Controller;
using Model;
using System;
using UnityEngine;

namespace View {

    [RequireComponent(typeof(Animator))]
    public class GameplayView : MonoBehaviour {

        [SerializeField] private SpaceShip _spaceShip;
        
        private SettingsConfig _settings;
        private MainStateModel _mainStateModel;
        private Animator _animator;

        public Action OnGameOver;
        public Action<BaseWeapon, GameObject, ParticleCollisionEvent> OnHit;

        public void Init() {

            _animator = GetComponent<Animator>();

            _settings = ModelInitiator.GetSettingsConfig();
            _mainStateModel = ModelInitiator.GetMainStateModel();

            _spaceShip.Init(_settings);
            
            _spaceShip.OnWallTouch += WallTouch;
            _spaceShip.OnColision += ShipCollision;

        }

        public void AddWeapon(BaseWeapon weapon) {
            _spaceShip.AddWeapon(weapon).OnHit += OnAnyHit;
        }

        private void OnAnyHit(BaseWeapon weapon, GameObject target, ParticleCollisionEvent collision) {
            OnHit(weapon, target, collision);
        }

        internal void ResetView() {
            _spaceShip.ResetView();
            _animator.SetInteger("State", 0);
        }

        internal void StartGame() {
            _spaceShip.StartGame();
            _animator.SetInteger("State", 1);
        }

        internal void Tick(float deltaTime) {
            _spaceShip.Tick(deltaTime);
        }

        private void ShipCollision(Collider2D col) {
            _spaceShip.GameOver();
            OnGameOver();
        }

        internal void Steer(InputManager.SteerDir dir) {
            if (dir == InputManager.SteerDir.None) {
                _spaceShip.StopSteer();
            } else {
                _spaceShip.Steer(((dir == InputManager.SteerDir.Left)?-1:1));
            }
        }

        internal void HardSteer() {
            if (_mainStateModel.ShipProgress < 0) return;
            _spaceShip.HardSteer();
        }

        private void WallTouch(bool obj) {
            _spaceShip.StopSteer();
        }

    }
}
