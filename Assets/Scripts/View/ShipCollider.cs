using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {
    public class ShipCollider : MonoBehaviour {

        public Action<Collider2D> OnCollision;

        private void OnTriggerEnter2D(Collider2D other) {
            OnCollision(other);
        }

       

    }
}