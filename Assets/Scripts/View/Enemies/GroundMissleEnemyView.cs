using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {
    
    public class GroundMissleEnemyView : BaseEnemyView, ITracking, ITimeLimited {

        [SerializeField] private Collider2D[] _colliders;
        [SerializeField] private GameObject _targetMarker;
        [SerializeField] private Transform _missile;
        [SerializeField] private GameObject _missileFX;
        [SerializeField] private Vector3 _misslieStartPoint;

        [SerializeField] private float _impactDelay;
        [SerializeField] private float _lockSpeed;
        [SerializeField] private float _lockStartDist;


        private enum MissileStates { Locking, Impact, Cooldown};

        private Transform _target;
        private SpriteRenderer _targetMarkerSprite;
        private MissileStates _missileState;

        private float _impactTime;
        private bool _fixedUpdateDone;
        

        override internal void Init() {
            base.Init();
            _targetMarkerSprite = _targetMarker.GetComponent<SpriteRenderer>();
        }

        override internal void ResetView() {
            base.ResetView();
            _missileState = MissileStates.Locking;
            foreach(var c in _colliders) {
                c.enabled = false;
            }
            _targetMarkerSprite.enabled = false;
            _missile.position = _misslieStartPoint;

            _fixedUpdateDone = false;

            _missileFX.SetActive(false);
        }

        override internal void SetActive(bool isActive) {
            base.SetActive(isActive);
            if (isActive) {
                _impactTime = Time.time + _impactDelay;


                Vector2 offset = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)) * Vector2.one * _lockStartDist;

                _targetMarker.transform.position = _target.position + (Vector3)offset;
                _targetMarkerSprite.enabled = true;
                _missile.gameObject.SetActive(true);
            }

        }

        public void SetTarget(Transform target) {
            _target = target;
        }

        void FixedUpdate() {
            if (_missileState == MissileStates.Impact) {
                _fixedUpdateDone = true;
            }
        }

        void Update() {
            if (!_isActive) return;

            switch (_missileState) {
                case MissileStates.Locking:
                    if (Time.time >= _impactTime) {
                        _missileState = MissileStates.Impact;
                        foreach (var c in _colliders) {
                            c.enabled = true;
                        }
                        _targetMarkerSprite.enabled = false;
                        _missile.gameObject.SetActive(false);
                        _missileFX.transform.localPosition = _targetMarkerSprite.transform.localPosition;
                        _missileFX.SetActive(true);
                    } else {
                        if (_impactTime - Time.time > 0.5f) {
                            _targetMarkerSprite.color = Color.green;
                        } else {
                            _targetMarkerSprite.color = Color.red;
                        }
                    }
                    float rat = (Time.time - (_impactTime - _impactDelay)) / _impactDelay;
                    _missile.position = Vector3.Lerp(_misslieStartPoint, _targetMarker.transform.position, rat);

                    Vector3 targetShift = (_target.position - _targetMarker.transform.position);

                    float movDist = _lockSpeed * Time.deltaTime;
                    if (targetShift.magnitude > movDist) {
                        targetShift.Normalize();
                        targetShift *= movDist;
                    }

                    _targetMarker.transform.position += targetShift;


                    break;
                case MissileStates.Impact:
                    if (_fixedUpdateDone) {
                        foreach (var c in _colliders) {
                            c.enabled = false;
                        }
                        _missileState = MissileStates.Cooldown;
                    }
                    
                    break;
                case MissileStates.Cooldown:
                    break;
            }
            

            
        }

        internal override Transform GetTransform() {
            return null;
        }
    }
}