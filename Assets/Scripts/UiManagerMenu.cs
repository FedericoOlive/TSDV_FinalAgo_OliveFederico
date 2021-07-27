using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UiManagerMenu : MonoBehaviour
{
    public CanvasGroup mainMenuCanvasGroup;
    public CanvasGroup creditsCanvasGroup;
    [SerializeField] private float timeTransition = 1;

    private float onTime;

    private void Start()
    {
        onTime = 0;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SwitchPanelMenuToCredits()
    {
        MenuCheckGameObjectsReference();
        mainMenuCanvasGroup.blocksRaycasts = false;
        mainMenuCanvasGroup.interactable = false;
        StartCoroutine(SwitchPanel(timeTransition, creditsCanvasGroup, mainMenuCanvasGroup));
    }

    public void SwitchPanelCreditsToMenu()
    {
        MenuCheckGameObjectsReference();
        creditsCanvasGroup.blocksRaycasts = false;
        creditsCanvasGroup.interactable = false;
        StartCoroutine(SwitchPanel(timeTransition, mainMenuCanvasGroup, creditsCanvasGroup));
    }

    void MenuCheckGameObjectsReference()
    {
        if (!mainMenuCanvasGroup)
        {
            mainMenuCanvasGroup = GameObject.Find("PanelMainMenu").GetComponent<CanvasGroup>(); // Sobrechequeo.
            Debug.LogWarning("mainMenuCanvasGroup no estaba asignado", gameObject);
        }

        if (!creditsCanvasGroup)
        {
            creditsCanvasGroup = GameObject.Find("PanelCredits").GetComponent<CanvasGroup>(); // Sobrechequeo.
            Debug.LogWarning("creditsCanvasGroup no estaba asignado", gameObject);
        }
    }

    IEnumerator SwitchPanel(float maxTime, CanvasGroup on, CanvasGroup off)
    {
        while (onTime < maxTime)
        {
            onTime += Time.deltaTime;
            on.alpha = onTime / maxTime;
            off.alpha = 1 - onTime / maxTime;
            yield return null;
        }

        on.blocksRaycasts = true;
        on.interactable = true;
        onTime = 0;
    }
}