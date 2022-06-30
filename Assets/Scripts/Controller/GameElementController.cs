using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;

namespace Controller {

    public class GameElementController {

        private GameElementView _gameElementView;

        private SettingsConfig _settings;
        private MainStateModel _mainStateModel;

        private List<BaseEnemyModel> _activeEnemies = new List<BaseEnemyModel>();

        private float _lastSpawn;
        
        private int _currentWave;

        internal void Init() {

            _gameElementView = ViewInitiator.GetObstacleView();

            _settings = ModelInitiator.GetSettingsConfig();
            _mainStateModel = ModelInitiator.GetMainStateModel();

            _gameElementView.Init();
        }

        internal void Reset() {
            _gameElementView.ResetView(_activeEnemies);
            _lastSpawn = 0;
            _currentWave = 0;
        }

        public void StartGame() {
            

        }

        private void SpawnEnemyWave() {

            WaveConfig wave = _settings.Levels[0].Waves[_currentWave];

            _lastSpawn += wave.GapToPrevious;

            int enemyCount = (wave.GrpType == WaveConfig.GroupType.Single) ? 1 : wave.GroupSize;

            for (int i = 0; i < enemyCount; i++) {
                BaseEnemyModel oc = new BaseEnemyModel();
                _activeEnemies.Add(oc);

                oc.HP = wave.Enemy.BaseHP;
                oc.Wave = wave;

                if (wave.GrpType == WaveConfig.GroupType.Single) {
                    oc.SpawnDelay = 0;
                } else {
                    oc.SpawnDelay = wave.GroupSpawnDelay * i;
                    oc.PathStartTime = Time.time + oc.SpawnDelay;
                    oc.PathEndTime = oc.PathStartTime + wave.PathTime;
                }
                

                switch (wave.MovType) {
                    case WaveConfig.MovementType.Static:
                        oc.View = _gameElementView.SpawnEnemy(wave.Enemy, new Vector2(wave.StaticHorizPos, _settings.SpawnDistance));
                        break;
                    default:
                        oc.View = _gameElementView.SpawnEnemy(wave.Enemy, wave.Path.PathDots[0].PointPos);
                        break;
                }

                oc.View.Model = oc; //the view needs a model ref
            }


            _currentWave++;

        }

        internal void OnEnemyHit(GameObject target, int damage) {
            for (int i = 0; i < _activeEnemies.Count; i++) {
                if (target == _activeEnemies[i].View.gameObject) {
                    _activeEnemies[i].HP -= damage;
                    if (_activeEnemies[i].HP <= 0) {
                        DeSpawnEnemy(_activeEnemies[i]);
                    }
                    break;
                }
            }
        }

        internal void DeSpawnEnemy(BaseEnemyModel obs) {
            _gameElementView.DeSpawnEnemy(obs.View);
            _activeEnemies.Remove(obs);

        }

        public void Tick(float distProg, float timeDelta) {

            //progress existing
            for (int i = 0; i < _activeEnemies.Count; i++) {
                var e = _activeEnemies[i];

                if (e.IsSpawned) {
                    bool isOutOfScope = _gameElementView.ProgressEnemy(e, distProg);

                    if (isOutOfScope) {
                        //recyle object
                        DeSpawnEnemy(e);
                        i--;
                    } 
                } else {
                    e.SpawnDelay -= timeDelta;
                    if (e.SpawnDelay <= 0) {
                        e.IsSpawned = true;
                        e.View.SetActive(true);
                    }
                }

                
            }

            //spawn next
            if (_mainStateModel.FlowState == MainStateModel.FState.Gameplay) {
                while (_currentWave < _settings.Levels[0].Waves.Length && _lastSpawn + _settings.Levels[0].Waves[_currentWave].GapToPrevious - _mainStateModel.ShipProgress <= 0 ) {
                    SpawnEnemyWave();
                }
            }

        }
    }
}