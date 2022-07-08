using Model;
using System;
using UnityEngine;

namespace Controller {

    public class BaseWeapon : MonoBehaviour {
        [SerializeField] public WeaponConfig Config;
        [SerializeField] private WeaponPS[] _particles;

        public Action<BaseWeapon, GameObject, ParticleCollisionEvent> OnWeaponHit;

        void Awake() {
            for (int i = 0; i < _particles.Length; i++) {
                _particles[i].OnHit += OnPSHit;
            }
        }

        private void OnPSHit(GameObject target, ParticleCollisionEvent collision) {
            OnWeaponHit(this, target, collision);
        }
    }
}