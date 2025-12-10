
using System;
using TMPro;
using UnityEngine;

public class MainMenu_Panel : Panel
{
  [SerializeField]private TextMeshProUGUI userNameText; 
 
 

  public void ShowUserId()
  {
    userNameText.text = GameManager.Instance.nakamaConnection.Session.Username;
  }
  
  public void ExitGameButton()
  {
    ClientCoordinator.Instance.OpenPopup<ExitGame_Popup>(true);
  }
  public void LeaderboardButton()
  {
    ClientCoordinator.Instance.OpenPanel<Leaderboard_Panel>(false);
  }
}
