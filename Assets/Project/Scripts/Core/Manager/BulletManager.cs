using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private ObjectPool<GameObject> bulletPool;

    private bool _isReloading = false;
    private bool _canShoot = true;
    private Coroutine _shootCoroutine;
    public int MaxAmmo => maxAmmo;
    private List<GameObject> _listMovingBullet = new List<GameObject>();

    public void OnAwake()
    {
        bulletPool = new ObjectPool<GameObject>(CreateBullet, OnGetObstacleFromPool, OnReturnObstacleToPool, null, true, 20);
    }

    public void OnStart()
    {
        currentAmmo = maxAmmo;
        EventManager.PlayerAmmoChanged(currentAmmo);
        EventManager.OnGameStart += ResetAmmo;
        EventManager.OnGameStart += Shoot;
    }

    public void Shoot()
    {
        if (!_canShoot || _isReloading || currentAmmo <= 0 ||
            GameManager.Instance.gameState != GameState.Playing)
            return;

        _shootCoroutine = StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
        while (GameManager.Instance.gameState == GameState.Playing)
        {
            GameObject bullet = bulletPool.Get();
            bullet.transform.position = firePoint.position;

            currentAmmo--;
            EventManager.PlayerAmmoChanged(currentAmmo);

            if (currentAmmo <= 0)
            {
                Reload();
            }

            yield return new WaitForSeconds(fireRate);
        }
    }

    public void Reload()
    {
        if (_isReloading || currentAmmo >= maxAmmo)
            return;
        if (_shootCoroutine != null)
        {
            StopCoroutine(_shootCoroutine);
        }

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        _isReloading = true;
        EventManager.ShowReloading(true);
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        EventManager.PlayerAmmoChanged(currentAmmo);
        EventManager.ShowReloading(false);
        _isReloading = false;
        Shoot();
    }

    private void ResetAmmo()
    {
        currentAmmo = maxAmmo;
        _isReloading = false;
        _canShoot = true;
        if (_shootCoroutine != null)
        {
            StopCoroutine(_shootCoroutine);
        }

        EventManager.PlayerAmmoChanged(currentAmmo);
    }

    private void OnDestroy()
    {
        EventManager.OnGameStart -= ResetAmmo;
    }

    private GameObject CreateBullet()
    {
        return Instantiate(bulletPrefab);
    }

    private void OnGetObstacleFromPool(GameObject bullet)
    {
        bullet.SetActive(true);
        _listMovingBullet.Add(bullet);
    }

    private void OnReturnObstacleToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        _listMovingBullet.Remove(bullet);
    }

    public void DestroyBullet(GameObject bullet)
    {
        if (_listMovingBullet.Contains(bullet))
        {
            _listMovingBullet.Remove(bullet);
            bulletPool.Release(bullet);
        }
    }

    public void OnUpdate()
    {
        var deltaSpeed = Time.deltaTime * moveSpeed;
        for (int i = _listMovingBullet.Count - 1; i >= 0; i--)
        {
            var bullet = _listMovingBullet[i];
            if (bullet == null) continue;

            if (bullet.transform.position.x >= 10f)
            {
                _listMovingBullet.RemoveAt(i);
                bulletPool.Release(bullet);
            }
            else
            {
                bullet.transform.Translate(Vector3.down * deltaSpeed); // vector down for shooting right
            }
        }
    }
}
