using System;
using UnityEngine;

public class PlayerScoreManager : MonoBehaviour
{
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager= GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            _gameManager.nakamaConnection.SavePlayerScore(_gameManager.AddScore());
            _gameManager.UpdateLeaderBoard();
        }
    }
}
