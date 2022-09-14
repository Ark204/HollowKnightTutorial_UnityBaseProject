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
        if(!circleQueue.IsEmpty()&&circleQueue.RearItem().triggerTime==Time.fixedTime)//��ͬһʱ���Ĳ�ͬ��Ϣ
        {
            circleQueue.RearItem().infoList.Add(new KeyValuePair<TimeInfo2.InfoType, object>(infoType, info));
        }
        else//����µ�ʱ��ڵ�
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
    #region Э�̸�������
    /// <summary>
    /// Э�̺����������ӳٵ���0��������
    /// </summary>
    /// <param name="callBack">���ӳٵ��õ�0��������</param>
    /// <param name="time">�ӳ�ʱ��,Ĭ��Ϊ0</param>
    /// <returns></returns>
    public static IEnumerator DelayFunc(Action callBack, float time = 0)
    {
        yield return new WaitForSecondsRealtime(time);
        callBack?.Invoke();
    }
    /// <summary>
    /// Э�̺����������ӳٵ���1��������
    /// </summary>
    /// <typeparam name="T">���ú����Ĳ�������</typeparam>
    /// <param name="callBack">���ӳٵ��õ�1��������</param>
    /// <param name="arg1">���ú����ľ������1</param>
    /// <param name="time">�ӳ�ʱ��,Ĭ��Ϊ0</param>
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
