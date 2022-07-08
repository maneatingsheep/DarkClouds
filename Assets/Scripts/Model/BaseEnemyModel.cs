using View;


namespace Model {
    public class BaseEnemyModel {
        public BaseEnemyView View;
        public int HP;
        public BaseEnemyConfig Config;
        public bool IsSpawned = false;

        public float SpawnTime;
        public float OutOfScopeTime;

        public WaveConfig Wave;
    }
}