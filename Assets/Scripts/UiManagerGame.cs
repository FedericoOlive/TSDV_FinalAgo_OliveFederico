using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManagerGame : MonoBehaviour
{
    public Player player;
    public CanvasGroup[] canvasGroup;
    private Color GameOverLoseColor = new Color(200, 60, 60, 170);
    private Color GameOverWinColor = new Color(60, 200, 60, 170);
    enum CanvasGroups{ Game, Pause, GameOver }
    private CanvasGroups actualCanvasGroups = CanvasGroups.Game;    // off
    private CanvasGroups nextCanvasGroups = CanvasGroups.Game;      // on
    private float timeTransition = 1f;
    private float onTime;
    void Start()
    {
        foreach (CanvasGroup canvas in canvasGroup)
        {
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
            canvas.alpha = 0;
        }
        canvasGroup[(int) CanvasGroups.Game].interactable = true;
        canvasGroup[(int) CanvasGroups.Game].blocksRaycasts = true;
        canvasGroup[(int)CanvasGroups.Game].alpha = 1;

    }
    void Update()
    {
        
    }
    public void Pause(bool on)
    {
        Time.timeScale = on ? 0 : 1;
        SwitchPanel(on ? 1 : 0);
    }
    void SwitchPanel(int panelOn)
    {
        nextCanvasGroups = (CanvasGroups) panelOn;
        canvasGroup[(int) actualCanvasGroups].blocksRaycasts = false;
        canvasGroup[(int) actualCanvasGroups].interactable = false;
        StartCoroutine(SwitchPanel(timeTransition, canvasGroup[(int) nextCanvasGroups], canvasGroup[(int) actualCanvasGroups]));
    }
    IEnumerator SwitchPanel(float maxTime, CanvasGroup on, CanvasGroup off)
    {
        while (onTime < maxTime)
        {
            onTime += Time.unscaledDeltaTime;
            on.alpha = onTime / maxTime;
            off.alpha = 1 - onTime / maxTime;
            yield return null;
        }

        actualCanvasGroups = nextCanvasGroups;
        on.blocksRaycasts = true;
        on.interactable = true;
        onTime = 0;
    }

    public void GoToScene(string scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }
}