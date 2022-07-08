using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {
    public class ShipEnemyView : BaseEnemyView, IPathFollow {
        internal override Transform GetTransform() {
            return transform;
        }
    }
}