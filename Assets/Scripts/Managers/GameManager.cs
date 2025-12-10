using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Nakama;
using TMPro;
#if UNITY_EDITOR
using Unity.Multiplayer.Playmode;
#endif


public class OpCodes
{
    public const long VelocityAndPosition = 1;
}

public class GameManager : MonoBehaviour
{
    public static int Score=0; 
    public NakamaConnection nakamaConnection;
    public MainMenu_Panel mainMenuPanel;
    public GameObject networkLocalPlayerPrefab;
    public GameObject networkRemotePlayerPrefab;
    public GameObject spawnPoints;
    public MenuManager menuManager;

    private GameObject _localPlayerGameObject;
    private IUserPresence _localUser;
    private IMatch _currentMatch;
    private IDictionary<string, GameObject> _players;

    private async void Start()
    {
        await SetupGameManager();
    }

    public async Task SetupGameManager()
    {
        _players=new Dictionary<string, GameObject>();
        var mainThread = UnityMainThreadDispatcher.Instance();
        await nakamaConnection.Connect(GetNakamaDeviceId());
        mainMenuPanel.ShowUserId();
        menuManager.EnableFindMatchButton();
        nakamaConnection.Socket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnReceivedMatchPresence(m));
        nakamaConnection.Socket.ReceivedMatchmakerMatched += m => mainThread.Enqueue(() => OnReceivedMatchmakerMatched(m));
        //nakamaConnection.Socket.ReceivedMatchState += m => mainThread.Enqueue(async () =>await OnReceivedMatchState(m));
    }

    // private async Task OnReceivedMatchState(IMatchState matchState)
    // {
    //     Debug.Log("OnReceivedMatchState Start"+matchState.OpCode.ToString());
    // }

    #region Singleton

    public static GameManager Instance { get; private set; } = null;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    #endregion
    
    public string GetNakamaDeviceId()
    {
#if UNITY_EDITOR

        foreach (var tag in  CurrentPlayer.ReadOnlyTags())
        {
            switch (tag)
            {
                case "EditorPlayer":
                    return "EditorPlayer123456789";
                case "Client1":
                    return "Client1123456789";
                case "Client2":
                    return "Client2123456789";
                case "Client3":
                    return "Client3123456789";
            }
        }
#else
   
#endif
        return "123456789123456789";  // Test :)
    }

    private async void OnReceivedMatchmakerMatched(IMatchmakerMatched matchmakerMatched)
    {
        _localUser = matchmakerMatched.Self.Presence;

        var match = await nakamaConnection.Socket.JoinMatchAsync(matchmakerMatched);
        menuManager.ActiveGameMenuPanel(true);
        foreach (var user in match.Presences)
        {
            SpawnPlayer(match.Id, user);
        }

        _currentMatch = match;
        LoadPlayerData();
    }

    private async void LoadPlayerData()
    {
       Score=await nakamaConnection.LoadPlayerScore();
       UpdateScore();
    }

    private void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        foreach (var user in matchPresenceEvent.Joins)
        {
            SpawnPlayer(matchPresenceEvent.MatchId,user);
        }
        foreach (var user in matchPresenceEvent.Leaves)
        {
            Destroy(_players[user.SessionId]);
            _players.Remove(user.SessionId);
        }
    }

    void SpawnPlayer(string matchId,IUserPresence user)
    {
        if (_players.ContainsKey(user.SessionId)) return;
        var isLocal = user.SessionId == _localUser.SessionId;
        var playerPrefab=isLocal? networkLocalPlayerPrefab:networkRemotePlayerPrefab;
        var spawnPoint = spawnPoints.transform.GetChild(Random.Range(0, spawnPoints.transform.childCount - 1));
        var player = Instantiate(playerPrefab,spawnPoint.transform.position,Quaternion.identity);
        
        _players.Add(user.SessionId, player);
        if (isLocal)
            _localPlayerGameObject = player;
        else
        {
            player.GetComponent<PlayerNetworkRemoteSync>().NetworkData = new RemotePlayerNetworkData
            {
                MatchId = matchId,
                User = user
            };
        }
    }

    public async Task SendMatchStateAsync(long opCode, string state)
    {
       await nakamaConnection.Socket.SendMatchStateAsync(_currentMatch.Id,opCode, state);
    }
    
    public void SendMatchState(long opCode, string state)
    {
        nakamaConnection.Socket.SendMatchStateAsync(_currentMatch.Id, opCode, state);
    }

    void UpdateScore()
    {
        menuManager.UpdateScoreUI();
    }
    public int AddScore()
    {
        Score+=10;
        menuManager.UpdateScoreUI();
        return Score;
    }

    public async void LeaveMatch()
    {
        print($"Socket match is connect : {nakamaConnection.Socket.IsConnected}");
        print($"Socket match : {nakamaConnection.Socket}");
        
        await nakamaConnection.Socket.LeaveMatchAsync(_currentMatch);
        menuManager.ActiveGameMenuPanel(false);
        foreach (var player in _players.Values)
        {
            Destroy(player);
        }
        _players.Clear();
        _currentMatch=null;
        _localUser=null;
       await SetupGameManager();
    }

    public async void UpdateLeaderBoard()
    {
    var leaderboardRecord= await nakamaConnection.Client.WriteLeaderboardRecordAsync(nakamaConnection.Session, "leaderboard1", Score);
        Debug.Log($" Leaderboard Score :{leaderboardRecord.Score} |  Rank:{leaderboardRecord.Rank}" );
    }
}
