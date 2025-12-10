using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public class ClientCoordinator : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas;
    
    #region Singleton

    public static ClientCoordinator Instance { get; private set; } = null;

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

    public bool hasPurchase { get; set; } = true;

    

    #region Overlay

    #region ---- PANEL  ----

    private List<Panel> panelList = new List<Panel>();

    public T OpenPanel<T>(bool keepLastPanel) where T : Panel
    {
        panelList.RemoveAll((t) => !t);

        Panel lastPanel = null;

        if (panelList.Count > 0)
            lastPanel = panelList[0];

        GameObject panelGameObject =
            Addressables.InstantiateAsync(typeof(T).Name, mainCanvas.transform).WaitForCompletion();


        Panel panel = panelGameObject.GetComponent<T>();


        panelList.Add(panel);

        if (!keepLastPanel && lastPanel)
        {
            panelList.Remove(lastPanel);

            GameObject lastPanelGameObject = lastPanel.gameObject;

            Destroy(lastPanel);

            StartCoroutine(OpenAnimation(panel, () => { Addressables.ReleaseInstance(lastPanelGameObject); }));
        }
        else
        {
            StartCoroutine(OpenAnimation(panel));
        }

        return (T)panel;        
    }

    public void ClosePanel(Panel panel, Action onClosed = null)
    {
        if (!panelList.Contains(panel))
            return;

        panelList.Remove(panel);

        Addressables.ReleaseInstance(panel.gameObject);

        if (panelList.Count == 0)
        {
            //OpenPanel<Panel_Main>(false).Setup();
        }
    }

    #endregion // Panel 

    #region ---- POPUP ----

    public List<string> ShownPopups { get; set; } = new List<string>();


    private List<Popup> popupList = new List<Popup>();

    public T OpenPopup<T>(bool showOnTop) where T : Popup
    {
        popupList.RemoveAll((t) => !t);   

   

        GameObject popupGameObject =
            Addressables.InstantiateAsync(typeof(T).Name, mainCanvas.transform).WaitForCompletion();
  
        Popup popup = popupGameObject.GetComponent<T>();

        if (popupList.Count != 0 && !showOnTop)
            popup.content.SetActive(false);
        else
            StartCoroutine(OpenAnimation(popup));

        if (showOnTop)
            popupList.Insert(0, popup);
        else
            popupList.Add(popup);

        return (T)popup;
    }

    public void ClosePopup(Popup popup, Action onClosed = null)
    {
        if (!popupList.Contains(popup))
            return;

        popupList.Remove(popup);

        StartCoroutine(CloseAnimation(popup, () =>
        {

            onClosed?.Invoke();

            Addressables.ReleaseInstance(popup.gameObject);

            if (popupList.Count != 0)
            {
                bool wasActive = popupList[0].content.activeSelf;

                popupList[0].content.SetActive(true);

                if (!wasActive)
                    StartCoroutine(OpenAnimation(popupList[0]));
            }
        }));

    }

    
    public void CloseAllPopup()
    {
        
        for (int i = 0; i < popupList.Count; i++)
        {
            Popup popup = popupList[i];

            if (popup != null)
            {
                Addressables.ReleaseInstance(popup.gameObject);
            }
            popupList.RemoveAt(i);
        }

    }
    #endregion // Popup 

    #region ---- Animations ----

    private IEnumerator OpenAnimation(Overlay overlay, Action onComplete = null)
    {
        OverlayAnimationData animationData = overlay.openAnimationData;
        if (!animationData.useAnimation)
        {
            onComplete?.Invoke();
            yield break;
        }

        if (animationData.elements != null)
        {
            for (int i = 0; i < animationData.elements.Length; i++)
            {
                animationData.elements[i].transform.localScale = Vector3.zero;
            }

            for (int i = 0; i < animationData.elements.Length; i++)
            {
                LeanTween.scale(animationData.elements[i], Vector3.one,animationData.durations[Mathf.Min(i, animationData.durations.Length - 1)]).setEase(animationData.easeTypes[Mathf.Min(i, animationData.easeTypes.Length - 1)]);
                yield return new WaitForSeconds(animationData.delay);
            }
        }

        onComplete?.Invoke();
    }

    private IEnumerator CloseAnimation(Popup popup, Action onComplete)
    {

        OverlayAnimationData animationData = popup.closeAnimationData;

        if (animationData.useAnimation)
        {
            float additionalDelay = 0;

            if (animationData.elements != null)
                for (int i = 0; i < animationData.elements.Length; i++)
                {
                    LeanTween.scale(animationData.elements[i], Vector3.zero,
                            animationData.durations[Mathf.Min(i, animationData.durations.Length - 1)])
                        .setEase(animationData.easeTypes[Mathf.Min(i, animationData.easeTypes.Length - 1)]);

                    additionalDelay += animationData.durations[Mathf.Min(i, animationData.durations.Length - 1)];

                    yield return new WaitForSeconds(animationData.delay);
                }

            yield return new WaitForSeconds(additionalDelay -
                                            (animationData.delay * animationData.elements.Length));
        }

        onComplete?.Invoke();
    }



    #endregion

    private void Update()
    {

        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (popupList.Count > 0)
            {
                if (popupList[0].closeWithBackButton)
                    popupList[0].Close();
            }
            else
            {
                Panel panel = panelList[0];

                if (panelList.Count > 1)
                {
                    if (panel.closeWithBackButton)
                    {
                        panelList.RemoveAt(0);
                        Addressables.ReleaseInstance(panel.gameObject);
                    }

                }
                else
                {
                    // if (panel is Panel_Main)
                    // {
                    //     // open popup quit game
                    // }
                    // else
                    // {
                        if (panel.closeWithBackButton)
                        {
                            panelList.RemoveAt(0);
                            Addressables.ReleaseInstance(panel.gameObject);
                            
                           // OpenPanel<Panel_Main>(false).Setup();
                        }
                   // }

                }
            }
        }
    }

    #endregion
    

    
}
public enum ClientState
{
    None,
    Online,
    Offline
}