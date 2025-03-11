using Luzart;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeSequenceAction : MonoBehaviour
{
    public SequenceActionEvent[] sequenceActionEvents;

    private void Start()
    {
        int length = sequenceActionEvents.Length;
        for (int i = 0; i < length; i++)
        {
            sequenceActionEvents[i]?.PreInit();
        }
        List<Action<Action>> listStep = new List<Action<Action>>();
        for (int i = 0; i < length; i++)
        {
            int index = i;
            var instance = sequenceActionEvents[index];
            listStep.Add(next => instance?.Init(next));
        }
        GameUtil.Instance.WaitAFrame(() =>
        {
            GameUtil.StepToStep(listStep.ToArray());
        });
    }
}
