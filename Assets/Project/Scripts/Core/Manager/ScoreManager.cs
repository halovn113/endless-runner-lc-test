using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    [Header("Score Settings")]
    [SerializeField] private int pointsPerSecond = 10;
    [SerializeField] private int pointsPerObstacle = 50;
    [SerializeField] private int pointsPerAmmoPickup = 20;

    private int _currentScore;
    private float _timeScore;

    public int CurrentScore => _currentScore;

    public void OnStart()
    {
        EventManager.OnObstacleDestroyed += HandleObstacleDestroyed;
        EventManager.OnGameStart += ResetScore;
    }

    public void OnUpdate()
    {
        if (GameManager.Instance.gameState == GameState.Playing)
        {
            _timeScore += Time.deltaTime;

            if (_timeScore >= 1f)
            {
                AddScore(pointsPerSecond);
                _timeScore = 0f;
            }
        }
    }

    public void AddScore(int points)
    {
        _currentScore += points;
        EventManager.ScoreChanged(_currentScore);
    }

    private void HandleObstacleDestroyed(GameObject obstacle, string tag)
    {
        if (tag == "Bullet")
        {
            AddScore(pointsPerObstacle);
        }
    }

    private void ResetScore()
    {
        _currentScore = 0;
        _timeScore = 0f;
        EventManager.ScoreChanged(_currentScore);
    }

    private void OnDestroy()
    {
        EventManager.OnObstacleDestroyed -= HandleObstacleDestroyed;
        EventManager.OnGameStart -= ResetScore;
    }
}
