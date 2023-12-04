using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Serialization<T>
{
    [SerializeField]
    private List<T> items;

    public List<T> ToList()
    {
        return items;
    }

    public Serialization(List<T> items)
    {
        this.items = items;
    }
}
