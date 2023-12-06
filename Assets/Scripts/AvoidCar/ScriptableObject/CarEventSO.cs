using UnityEngine;
using UnityEngine.Events;

namespace AvoidCar.Common
{
    [CreateAssetMenu(menuName = "Event/CarEventSO")]
    public class CarEventSO : ScriptableObject
    {
        public UnityAction OnEventRaised;

        public void Raise()
        {
            OnEventRaised?.Invoke();
        }
    }
}
