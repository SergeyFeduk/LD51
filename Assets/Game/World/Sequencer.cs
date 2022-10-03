using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
class SequenceElement
{
    public string name;
    public float timeStamp;
    public UnityEvent action;
    [HideInInspector] public bool isUsed = false;
}

public class Sequencer : MonoBehaviour
{
    [SerializeField] private List<SequenceElement> sequence = new List<SequenceElement>();
    public UnityEvent actionAt10;
    private List<SequenceElement> runtimeSequence = new List<SequenceElement>();

    Timer timer = new Timer(10);

    private void Start()
    {
        sequence = sequence.OrderBy(x => x.timeStamp).ToList();
        runtimeSequence = new List<SequenceElement>(sequence);
    }

    private void Update()
    {
        if (timer.ExecuteTimer()) {
            timer.SetTimer(10);
            ResetSequence();
            actionAt10.Invoke();
        }
        if (runtimeSequence.Count == 0) return;
        if (runtimeSequence[0].timeStamp < 10 - timer.GetTimeLeft()) {
            runtimeSequence[0].action.Invoke();
            runtimeSequence.RemoveAt(0);
        }
    }

    public void ResetSequence() {
        runtimeSequence = new List<SequenceElement>(sequence);
    }
}
