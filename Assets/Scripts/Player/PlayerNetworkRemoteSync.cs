using System;
using System.Collections.Generic;
using UnityEngine;
using Nakama.TinyJson;
using System.Text;
using Nakama;
public class RemotePlayerNetworkData
{
    public string MatchId;
    public IUserPresence User;
}

public class PlayerNetworkRemoteSync : MonoBehaviour
{
    public RemotePlayerNetworkData NetworkData;
    private GameManager _gameManager;
    private Rigidbody2D _rigidbody2D;
    private Transform _playerTransform;

    public float LerpTime = 0.05f;
    private float lerpTimer;
    private Vector3 lerpFromPosition;
    private Vector3 lerpToPosition;
    private bool lerpPosition;
    private void Start()
    { 
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        _playerTransform = _rigidbody2D.GetComponent<Transform>(); 
        _gameManager.nakamaConnection.Socket.ReceivedMatchState += EnqueueOnReceivedMatchState;
    }

    
    private void OnDestroy()
    {
        if (_gameManager != null)
        {
            _gameManager.nakamaConnection.Socket.ReceivedMatchState -= EnqueueOnReceivedMatchState;
        }
    }
    private void LateUpdate()
    {
        if (!lerpPosition)
        {
            return;
        }
        _playerTransform.position = Vector3.Lerp(lerpFromPosition, lerpToPosition, lerpTimer / LerpTime);
        lerpTimer += Time.deltaTime;
        
        if (lerpTimer >= LerpTime)
        {
            _playerTransform.position = lerpToPosition;
            lerpPosition = false;
        }
    }
    private void EnqueueOnReceivedMatchState(IMatchState matchState)
    {
        var mainThread = UnityMainThreadDispatcher.Instance();
        mainThread.Enqueue(() => OnReceivedMatchState(matchState));
    }
    
    private void OnReceivedMatchState(IMatchState matchState)
    {
        
        if (matchState.UserPresence.SessionId != NetworkData.User.SessionId)
        {
            return;
        }
        switch (matchState.OpCode)
        {
            case OpCodes.VelocityAndPosition:
                UpdateVelocityAndPositionFromState(matchState.State);
                break;
            default:
                break;
        }
    }
    private IDictionary<string, string> GetStateAsDictionary(byte[] state)
    {
        return Encoding.UTF8.GetString(state).FromJson<Dictionary<string, string>>();
    }
    
    private void UpdateVelocityAndPositionFromState(byte[] matchStateState)
    {
        var stateDictionary = GetStateAsDictionary(matchStateState);

        _rigidbody2D.linearVelocity = new Vector2(float.Parse(stateDictionary["velocity.x"]), float.Parse(stateDictionary["velocity.y"]));

        var position = new Vector3(
            float.Parse(stateDictionary["position.x"]),
            float.Parse(stateDictionary["position.y"]),
            0);

       
        lerpFromPosition = _playerTransform.position;
        lerpToPosition = position;
        lerpTimer = 0;
        lerpPosition = true;
    }
}
