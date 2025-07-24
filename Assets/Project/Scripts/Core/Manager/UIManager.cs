using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("UI Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Game UI Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI reloadText;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;

    public void OnStart()
    {
        EventManager.OnPlayerHealthChanged += UpdateHealthUI;
        EventManager.OnPlayerAmmoChanged += UpdateAmmoUI;
        EventManager.OnScoreChanged += UpdateScoreUI;
        EventManager.OnGameStart += ShowGameUI;
        EventManager.OnGameOver += ShowGameOverUI;
        EventManager.OnShowMenu += ShowMenu;
        EventManager.OnShowReload += ShowReloadUI;
        reloadText.gameObject.SetActive(false);
    }

    private void UpdateHealthUI(int health)
    {
        if (healthText != null)
            healthText.text = "Health: " + health;
    }

    private void UpdateAmmoUI(int ammo)
    {
        if (ammoText != null)
            ammoText.text = "Ammo: " + ammo;
    }

    private void UpdateScoreUI(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void ShowReloadUI(bool show)
    {
        reloadText.gameObject.SetActive(show);
    }

    private void ShowGameUI()
    {
        menuPanel?.SetActive(false);
        gamePanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
    }

    private void ShowMenu()
    {
        menuPanel?.SetActive(true);
        gamePanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
    }

    private void ShowGameOverUI()
    {
        gamePanel?.SetActive(false);
        gameOverPanel?.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Final Score:" + ScoreManager.Instance.CurrentScore;
    }
}
