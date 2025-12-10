using UnityEngine;

public class PlayerNetworkLocalSync : MonoBehaviour
{  
    public float stateFrequency = 0.1f;
    
    private GameManager _gameManager;
    private Rigidbody2D _playerRigidbody;
    private Transform _playerTransform;
    private float _stateSyncTimer;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _playerRigidbody = GetComponentInChildren<Rigidbody2D>();
        _playerTransform = _playerRigidbody.GetComponent<Transform>();
    }
    private void LateUpdate()
    {
        if (_stateSyncTimer <= 0)
        {
            _gameManager.SendMatchState(OpCodes.VelocityAndPosition, MatchDataJson.VelocityPosition(_playerRigidbody.linearVelocity, _playerTransform.position));
            _stateSyncTimer = stateFrequency;
        }
        _stateSyncTimer -= Time.deltaTime;
     
    }
}
