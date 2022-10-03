using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void Play()
    {
        SceneManagerScript.instance.ChangeScene("Game");
    }

    public void Exit() {
        Application.Quit();
    }
}
