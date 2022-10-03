using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounter : MonoBehaviour
{
    private int robotsKilled = 0;
    private float timeSurvived;

    public void AddKill() {
        robotsKilled++;
    }

    public int GetKills() { return robotsKilled; }

    private void Update()
    {
        timeSurvived = Time.timeSinceLevelLoad;
    }
}
