using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View {

    public class GameElementView : MonoBehaviour {

        private SettingsConfig _settings;
        private BezierCalculator _bezierCalc;
        private Dictionary<BaseEnemyConfig, LinearPool<BaseEnemyView>> _pools = new Dictionary<BaseEnemyConfig, LinearPool<BaseEnemyView>>();



        public void Init() {
            _settings = ModelInitiator.GetSettingsConfig();
            _bezierCalc = new BezierCalculator();
        }


        //position placement
        public BaseEnemyView SpawnEnemy(BaseEnemyConfig enemyConfig) {

            if (!_pools.ContainsKey(enemyConfig)) {
                _pools.Add(enemyConfig, new LinearPool<BaseEnemyView>());
            }

            BaseEnemyView obstacle = _pools[enemyConfig].GetFromPool();
            if (obstacle == null) {
                obstacle = Instantiate(enemyConfig.Prefab, transform);
                obstacle.Config = enemyConfig;
                obstacle.Init();
            }


            return obstacle;

        }


        internal void DeSpawnEnemy(BaseEnemyView obs) {
            _pools[obs.Config].GiveToPool(obs);

        }

        internal bool ProgressEnemy(BaseEnemyModel obj, float distProg) {

            bool isOutOfScreenBound = false;

            if (obj.View is IPathFollow) {
                var t = (Time.time - obj.SpawnTime) / (obj.Wave.LifeTime);
                var posRot = _bezierCalc.CalculatePathPositionAndRotation(obj.Wave.Path, t);

                Transform tr = obj.View.GetTransform();

                tr.position = posRot.pos;
                tr.rotation = Quaternion.Euler(0, 0, 90 + 360f * posRot.rot / (2 * Mathf.PI));
            } else if (obj.View is IStationary) {
                obj.View.transform.Translate(new Vector3(0, -distProg, 0), Space.World);
                isOutOfScreenBound = (obj.View.GetTransform().position.y < _settings.EnemyDestructionDist);
            }

            return isOutOfScreenBound;
        }
    }
}
