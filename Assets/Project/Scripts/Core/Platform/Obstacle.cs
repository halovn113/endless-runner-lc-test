using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float currentHealth;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            EventManager.ObstacleDestroyed(gameObject, other.tag);
            var gameManager = GameManager.Instance;
            gameManager.GetPlayer().TakeDamage(gameManager.GetPlatformController().damage);
        }
        else if (other.tag == "Bullet")
        {
            GameManager.Instance.GetBulletManager().DestroyBullet(other.gameObject);
            TakeDamage(GameManager.Instance.GetPlayer().damage);
        }
    }

    private void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        if (currentHealth <= 0)
        {
            EventManager.ObstacleDestroyed(gameObject, "Bullet");
        }
    }

    public void ResetHealth(float maxHealth)
    {
        currentHealth = maxHealth;
    }

}
