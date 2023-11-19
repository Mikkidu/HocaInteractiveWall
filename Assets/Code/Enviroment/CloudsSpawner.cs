using UnityEngine;


namespace HocaInk.InteractiveWall
{


    public class CloudsSpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private CloudController[] _cloudPrefabs;
        [SerializeField] private float _maxSpawnInterval;
        [SerializeField] private float _minSpawnInterval;

        private float _spawnTrigger;

        void Update()
        {
            if (Time.time > _spawnTrigger)
            {
                _spawnTrigger = Time.time + Random.Range(_minSpawnInterval, _maxSpawnInterval);
                var cloudIndex = Random.Range(0, _cloudPrefabs.Length);
                var pointIndex = Random.Range(0, _spawnPoints.Length);
                Instantiate(
                    _cloudPrefabs[cloudIndex], 
                    _spawnPoints[pointIndex].position, 
                    _spawnPoints[pointIndex].rotation, 
                    transform)
                    .Initialize(pointIndex + 1);
            }
        }
    }


}
