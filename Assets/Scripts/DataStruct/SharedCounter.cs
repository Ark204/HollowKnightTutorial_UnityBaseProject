using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SharedCounter<T> //where T: class
{
    public delegate X Return<X>();
    public delegate void Action<X>(ref X x);
    [SerializeField] int count = 0;
    [SerializeField] T m_ref;
    Return<T> onFirstRef;
    Action<T> onLastRef;
    public T GetRef() { return m_ref; }
    public void Increase()
    {
        if (count == 0) m_ref = onFirstRef();
        count++;
    }
    public void Decrease()
    {
        if (--count == 0) onLastRef(ref m_ref);//;m_ref = null;
    }
    public SharedCounter(T def,Return<T> first,Action<T> last)
    {
        onFirstRef = first;
        onLastRef = last;
        m_ref = def;
    }
}