using Controller;
using Model;
using System;
using UnityEngine;

namespace View {

    [RequireComponent(typeof(Animator))]
    public class GameplayView : MonoBehaviour {

        [SerializeField] PlayerCont _player;
        [SerializeField] Camera _camera;

        private SettingsConfig _settings;
        private MainStateModel _mainStateModel;
        private Animator _animator;

        public Action OnGameOver;
        public Action<BaseWeapon, GameObject, ParticleCollisionEvent> OnHit;

        public void Init() {

            _animator = GetComponent<Animator>();

            _settings = ModelInitiator.GetSettingsConfig();
            _mainStateModel = ModelInitiator.GetMainStateModel();

            _player.Init(_settings);

            _player.OnColision += ShipCollision;

        }

        public void AddWeapon(BaseWeapon weapon) => _player.AddWeapon(weapon).OnHit += OnAnyHit;

        private void OnAnyHit(BaseWeapon weapon, GameObject target, ParticleCollisionEvent collision) {
            OnHit(weapon, target, collision);
        }

        internal void ResetView() {
            _player.transform.localPosition = _settings.PlayerStartPos;
            _player.ResetView();
            _animator.SetInteger("State", 0);
        }

        internal void StartGame() {
            _player.StartGame();
            _animator.SetInteger("State", 1);
        }

        internal void Tick(float deltaTime) {
            if (_mainStateModel.FlowState == MainStateModel.FState.Gameplay) {
                Vector3 transPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_camera.transform.position.z);

                _player.transform.position = _camera.ScreenToWorldPoint(transPos);
                _player.Tick(deltaTime);
            } else {
                _player.transform.position = _settings.PlayerStartPos;
            }
            
        }

        private void ShipCollision(Collider2D col) {
            _player.GameOver();
            OnGameOver();
        }

      

    }
}
