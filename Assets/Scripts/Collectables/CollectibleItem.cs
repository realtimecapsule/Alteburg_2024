using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class CollectibleItem : MonoBehaviour
{
    public int itemIndex; // Index des Gegenstands im UI-Array
    
    private bool canCollect;

    public InputAction currentInputAction;

    public Canvas collectCanvas;

    private void OnEnable()
    {
        if (currentInputAction != null)
        {
            // Event verbinden, wenn die Input Action gesetzt ist
            currentInputAction.performed += OnKeyPressed;
            currentInputAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (currentInputAction != null)
        {
            // Event abmelden
            currentInputAction.performed -= OnKeyPressed;
            currentInputAction.Disable();
        }
    }

    private void OnKeyPressed(InputAction.CallbackContext context)
    {
      if( canCollect )
        {
            CollectItem();
        }  
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCollect = true;
            collectCanvas.transform.GetChild(0).gameObject.SetActive(true); //Child(0) ist das Image für die Taste, die abgebildet wird, damit man es einsammeln kann

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCollect = false;
            collectCanvas.transform.GetChild(0).gameObject.SetActive(false);

        }
    }

    public void CollectItem()
    {
        if (canCollect)
        {
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            // Gegenstand im UI freischalten
            inventoryManager.UnlockItem(itemIndex);

            // Optional: Nachricht ausgeben
            Debug.Log($"Gegenstand {itemIndex} eingesammelt!");

            
            //Aktivert und deaktiviert die verschiedenen Canvas
            collectCanvas.transform.GetChild(0).gameObject.SetActive(false);
            collectCanvas.transform.GetChild(1).gameObject.SetActive(true); //Child(1) ist das Objekt, dass den Hinweis zeigt, dass und welches Objekt man eingesammelt hat (Mit Animation zum ausfaden)

            //Image des gefundenden Items setzen
            GameObject itemCollectedCanvasIcon = collectCanvas.transform.GetChild(1).gameObject;
            itemCollectedCanvasIcon.transform.GetChild(1).GetComponent<Image>().sprite = inventoryManager.itemSlots[itemIndex].sprite;

            // Item deaktivieren
            gameObject.SetActive(false);
        }
    }

    
}

