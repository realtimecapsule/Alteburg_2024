using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
            collectCanvas.gameObject.SetActive(true);
            
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCollect = false;
            collectCanvas.gameObject.SetActive(false);

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

            // Gegenstand aus der Welt entfernen
            gameObject.SetActive(false);
            collectCanvas.gameObject.SetActive(false);
        }
    }
}

