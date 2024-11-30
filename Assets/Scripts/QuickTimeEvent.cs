using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuickTimeEvent : MonoBehaviour
{
    public GameObject slowMotionOBJ;
    public GameObject qteCanvas;
    public GameObject boxVolume;

    [SerializeField]
    private GameObject qte_Failed_Object; // Timelineobjekt, dass die Death Canvas einbelndet

    public Image keyImage;  
    public Sprite keySprite; // UI Bild der Taste

    public float timeLimit = 2f;  // Zeitlimit in Sekunden
    public PlayableDirector timeline;  // Die Timeline für das QTE

    public InputAction currentInputAction;  // Die dynamisch gesetzte Input Action
    
    private bool isEventActive = false;
    public bool qte_Failed = true;
    public float timer = 0f;

    public FillImage fillImage;

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

   

    void Update()
    {
        if (isEventActive)
        {
            timer += Time.deltaTime;

            if (timer >= timeLimit)
            {
                Fail();
            }
        }
    }

    public void StartQuickTimeEvent()
    {
        isEventActive = true;
        timer = 0f;
        keyImage.sprite = keySprite;
        
        slowMotionOBJ.GetComponent<SlowMotion>().StartSlowMotion();
        qteCanvas.SetActive(true);
        boxVolume.SetActive(true);

        
    }

    private void OnKeyPressed(InputAction.CallbackContext context)
    {
        if (isEventActive)
        {
            Success();
        }
    }

    private void Success()
    {
        timeline.Play();
        isEventActive = false;
        qteCanvas.SetActive(false);
        boxVolume.SetActive(false);

        slowMotionOBJ.GetComponent<SlowMotion>().EndSlowMotion();

        Debug.Log("Quick Time Event bestanden!");

        timer = 0f;

        fillImage.timeRemaining = 2f;
        fillImage.fillImage.fillAmount = 0;

        qte_Failed = false;
    }

    private void Fail()
    {
        isEventActive = false;
        qteCanvas.SetActive(false);
        boxVolume.SetActive(false);

        slowMotionOBJ.GetComponent<SlowMotion>().EndSlowMotion();

        Debug.Log("Quick Time Event nicht bestanden!");
        
        
        timer = 0f;

        fillImage.timeRemaining = 2f;
        fillImage.fillImage.fillAmount = 0;

        qte_Failed = true;
        QuickTimeEventFailed();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartQuickTimeEvent();  


        }
    }

    public void QuickTimeEventFailed()
    {
        qte_Failed_Object.SetActive(true);
    }
}
