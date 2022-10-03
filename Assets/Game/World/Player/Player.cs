using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }

    [Header("Imports")]
    public PlayerController controller;
    public PlayerHealth heath;
    public PlayerCounter counter;
    public PlayerAnimation playerAnimator;

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
}
