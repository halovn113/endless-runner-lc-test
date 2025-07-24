using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private bool _isMoving = false;
    private int _moveIndex = 1;
    private Vector3 _targetPosition;
    public void OnAwake()
    {
        playerInput = new PlayerInput();
    }

    void OnEnable()
    {
        playerInput.Enable();
        playerInput.PlayerAction.Up.performed += ctx => MovePlayer(1);
        playerInput.PlayerAction.Down.performed += ctx => MovePlayer(-1);
        playerInput.PlayerAction.Reload.performed += ctx => Reload();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }

    public void MovePlayer(int direction)
    {
        if (GameManager.Instance.gameState != GameState.Playing)
        {
            return;
        }
        _isMoving = true;
        _moveIndex = Mathf.Clamp(direction + _moveIndex, 0, 2);
        var lanePositions = GameManager.Instance.lanePositions[_moveIndex];
        _targetPosition = new Vector3(transform.position.x, transform.position.y, lanePositions);
    }

    public void Reload()
    {
        GameManager.Instance.GetBulletManager().Reload();
    }

    public void OnUpdate()
    {
        if (GameManager.Instance.gameState != GameState.Playing)
        {
            return;
        }

        if (!_isMoving)
        {
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, 2 * Time.deltaTime);
        if (transform.position == _targetPosition)
        {
            _isMoving = false;
        }
    }
}
