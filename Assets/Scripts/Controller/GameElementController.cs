using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;

namespace Controller {

    public class GameElementController {


        public Action OnAllEnemiesDespawned;

        private GameElementView _gameElementView;

        private SettingsConfig _settings;
        private MainStateModel _mainStateModel;
        private PlayerView _playerView;

        private List<BaseEnemyModel> _activeEnemies = new List<BaseEnemyModel>();

        private float _lastSpawn;
        
        private int _currentWave;

        internal void Init() {

            _gameElementView = ViewInitiator.GetObstacleView();

            _settings = ModelInitiator.GetSettingsConfig();
            _mainStateModel = ModelInitiator.GetMainStateModel();
            _playerView = ViewInitiator.GetPlayerView();


            _gameElementView.Init();
        }

        internal void Reset() {
            for (int i = 0; i < _activeEnemies.Count; i++) {
                DeSpawnEnemy(_activeEnemies[i]);
            }

            _activeEnemies.Clear();

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
                BaseEnemyModel enemy = new BaseEnemyModel();
                _activeEnemies.Add(enemy);

                enemy.HP = wave.Enemy.BaseHP;
                enemy.Wave = wave;

                if (wave.GrpType == WaveConfig.GroupType.Single) {
                    enemy.SpawnTime = Time.time;
                } else {
                    enemy.SpawnTime = Time.time + wave.GroupSpawnDelay * i;
                }

                enemy.View = _gameElementView.SpawnEnemy(wave.Enemy);

                enemy.View.ResetView();

                if (enemy.View is IPathFollow) {
                    enemy.View.transform.localPosition = wave.Path.PathDots[0].PointPos;
                } else if (enemy.View is IStationary) {
                    enemy.View.transform.localPosition = new Vector2(wave.StaticHorizPos, _settings.SpawnDistance);
                }



                if (enemy.View is IPathFollow || enemy.View is ITimeLimited) {
                    enemy.OutOfScopeTime = enemy.SpawnTime + wave.LifeTime;
                }

                enemy.View.Model = enemy; //the view needs a model ref

                if (enemy.View is ITracking) {
                    (enemy.View as ITracking).SetTarget(_playerView.GetShip().transform);
                }
            }
            _currentWave++;

        }

        internal void DeSpawnEnemy(BaseEnemyModel obs) {
            obs.View.SetActive(false);
            _gameElementView.DeSpawnEnemy(obs.View);
            _activeEnemies.Remove(obs);

            if (_activeEnemies.Count == 0 && _currentWave >= _settings.Levels[0].Waves.Length) {
                OnAllEnemiesDespawned();
            }

        }

        public void Tick(float distProg, float timeDelta) {

            //progress existing
            for (int i = 0; i < _activeEnemies.Count; i++) {
                var e = _activeEnemies[i];

                if (e.IsSpawned) {
                    bool isOutOfScreenBounds = _gameElementView.ProgressEnemy(e, distProg);

                    bool doDespawn = isOutOfScreenBounds;

                    if (e.View is IPathFollow || e.View is ITimeLimited) {
                        doDespawn = Time.time >= e.OutOfScopeTime;
                    }

                    if (doDespawn) {
                        //recyle object
                        DeSpawnEnemy(e);
                        i--;
                    } 
                } else {
                    if (Time.time >= e.SpawnTime) {
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
    }
}