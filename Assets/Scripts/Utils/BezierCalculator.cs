
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

        public (Vector2 pos, float rot) CalculatePathPositionAndRotation(PathConfig path, float prog) {
            Vector2 pos = CalculatePathPosition(path, prog);

            Vector2 lookAtPos = CalculatePathPosition(path, prog + 0.01f);
            float rot = Mathf.Atan2(pos.y - lookAtPos.y, pos.x - lookAtPos.x);
            return (pos, rot);
        }

        public Vector2 CalculatePathPosition(PathConfig path, float prog) {
            float d = 0;
            for (int i = 0; i < path.PathDistances.Length; i++) {
                if (d + path.PathDistances[i] > prog) {
                    float relProg = (prog - d) / path.PathDistances[i];
                    return CalculatePosition(path.PathDots[i], path.PathDots[i + 1], relProg);

                } else {
                    d += path.PathDistances[i];
                }
            }

            return Vector2.zero;
        }

        


    }
}