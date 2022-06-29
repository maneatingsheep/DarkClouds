
using Model;
using UnityEngine;

namespace View {

    public class BezierCalculator {

        public Vector2 CalculatePosition(MidPointConfig startPos, MidPointConfig endPos, float prog) {
            return Mathf.Pow(1 - prog, 3) * startPos.PointPos +
                3 * Mathf.Pow(1 - prog, 2) * prog * startPos.OutHandlePos +
                3 * (1 - prog) * Mathf.Pow(prog, 2) * endPos.InHandlePos +
                Mathf.Pow(prog, 3) * endPos.PointPos;

        }

    }
}