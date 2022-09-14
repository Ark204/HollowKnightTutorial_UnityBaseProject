using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public delegate T Return<T>();
//Util
public static class TQueueExtion 
{
    public static CircleQueue<TimeInfo2> CircleQueue(float timeLength)
    {
        int size = (int)(timeLength / Time.fixedDeltaTime);
        return new CircleQueue<TimeInfo2>(size);
    }
    public static void Push(this CircleQueue<TimeInfo2> circleQueue,TimeInfo2.InfoType infoType,object info)
    {
        if(!circleQueue.IsEmpty()&&circleQueue.RearItem().triggerTime==Time.fixedTime)//在同一时间点的不同信息
        {
            circleQueue.RearItem().infoList.Add(new KeyValuePair<TimeInfo2.InfoType, object>(infoType, info));
        }
        else//添加新的时间节点
        {
            circleQueue.EnQueue(new TimeInfo2(infoType, info));
        }
    }
    public static Stack<TimeInfo2> ToStack(this CircleQueue<TimeInfo2> queue)
    {
        Stack<TimeInfo2> stack = new Stack<TimeInfo2>();
        while(!queue.IsEmpty())
        {
            stack.Push(queue.DeQueue());
        }
        return stack;
    }
    #region 协程辅助函数
    /// <summary>
    /// 协程函数，用于延迟调用0参数函数
    /// </summary>
    /// <param name="callBack">将延迟调用的0参数函数</param>
    /// <param name="time">延迟时间,默认为0</param>
    /// <returns></returns>
    public static IEnumerator DelayFunc(Action callBack, float time = 0)
    {
        yield return new WaitForSecondsRealtime(time);
        callBack?.Invoke();
    }
    /// <summary>
    /// 协程函数，用于延迟调用1参数函数
    /// </summary>
    /// <typeparam name="T">调用函数的参数类型</typeparam>
    /// <param name="callBack">将延迟调用的1参数函数</param>
    /// <param name="arg1">调用函数的具体参数1</param>
    /// <param name="time">延迟时间,默认为0</param>
    /// <returns></returns>
    public static IEnumerator DelayFunc<T>(Action<T> callBack, T arg1, float time = 0)
    {
        yield return new WaitForSecondsRealtime(time);
        callBack?.Invoke(arg1);
    }
    public static IEnumerator DelayFunc<T, X>(Action<T, X> callBack, T arg1, X arg2, float time = 0)
    {
        yield return new WaitForSecondsRealtime(time);
        callBack?.Invoke(arg1, arg2);
    }
    #endregion
}
public struct TimeInfo2
{
    public enum InfoType
    {
        Position,
        Localscale,
        AnimatorState,
        AttackPoint,
        Particle,
        Prefab
    }
    public TimeInfo2(InfoType infoType,object info)
    {
        triggerTime = Time.fixedTime;
        infoList = new List<KeyValuePair<InfoType,object>>();
        infoList.Add(new KeyValuePair<InfoType,object>(infoType, info));
    }
    public float triggerTime;
    public List<KeyValuePair<InfoType, object>> infoList;
}
