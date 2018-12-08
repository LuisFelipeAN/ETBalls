using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Subject
{
    private List<IObserver> observers;
    public Subject()
    {
        observers = new List<IObserver>();
    }

    public void addObserver(IObserver o)
    {
        observers.Add(o);
    }
    public void removeObserver(IObserver o)
    {
        observers.Remove(o);
    }

    public void removeAllObservers()
    {
        observers.Clear();
    }
    protected void notifyObservers()
    {
        foreach(IObserver o in observers)
        {
            o.notify();
        }
    }
}

