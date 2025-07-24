using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlatformController : MonoBehaviour
{
    public PlatformData platformData;
    public GameObject[] listPlatforms;
    private GameObject _lastGround;

    [Header("Obstacle")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private float spawnRate = 2f;
    public float obstacleMaxHealth = 100f;
    public int damage = 20;

    private ObjectPool<GameObject>[] _listPool;
    private Coroutine _spawnCoroutine;
    private float _currentSpawnRate;
    private List<GameObject> _listMovingObstacles = new List<GameObject>();
    private Vector3[] _groundOriginPosition;
    private float segmentLength;

    public void OnUpdate()
    {
        var deltaSpeed = platformData.maxSpeed * Time.deltaTime;
        for (int i = 0; i < listPlatforms.Length; i++)
        {
            var ground = listPlatforms[i];
            if (ground.transform.position.x <= -(segmentLength * 2))
            {
                for (int j = 0; j < ground.transform.childCount; j++)
                {
                    var child = ground.transform.GetChild(j);
                    int index = int.Parse(child.gameObject.name.Split('_')[1]);
                    if (index >= 0 && index < _listPool.Length)
                    {
                        _listPool[index].Release(child.gameObject);
                    }
                }
                var x = _lastGround.transform.position.x + segmentLength - deltaSpeed;
                ground.transform.position = new Vector3(x, 0, 0);
                _lastGround = ground;
            }
            else
            {
                ground.transform.Translate(Vector3.left * deltaSpeed);
            }
        }

    }

    public void OnAwake()
    {
        _currentSpawnRate = spawnRate;
        _groundOriginPosition = new Vector3[listPlatforms.Length];
        EventManager.OnGameStart += StartSpawn;
        EventManager.OnGameOver += StopSpawn;
        EventManager.OnObstacleDestroyed += DestroyObstacle;
        EventManager.OnDifficultyChanged += HandleDifficultyChange;

        _listPool = new ObjectPool<GameObject>[obstaclePrefabs.Length];

        for (int i = 0; i < _groundOriginPosition.Length; i++)
        {
            var pos = listPlatforms[i].transform.position;
            _groundOriginPosition[i] = new Vector3(pos.x, pos.y, pos.z);
        }

        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            int index = i;
            _listPool[i] = new ObjectPool<GameObject>(() => CreateObstacle(index), OnGetObstacleFromPool, OnReturnObstacleToPool, null, true, 5);
        }
        _lastGround = listPlatforms[listPlatforms.Length - 1];
        segmentLength = _lastGround.GetComponent<Renderer>().bounds.size.x;
        StartSpawn();
    }

    private void StartSpawn()
    {
        for (int i = 0; i < _groundOriginPosition.Length; i++)
        {
            listPlatforms[i].transform.position = _groundOriginPosition[i];
        }

        _spawnCoroutine = StartCoroutine(SpawnObstacles());
    }

    private void StopSpawn()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }

        for (int i = 0; i < _listMovingObstacles.Count; i++)
        {
            _listPool[int.Parse(_listMovingObstacles[i].name.Split('_')[1])].Release(_listMovingObstacles[i]);
        }

        _listMovingObstacles.Clear();
    }

    private IEnumerator SpawnObstacles()
    {
        var lanePositions = GameManager.Instance.lanePositions;
        while (GameManager.Instance.gameState == GameState.Playing)
        {
            int randomIndex = UnityEngine.Random.Range(0, obstaclePrefabs.Length);
            GameObject obstacle = _listPool[randomIndex].Get();
            obstacle.transform.position = new Vector3(segmentLength * 2, 1, lanePositions[UnityEngine.Random.Range(0, lanePositions.Length)]);
            obstacle.transform.SetParent(_lastGround.transform);
            yield return new WaitForSeconds(_currentSpawnRate);
        }
    }

    private GameObject CreateObstacle(int index)
    {
        var newObstacle = Instantiate(obstaclePrefabs[index]);
        newObstacle.gameObject.name = "Obstacle_" + index;
        return newObstacle;
    }

    private void OnGetObstacleFromPool(GameObject obstacle)
    {
        obstacle.SetActive(true);
        obstacle.GetComponent<Obstacle>().ResetHealth(obstacleMaxHealth);
        _listMovingObstacles.Add(obstacle);
    }

    private void OnReturnObstacleToPool(GameObject obstacle)
    {
        obstacle.transform.SetParent(null);
        obstacle.SetActive(false);
        _listMovingObstacles.Remove(obstacle);
    }

    public void DestroyObstacle(GameObject obstacle, string tag)
    {
        _listPool[int.Parse(obstacle.name.Split('_')[1])].Release(obstacle);
    }

    private void HandleDifficultyChange(float difficultyMultiplier)
    {
        _currentSpawnRate = spawnRate / difficultyMultiplier;
        _currentSpawnRate = Mathf.Max(1, _currentSpawnRate); // Minimum spawn rate
    }

}
