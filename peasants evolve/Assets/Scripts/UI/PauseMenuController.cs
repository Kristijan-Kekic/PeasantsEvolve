using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public BuildingMenuController buildingMenuController;
    public BuildingPlacement buildingPlacement;
    private bool isPaused = false;
    public string _newGameLevel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (buildingPlacement.IsPlacingBuilding && buildingPlacement != null)
            {
                return;
            }

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame(); 
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0; 
        pauseMenuUI.SetActive(true);
        buildingMenuController.SetPauseMenuActive(true);
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1; 
        pauseMenuUI.SetActive(false);
        buildingMenuController.SetPauseMenuActive(false);
    }

    public void Exit()
    {
        ResumeGame();
        SceneManager.LoadScene(_newGameLevel);
    }

}