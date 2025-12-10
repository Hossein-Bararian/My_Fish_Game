using UnityEngine;

public class ExitMatch_Popup : Popup
{
    public void LeaveMatch()
    {
       GameManager.Instance.LeaveMatch();
       this.Close();
    }

    public override void Close()
    {
        base.Close();
    }
}
