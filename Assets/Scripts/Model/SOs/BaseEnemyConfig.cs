using UnityEngine;
using View;

namespace Model {

    [CreateAssetMenu(menuName = "Config/Enemy/Enemy")]
    public class BaseEnemyConfig : ScriptableObject {

        public BaseEnemyView Prefab;

        public int Score;
        public int BaseHP;
    }
}