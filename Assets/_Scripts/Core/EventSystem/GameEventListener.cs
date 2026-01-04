using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }
public class GameEventListener : MonoBehaviour
{

    [SerializeField]
    private List<GameEvent> events;
    [SerializeField]
    private List<CustomGameEvent> responses;

    private void OnEnable()
    {
        for (int i = 0; i < events.Count; i++)
            events[i].registerListener(this);
    }

    private void OnDisable()
    {
        for (int i = 0; i < events.Count; i++)
            events[i].unregisterListener(this);
    }

    public void OnRaise(GameEvent triggeredEvent, Component sender, object data)
    {
        responses[events.IndexOf(triggeredEvent)].Invoke(sender, data);
    }
}