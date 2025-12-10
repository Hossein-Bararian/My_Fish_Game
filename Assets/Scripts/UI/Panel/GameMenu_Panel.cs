using System.Threading.Tasks;
using UnityEngine;

public class GameMenu_Panel : Panel
{
    private void Start()
    {
       // ClientCoordinator.Instance.CloseAllPopup();
        Debug.Log("Game Menu Panel");
    }
    
    public void LeaveMatchButton()
    {
        ClientCoordinator.Instance.OpenPopup<ExitMatch_Popup>(true);
    }
}
