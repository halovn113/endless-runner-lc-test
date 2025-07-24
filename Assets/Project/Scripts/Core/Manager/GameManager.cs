using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : Singleton<GameManager>
{
    [Header("Player Information")]
    public PlayerData playerData;
    public int difficultyIncreaseInterval;
    public int speedIncreaseRate = 1;
    public int maxGameSpeed = 5;
    public int baseGameSpeed;

    [Header("Manager")]
    [SerializeField]
    private PlatformController platformController;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private ScoreManager scoreManager;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private Player player;
    [SerializeField]
    private BulletManager bulletManager;
    private Coroutine _difficultyCoroutine;
    private int _currentGameSpeed;
    private float _gameTime;
    public GameState gameState;

    public float[] lanePositions = { -1.5f, 0f, 1.5f };

    protected override void Awake()
    {
        base.Awake();
        platformController.OnAwake();
        playerController.OnAwake();
        bulletManager.OnAwake();
        EventManager.OnPlayerDied += HandlePlayerDeath;
        _currentGameSpeed = baseGameSpeed;
    }

    public void Start()
    {
        uiManager.OnStart();
        scoreManager.OnStart();
        bulletManager.OnStart();
        EventManager.ShowMenu();
    }

    void Update()
    {
        if (gameState == GameState.Playing)
        {
            platformController.OnUpdate();
            playerController.OnUpdate();
            scoreManager.OnUpdate();
            bulletManager.OnUpdate();
        }
    }

    public void StartGame()
    {
        gameState = GameState.Playing;

        _gameTime = 0f;
        _currentGameSpeed = baseGameSpeed;

        EventManager.GameStart();

        if (_difficultyCoroutine != null)
            StopCoroutine(_difficultyCoroutine);
        _difficultyCoroutine = StartCoroutine(IncreaseDifficulty());

    }

    public Player GetPlayer()
    {
        return player;
    }

    public PlatformController GetPlatformController()
    {
        return platformController;
    }

    private void OnDestroy()
    {
        EventManager.OnPlayerDied -= HandlePlayerDeath;
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;

        if (_difficultyCoroutine != null)
        {
            StopCoroutine(_difficultyCoroutine);
            _difficultyCoroutine = null;
        }

        EventManager.GameOver();
    }

    private void HandlePlayerDeath()
    {
        GameOver();
    }

    public BulletManager GetBulletManager()
    {
        return bulletManager;
    }

    private IEnumerator IncreaseDifficulty()
    {
        while (gameState == GameState.Playing)
        {
            yield return new WaitForSeconds(difficultyIncreaseInterval);

            _currentGameSpeed = Mathf.Clamp(_currentGameSpeed + speedIncreaseRate, baseGameSpeed, maxGameSpeed);
            EventManager.DifficultyChanged(_currentGameSpeed / baseGameSpeed);
        }
    }

}
