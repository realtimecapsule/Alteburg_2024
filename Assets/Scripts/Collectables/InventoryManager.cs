using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Image[] itemSlots; // Array für die UI-Elemente

    // Freischalten eines Gegenstands
    public void UnlockItem(int index)
    {
        if (index >= 0 && index < itemSlots.Length)
        {
            // Ändere die Farbe auf Weiß (freigeschaltet)
            itemSlots[index].color = Color.white;
        }
        else
        {
            Debug.LogWarning("Index außerhalb des gültigen Bereichs: " + index);
        }
    }
}
