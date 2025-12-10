using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userName;
    [SerializeField] private TextMeshProUGUI userScore;
    public void SetupItem(string username, string score)
    {
        userName.text = username;
        userScore.text = score;
    }
}
