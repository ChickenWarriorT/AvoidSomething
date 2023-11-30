using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CarEventSO")]
public class CarEventSO : ScriptableObject
{
    public UnityAction OnEventRaised;

    public void Raise()
    {
        OnEventRaised?.Invoke();
    }
}
