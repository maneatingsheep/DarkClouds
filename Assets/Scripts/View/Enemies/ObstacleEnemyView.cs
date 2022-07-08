using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {
    public class ObstacleEnemyView : BaseEnemyView, IStationary {
        internal override Transform GetTransform() {
            return transform;
        }
    }
}