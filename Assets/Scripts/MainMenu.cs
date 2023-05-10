using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GameController;

public class MainMenu : MonoBehaviour
{
    public static bool mainMenu = true;
    public static GameMode selectedGameMode;

    public GameObject gameOver;
    private Button playButton1;
    private Button playButton2;
    private Button quitButton;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (mainMenu)
        {
            this.gameObject.SetActive(true);
            gameOver.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(false);
            gameOver.SetActive(true);
        }
    }

    void Start()
    {
        playButton1 = transform.GetChild(1).GetComponent<Button>();
        playButton2 = transform.GetChild(2).GetComponent<Button>();
        quitButton = transform.GetChild(3).GetComponent<Button>();
        playButton1.onClick.AddListener(() => PlayGame(GameMode.Mode1));
        playButton2.onClick.AddListener(() => PlayGame(GameMode.Mode2));
        quitButton.onClick.AddListener(QuitGame);
    }

    public void PlayGame(GameMode gameMode)
    {
        mainMenu = false;
        selectedGameMode = gameMode;
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
