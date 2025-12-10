using System;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public NakamaConnection nakamaConnection;
    public Button findMatchButton;
    public TextMeshProUGUI  txtScore;
    public async void FindMatch()
    {
        await nakamaConnection.FindMatch();
    }
    public void EnableFindMatchButton()
    {
        print("Enable Button Interactable");
        findMatchButton= FindAnyObjectByType<MainMenu_Panel>().GetComponentInChildren<Button>();
        findMatchButton.interactable = true;
    }

    private void Start()
    {
        ClientCoordinator.Instance.OpenPanel<MainMenu_Panel>(false); 
    }

    public void ActiveGameMenuPanel(bool isActive)
    {
        if (isActive)
        {
            ClientCoordinator.Instance.OpenPanel<GameMenu_Panel>(false);
            txtScore=FindAnyObjectByType<GameMenu_Panel>().GetComponentInChildren<TextMeshProUGUI>();
        }
        else
            ClientCoordinator.Instance.OpenPanel<MainMenu_Panel>(false);
    }

    public void UpdateScoreUI()
    {
        txtScore.text = "Score: " + GameManager.Score;
    }
}
