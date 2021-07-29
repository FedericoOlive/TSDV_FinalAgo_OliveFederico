using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.UI;

public class UiManagerGame : MonoBehaviour
{
    [Serializable] public class ObjectsUI
    {
        public TextMeshProUGUI score;
        public TextMeshProUGUI timer;
        public TextMeshProUGUI distance;
        public Image bulletRecharge;
        public Slider fuel;
        public Image[] bullets;
        public Image clockFull;
    }
    [SerializeField] private ObjectsUI ui;

    [Serializable]
    public class ObjectsUIGameOver
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI life;
        public TextMeshProUGUI score;
        public TextMeshProUGUI distance;
        public TextMeshProUGUI bulletsShooted;
        public TextMeshProUGUI boxesDestroyed;
        public TextMeshProUGUI barrelsDestroyed;
    }
    [SerializeField] private ObjectsUIGameOver uiGameOver;

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
        GameManager.Get().updateScore += UpdateScore;
        foreach (CanvasGroup canvas in canvasGroup)
        {
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
            canvas.alpha = 0;
        }
        canvasGroup[(int) CanvasGroups.Game].interactable = true;
        canvasGroup[(int) CanvasGroups.Game].blocksRaycasts = true;
        canvasGroup[(int)CanvasGroups.Game].alpha = 1;
        player.onShooted += ShootedBullet;
        GameManager.Get().onGameOver += UpdatePanelGameOver;
    }
    void Update()
    {
        UpdateTime();
        UpdateFuel();
        UpdateDistance();
    }
    public void UpdateScore()
    {
        ui.score.text = GameManager.Get().score.ToString();
    }
    public void UpdateTime()
    {
        float maxTime = GameManager.Get().maxGameTime;
        float onTime = GameManager.Get().gameTime;
        ui.timer.text = onTime.ToString("F2");
        ui.clockFull.fillAmount = onTime / maxTime;
        //Debug.Log(onTime / maxTime);
    }
    public void UpdateFuel()
    {   // Se llama en el update porque llamar a un evento frame por frame mientras se mantenga apretada las teclas WASD me parece demasiado.
        ui.fuel.value = player.GetFuel() / player.GetMaxFuel();
        //Debug.Log(player.GetFuel() / player.GetMaxFuel());
    }
    public void UpdateDistance()
    {
        ui.distance.text = player.distance.ToString("F0");
    }
    public void ShootedBullet()
    {
        int bullets = player.GetBullets();

        for (int i = 0; i < ui.bullets.Length; i++)
        {
            ui.bullets[i].gameObject.SetActive(i < bullets);
        }

        StartCoroutine(Reloading());
    }
    IEnumerator Reloading()
    {
        ui.bulletRecharge.fillAmount = 0;
        while (ui.bulletRecharge.fillAmount < 0.95f)
        {
            //Debug.Log("Recargando: " + ui.bulletRecharge.fillAmount);
            ui.bulletRecharge.fillAmount = player.GetRateFireTime() / player.rateFire;
            yield return null;
        }

        ui.bulletRecharge.fillAmount = 1;
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
    public void UpdatePanelGameOver()
    {
        SwitchPanel((int) CanvasGroups.GameOver);
        float life = player.life;
        uiGameOver.title.text = life > 0 ? "Time Over" : "Game Over";
        uiGameOver.life.text = "Life: " + life.ToString("F0");
        uiGameOver.score.text = "Score: " + GameManager.Get().score.ToString();
        uiGameOver.distance.text = "Distance: " + player.distance.ToString("F0");
        uiGameOver.bulletsShooted.text = "Bullets Shooted: " + player.bulletsShooted.ToString();
        uiGameOver.boxesDestroyed.text = "Boxes Destroyed: " + GameManager.Get().boxesDestroyed.ToString();
        uiGameOver.barrelsDestroyed.text = "Barrels Destroyed: " + GameManager.Get().barrelsDestroyed.ToString();
    }
    public void GoToScene(string scene)
    {
        Destroy(GameManager.Get().gameObject);
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }
}