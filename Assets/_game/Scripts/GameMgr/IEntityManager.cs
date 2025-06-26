using UnityEngine;

public interface IEntityManager
{
    public void SpawnTurret(Vector3Int tilePosition, Vector3 pos, string turretName);
    // public void DeSpawnTurret(TurretCtrl turret);
    public void DespawnTurret(Vector3Int tilePosition);
    // public void SpawnEnemy(string name);
    // public void DeSpawnEnemy(EnemyCtrl enemy);
}