using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Nakama;

public class Leaderboard_Panel : Panel
{
  [SerializeField] private GameObject leaderboardItemPrefab;
  [SerializeField] private RectTransform contentRectTransform;
  [SerializeField] private NakamaConnection nakamaConecction;
  private async void Start()
  {
    await LoadLeaderboardData();
  }

  private async Task LoadLeaderboardData()
  {
    var scoresResponse = await nakamaConecction.Client.ListLeaderboardRecordsAsync(nakamaConecction.Session, "leaderboard1", limit: 10);
    foreach (IApiLeaderboardRecord record in scoresResponse.Records)
    {
      var item = Instantiate(leaderboardItemPrefab, contentRectTransform);
      item.GetComponent<LeaderboardItem>().SetupItem(record.Username, record.Score);
    }
  }

  public async void BackToMenuButton()
  {
    ClientCoordinator.Instance.OpenPanel<MainMenu_Panel>(false);
    await GameManager.Instance.SetupGameManager();
  }
}
