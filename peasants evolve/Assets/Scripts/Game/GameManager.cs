using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject victoryScreen;
    public GameObject defeatScreen;

    private SelectionManager selectionManager;

    void Start()
    {
        selectionManager = SelectionManager.Instance;
    }

    void Update()
    {
        Time.timeScale = 5f;
        CheckVictoryOrDefeat();
    }

    void CheckVictoryOrDefeat()
    {
        if (selectionManager.playerUnits.Count == 0 && selectionManager.playerBuildings.Count == 0)
        {
            ShowDefeatScreen();
        }
        else if (selectionManager.enemyUnits.Count == 0 && selectionManager.enemyBuildings.Count == 0)
        {
            ShowVictoryScreen();
        }
    }

    void ShowVictoryScreen()
    {
        victoryScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    void ShowDefeatScreen()
    {
        defeatScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
