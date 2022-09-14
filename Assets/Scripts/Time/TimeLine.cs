using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLine : MonoBehaviour
{
    public delegate void TimeTickAction(float fixedDeltaTime);
    public static event TimeTickAction timeTickEvent;
    private void FixedUpdate()
    {
        timeTickEvent?.Invoke(Time.fixedDeltaTime);
    }
}
