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

        private bool _isTouch;
        private Vector2 _startDragFingerPos;
        private Vector2 _startDragPlayerPos;

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
            _isTouch = false;
        }

        internal void StartGame() {
            _player.StartGame();
            _animator.SetInteger("State", 1);
        }

        internal void Tick(float deltaTime) {
            if (_mainStateModel.FlowState == MainStateModel.FState.Gameplay) {

                if (Input.GetMouseButton(0)) {
                    Vector2 transPos = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_camera.transform.localPosition.z));
                    if (!_isTouch) {
                        _startDragFingerPos = transPos;
                        _startDragPlayerPos = _player.transform.localPosition;
                    } else {
                        _player.transform.localPosition = _startDragPlayerPos + (transPos - _startDragFingerPos);
                    }
                    _isTouch = true;
                } else {
                    _isTouch = false;
                }

                _player.Tick(deltaTime);
            } else {
                _player.transform.localPosition = _settings.PlayerStartPos;
            }
        }

        private void ShipCollision(Collider2D col) {
            _player.GameOver();
            OnGameOver();
        }



    }
}
