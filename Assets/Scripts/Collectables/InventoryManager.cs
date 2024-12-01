using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Image[] itemSlots; // Array f�r die UI-Elemente

    // Freischalten eines Gegenstands
    public void UnlockItem(int index)
    {
        if (index >= 0 && index < itemSlots.Length)
        {
            // �ndere die Farbe auf Wei� (freigeschaltet)
            itemSlots[index].color = Color.white;
        }
        else
        {
            Debug.LogWarning("Index au�erhalb des g�ltigen Bereichs: " + index);
        }
    }
}
