using Controller;
using Model;
using System;
using UnityEngine;

namespace View {

    [RequireComponent(typeof(Animator))]
    public class PlayerView : MonoBehaviour {

        [SerializeField] ShipView _ship;
        [SerializeField] Camera _camera;

        private SettingsConfig _settings;
        private MainStateModel _mainStateModel;
        private Animator _animator;

        public Action OnGameOver;
        public Action<BaseWeapon, GameObject, ParticleCollisionEvent> OnWeaponHit;

        private bool _isTouch;
        private Vector2 _startDragFingerPos;
        private Vector2 _startDragPlayerPos;

        public void Init() {

            _animator = GetComponent<Animator>();

            _settings = ModelInitiator.GetSettingsConfig();
            _mainStateModel = ModelInitiator.GetMainStateModel();

            _ship.Init(_settings);

            _ship.OnDamage += ShipDamage;

        }

        public ShipView GetShip() {
            return _ship;
        }

        public void AddWeapon(BaseWeapon weapon) => _ship.AddWeapon(weapon).OnWeaponHit += OnAnyWeaponHit;

        private void OnAnyWeaponHit(BaseWeapon weapon, GameObject target, ParticleCollisionEvent collision) {
            OnWeaponHit(weapon, target, collision);
        }

        internal void ResetView() {
            _ship.transform.localPosition = _settings.PlayerStartPos;
            _ship.ResetView();
            _animator.SetInteger("State", 0);
            _isTouch = false;
        }

        internal void StartGame() {
            _ship.StartGame();
            _animator.SetInteger("State", 1);
        }

        internal void Tick(float deltaTime) {
            if (_mainStateModel.FlowState == MainStateModel.FState.Gameplay) {

                if (Input.GetMouseButton(0)) {


                    Vector3 worldPos = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_camera.transform.localPosition.z));

                    //transformation for tilted cam

                    Vector2 planePos = Vector2.zero;


                    //orthographic
                    if (_camera.orthographic) {
                        Vector3 camVec = _camera.transform.localRotation * Vector3.forward;
                        float slopey = (camVec.y) / (camVec.z);

                        //line formula. assuming the game plane is at 0
                        float intersectiony = worldPos.y - slopey * worldPos.z;
                        planePos = new Vector2(worldPos.x, intersectiony);
                    } else {
                        //perspective
                        float slopey = (worldPos.y - _camera.transform.position.y) / (worldPos.z - _camera.transform.position.z);
                        float slopex = (worldPos.x - _camera.transform.position.x) / (worldPos.z - _camera.transform.position.z);
                        
                        //line formula. assuming the game plane is at 0
                        float intersectiony = worldPos.y - slopey * worldPos.z;
                        float intersectionx = worldPos.x - slopex * worldPos.z;
                        planePos = new Vector2(intersectionx, intersectiony);
                    }


                    


                    if (!_isTouch) {
                        _startDragFingerPos = planePos;
                        _startDragPlayerPos = _ship.transform.localPosition;
                    } else {
                        _ship.transform.localPosition = _startDragPlayerPos + (planePos - _startDragFingerPos);
                    }
                    _isTouch = true;
                } else {
                    _isTouch = false;
                }

                _ship.Tick(deltaTime);
            } else {
                _ship.transform.localPosition = _settings.PlayerStartPos;
            }
        }

        private void ShipDamage(Collider2D col) {
            _ship.GameOver();
            OnGameOver();
        }



    }
}
