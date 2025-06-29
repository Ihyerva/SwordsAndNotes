using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvent", order = 0)]
public class GameEvent : ScriptableObject
{

    [SerializeField]
    private List<GameEventListener> Listeners = new List<GameEventListener>();


    public void Raise(Component sender, object data)
    {
        for (int i = 0; i < Listeners.Count; i++)
        {
            Listeners[i].OnRaise(this, sender, data);
        }
    }

    public void registerListener(GameEventListener Listener)
    {
        if (!Listeners.Contains(Listener))
            Listeners.Add(Listener);
    }

    public void unregisterListener(GameEventListener Listener)
    {
        if (Listeners.Contains(Listener))
            Listeners.Remove(Listener);
    }
}