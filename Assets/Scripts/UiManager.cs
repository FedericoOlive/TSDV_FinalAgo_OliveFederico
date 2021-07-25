using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public GameObject blockRaycast;
    public CanvasGroup mainMenuCanvasGroup;
    public CanvasGroup creditsCanvasGroup;
    public CanvasGroup gameCanvasGroup;
    public CanvasGroup pauseCanvasGroup;
    [SerializeField] private float timeTransitionMenu;
    [SerializeField] private float timeTransitionGame;

    private float onTime;
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
        mainMenuCanvasGroup.blocksRaycasts = false;
        creditsCanvasGroup.blocksRaycasts = false;
        StartCoroutine(SwitchPanel(timeTransitionMenu, creditsCanvasGroup, mainMenuCanvasGroup));
    }
    public void SwitchPanelCreditsToMenu()
    {
        
    }
    public void SwitchPanelGameToPause()
    {

    }
    public void SwitchPanelPauseToGame()
    {

    }

    IEnumerator SwitchPanel(float maxTime, CanvasGroup on, CanvasGroup off)
    {
        maxTime += Time.deltaTime;
        on.alpha = maxTime;
        yield return null;
        
        on.blocksRaycasts = true;
        onTime = 0;
    }
}