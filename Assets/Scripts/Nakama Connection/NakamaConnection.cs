using System;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

[CreateAssetMenu] [Serializable]
public class NakamaConnection : ScriptableObject
{
    public string scheme = "http";
    public string host = "localhost";
    public int port = 7349;
    public string serverKey = "defaultkey";
    
    public IClient Client;
    public ISession Session;
    public ISocket Socket;
    
    public async Task Connect(string playerTag) 
    {
        Client = new Nakama.Client(scheme, host, port, serverKey, UnityWebRequestAdapter.Instance);
        var deviceId = playerTag ;
        
        Session = await Client.AuthenticateDeviceAsync(deviceId);
        var resault = await SetRandomUserName();
        await Client.UpdateAccountAsync(Session,resault);
        Debug.Log($"user name : {Session.Username}");
        Socket= Client.NewSocket();
        await Socket.ConnectAsync(Session, true);
        Debug.Log($"Connected : {Socket.IsConnected}");
    }
    
    public async Task<string> SetRandomUserName()
    {
        var response = await Client.RpcAsync(Session,"random_username","");
        return response.Payload;
    }

    public async Task FindMatch()
    {
        var matchmaker =await Socket.AddMatchmakerAsync("*", 2, 2);
        Debug.Log($"Matchmaking start !  match Ticket :{matchmaker.Ticket}");
        
    }

    public async void SavePlayerScore(int score)
    {
        var playerData = new PlayerData()
        {
            score = score,
            playerId = Session.UserId,
            
        };
        
        string jasonData = JsonUtility.ToJson(playerData);

        var storageObj = new WriteStorageObject
        {
            Collection = "Score",
            Key = "2v2",
            Value = jasonData,
            PermissionRead = 1,
            PermissionWrite = 1,
        };
       var saveData= await Client.WriteStorageObjectsAsync(Session, new IApiWriteStorageObject[] { storageObj });
        Debug.Log($"Player Data Saved");
    }
    
    public async Task<int> LoadPlayerScore()
    {
        var ids = new StorageObjectId
        {
            Collection = "Score",
            Key = "2v2",
            UserId = Session.UserId
        };
       var result = await Client.ReadStorageObjectsAsync(Session,new IApiReadStorageObjectId[] {ids});

       if (!result.Objects.Any())
       {
           Debug.Log("PlayerData did not Found");
       }
       else
       {
           var jsonDataLoaded = result.Objects.First().Value; 
            var data = JsonUtility.FromJson<PlayerData>(jsonDataLoaded);
            return data.score;
       }

       return 0;
    }
    
}
