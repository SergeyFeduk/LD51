using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenuButtons : MonoBehaviour
{
    public void GoMenu()
    {
        SceneManagerScript.instance.ChangeScene("Menu");
    }
    public void Restart()
    {
        SceneManagerScript.instance.ChangeScene("Game");
    }
}
