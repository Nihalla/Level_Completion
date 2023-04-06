using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Buttons : MonoBehaviour
{
    [SerializeField] GameObject win_cond_screen;
    [SerializeField] GameObject canvas;
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void OnTriggerEnter(Collider other)
    {
        win_cond_screen.SetActive(true);
        canvas.SetActive(false);
    }
}
