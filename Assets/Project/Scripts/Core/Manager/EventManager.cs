using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static event Action<int> OnPlayerHealthChanged;
    public static event Action OnPlayerDied;
    public static event Action<int> OnPlayerAmmoChanged;

    // Game Events
    public static event Action<int> OnScoreChanged;
    public static event Action OnGameStart;
    public static event Action OnGameOver;
    public static event Action<float> OnDifficultyChanged;
    public static event Action OnShowMenu;
    public static event Action<bool> OnShowReload;


    // Obstacle Events
    public static event Action<GameObject, string> OnObstacleDestroyed;
    public static event Action<Vector3> OnObstacleHit;

    // Methods to trigger events
    public static void PlayerHealthChanged(int health) => OnPlayerHealthChanged?.Invoke(health);
    public static void PlayerDied() => OnPlayerDied?.Invoke();
    public static void PlayerAmmoChanged(int ammo) => OnPlayerAmmoChanged?.Invoke(ammo);

    public static void ScoreChanged(int score) => OnScoreChanged?.Invoke(score);
    public static void GameStart() => OnGameStart?.Invoke();
    public static void GameOver() => OnGameOver?.Invoke();
    public static void DifficultyChanged(float difficulty) => OnDifficultyChanged?.Invoke(difficulty);

    public static void ObstacleDestroyed(GameObject obstacle, string tag) => OnObstacleDestroyed?.Invoke(obstacle, tag);
    public static void ObstacleHit(Vector3 position) => OnObstacleHit?.Invoke(position);
    public static void ShowReloading(bool show) => OnShowReload?.Invoke(show);
    public static void ShowMenu() => OnShowMenu?.Invoke();

}
