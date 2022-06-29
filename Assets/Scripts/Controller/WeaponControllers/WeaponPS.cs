using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller {

    public class WeaponPS : MonoBehaviour {

        public Action<GameObject, ParticleCollisionEvent> OnHit;
        private ParticleSystem _particle;

        void Awake() {
            _particle = GetComponent<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject target) {
            List<ParticleCollisionEvent> collisions = new List<ParticleCollisionEvent>();
            ParticlePhysicsExtensions.GetCollisionEvents(_particle, target, collisions);
            for (int i = 0; i < collisions.Count; i++) {
                OnHit(target, collisions[i]);
            }
            
            
        }
    }
}