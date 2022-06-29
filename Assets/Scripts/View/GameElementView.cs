using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {

    public class GameElementView : MonoBehaviour {

        [SerializeField] private GameObject StartPrefab;


        private SettingsConfig _settings;
        private BezierCalculator _bezierCalc;
        private Dictionary<BaseEnemyConfig, LinearPool<BaseEnemyView>> _pools = new Dictionary<BaseEnemyConfig, LinearPool<BaseEnemyView>>();

        

        public void Init() {
            _settings = ModelInitiator.GetSettingsConfig();
            _bezierCalc = new BezierCalculator();
        }

        
        //position placement
        public BaseEnemyView SpawnEnemy(BaseEnemyConfig enemyConfig, Vector2 spawnPoint) {

            if (!_pools.ContainsKey(enemyConfig)){
                _pools.Add(enemyConfig, new LinearPool<BaseEnemyView>());
            }

            BaseEnemyView obstacle = _pools[enemyConfig].GetFromPool();
            if (obstacle == null) {
                obstacle = Instantiate(enemyConfig.Prefab, transform);
                obstacle.Config = enemyConfig;
            }

            obstacle.transform.localPosition = spawnPoint;

            return obstacle;

        }

        internal GameObject SpawnStartingLine(float distFromOrigin) {
            var start = Instantiate(StartPrefab, transform);

            start.transform.localPosition = new Vector3(0, distFromOrigin, 0);
            return start;
        }

        internal void DeSpawnEnemy(BaseEnemyView obs) {
            obs.SetActive(false);
            _pools[obs.Config].GiveToPool(obs);

        }

        internal bool ProgressEnemy(BaseEnemyModel obj, float distProg) {

            if (obj.Wave.MovType == WaveConfig.MovementType.Static) {
                obj.View.transform.Translate(new Vector3(0, -distProg, 0), Space.World);
                return (obj.View.transform.position.y < _settings.EnemyDestructionDist);
            } else {
                var t = (Time.time - obj.PathStartTime) / (obj.Wave.Path.PathTime);
                obj.View.transform.position = CalculatePathPosition(obj.Wave.Path, t);

                return Time.time > obj.PathEndTime;
            }
        }

        private Vector2 CalculatePathPosition(PathConfig path, float prog) {
            float d = 0;
            for (int i = 0; i < path.PathDistances.Length; i++) {
                if (d + path.PathDistances[i] > prog) {
                    float relProg = (prog - d) / path.PathDistances[i];
                    return _bezierCalc.CalculatePosition(path.PathDots[i], path.PathDots[i + 1], relProg);

                } else {
                    d += path.PathDistances[i];
                }
            }

            return Vector2.zero;
        }

        internal void ResetView(List<BaseEnemyModel> obs) {

            foreach(var o in obs) {
                o.View.gameObject.SetActive(false);
                _pools[o.Config].GiveToPool(o.View);
            }
        }
    }
}
