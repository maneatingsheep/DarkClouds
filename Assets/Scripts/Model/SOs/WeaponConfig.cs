using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {

    [CreateAssetMenu(menuName = "Config/Weapons/WeaponConfig")]
    public class WeaponConfig : ScriptableObject {
        public int DamagePerHit;
    }
}