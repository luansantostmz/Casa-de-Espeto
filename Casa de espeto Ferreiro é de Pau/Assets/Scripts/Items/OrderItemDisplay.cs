using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderItemDisplay : ItemDisplay
{
    public GameObject CompletedGameObject;

    public bool IsCompleted { get; private set; }

    public void SetComplete()
    {
        IsCompleted = true;
        CompletedGameObject.SetActive(true);
    }
}
