using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/GameEventSO")]
public class GameEventSO : ScriptableObject
{
    public UnityAction OnEventRaised;

    public void Raise()
    {
        OnEventRaised?.Invoke();
    }
}
