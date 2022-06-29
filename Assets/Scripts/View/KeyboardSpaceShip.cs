#if KEYBOARD_SPACESHIP
using Controller;
using DG.Tweening;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {

    public class KeyboardSpaceShip : MonoBehaviour {

        [SerializeField] private float _steerRollSpeed;
        [SerializeField] private ShipCollider[] _colliders;
        [SerializeField] private GameObject _body;
        [SerializeField] private GameObject _deathFx;


        public Action<bool> OnWallTouch;
        public Action<Collider2D> OnColision;

        private enum SteerState { None, NormalAccel, OppositeAccel, Brake };

        private float _horizontalSpeed;
        private float _roll;
        private SteerState _steerState;
        private float _hardSteerStart;
        private int _steerDirection;

        private SettingsConfig _settings;

        private bool _isHardSteer;

        internal void Init(SettingsConfig settings) {
            _settings = settings;
            
            transform.localScale = Vector3.one;
            foreach(var c in _colliders) {
                c.OnCollision += Colision;
            }

            //make ship collider ignore each other
            for (int i = 0; i < _colliders.Length - 1; i++) {
                for (int j = i + 1; j < _colliders.Length; j++) {
                    Physics2D.IgnoreCollision(_colliders[i].GetComponent<Collider2D>(), _colliders[j].GetComponent<Collider2D>(), true);

                }
            }

        }

        public BaseWeapon AddWeapon(BaseWeapon weapon) {
            return Instantiate(weapon, transform);
        }

        private void Colision(Collider2D other) {
            OnColision(other);
        }

        internal void ResetView() {
            _horizontalSpeed = 0;
            _roll = 0;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            _steerState = SteerState.None;
            _steerDirection = 0;
        }

        internal void StartGame() {
            _body.SetActive(true);
            _deathFx.SetActive(false);
        }

        internal void Steer(int steerDirection) {

            _isHardSteer &= _steerDirection != steerDirection;

            _steerDirection = steerDirection;
            
            if (_horizontalSpeed != 0 && _steerDirection > 0 != _horizontalSpeed > 0) {
                _steerState = SteerState.OppositeAccel;
            } else {
                _steerState = SteerState.NormalAccel;
            }

        }

        internal void GameOver() {
            _body.SetActive(false);
            _deathFx.SetActive(true);
        }

        internal void StopSteer() {
            if (_steerState != SteerState.NormalAccel) return;

            _steerState = SteerState.Brake;
            _steerDirection = (_horizontalSpeed > 0) ? -1 : 1;
            _isHardSteer = false;
        }

        internal void HardSteer() {
            if (_steerState != SteerState.NormalAccel) return;
            _isHardSteer = true;
        }

        internal void Tick(float deltaTime) {

            //sideways acceleration determined by the current state
            float horizontalAccel = 0;

            switch (_steerState) {
                case SteerState.None:
                    break;
                case SteerState.NormalAccel:
                    horizontalAccel = _steerDirection * _settings.NormalAccelStr;
                    break;
                case SteerState.Brake:
                case SteerState.OppositeAccel:
                    horizontalAccel = _steerDirection * _settings.StrongAccelStr;
                    break;
            }

            float prevSpeed = _horizontalSpeed;
            _horizontalSpeed += horizontalAccel * deltaTime;

            //detect zero or sign flip. speed crossed zero value
            bool speedSignflip = (prevSpeed != 0) && (_horizontalSpeed == 0 || ((_horizontalSpeed > 0) != (prevSpeed > 0)));
                
            if (speedSignflip) {
                if (_steerState == SteerState.OppositeAccel) {
                    //stop hard accel(breaking)
                    _steerState = SteerState.NormalAccel;
                } else if (_steerState == SteerState.Brake) {
                    //slow down complete
                    _steerState = SteerState.None;
                    _horizontalSpeed = 0;
                }
            }

            if (_isHardSteer) {
                _roll += Time.deltaTime * -800 * _steerDirection;

                //without this conversion, the ship will counterrotate
                if (_roll > 180) _roll = -360 + _roll;
                if (_roll < -180) _roll = 360 + _roll;

            } else {
                //roll reflects acceleration and smoothly follows
                float targetRoll = -horizontalAccel;
                _roll = Mathf.Lerp(_roll, targetRoll, _steerRollSpeed * Time.deltaTime);
            }
            

            //limit max horiz speed
            float speedLimit = (_isHardSteer) ? _settings.HardSteerMaxHorizSpeed : _settings.MaxHorizSpeed;
            _horizontalSpeed = Mathf.Clamp(_horizontalSpeed, -speedLimit, speedLimit);

            transform.rotation = Quaternion.Euler(0, _roll, 0);

            //normall horizontal progression
            float horPos = transform.position.x + _horizontalSpeed * Time.deltaTime;

            //thi limit for the ship (and lane obstacles) is half lane less then road width
            float avalibleSpace = _settings.HalfGameAreaWidth - 1f;
            horPos = Mathf.Clamp(horPos, -avalibleSpace, avalibleSpace);

            transform.position = new Vector3(horPos, 0, 0);

        }

    }
}
#endif