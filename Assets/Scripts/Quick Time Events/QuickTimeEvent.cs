using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuickTimeEvent : MonoBehaviour
{
    private GameObject slowMotionOBJ;

    [SerializeField]
    private Image tasteImage;
    [SerializeField]
    private GameObject qteCanvas;
    [SerializeField]
    private GameObject qte_Failed_Object; // Timelineobjekt, dass die Death Canvas einbelndet

    private bool isEventActive = false;
    private bool qte_Failed;
    private float timer = 0f;

    public Sprite keySprite; // UI Bild der Taste
    public float timeLimit = 2f;  // Zeitlimit in Sekunden
    public PlayableDirector timeline;  // Die Timeline für das QTE

    public InputAction currentInputAction;  // Die dynamisch gesetzte Input Action
    
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

    public void Start()
    {
        slowMotionOBJ = GameObject.Find("Slowmotion");
        
    }




    void Update()
    {
        if (isEventActive)
        {
            //Timer des QTEs
            timer += Time.deltaTime;

            if (timer >= timeLimit)
            {
                Fail();
            }
        }
    }

    public void StartQuickTimeEvent()
    {
        //Set Values
        isEventActive = true;
        timer = 0f;
        tasteImage.sprite = keySprite;

        slowMotionOBJ.GetComponent<SlowMotion>().StartSlowMotion();

        qteCanvas.SetActive(true);
        
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
        

        isEventActive = false;
        qteCanvas.SetActive(false);
        

        slowMotionOBJ.GetComponent<SlowMotion>().EndSlowMotion();

        Debug.Log("Quick Time Event bestanden!");

        //Reset Values
        timer = 0f;
        fillImage.timeRemaining = 2f;
        fillImage.fillImage.fillAmount = 0;
        qte_Failed = false;
    }

    private void Fail()
    {
        isEventActive = false;
        qteCanvas.SetActive(false);
        

        slowMotionOBJ.GetComponent<SlowMotion>().EndSlowMotion();

        Debug.Log("Quick Time Event nicht bestanden!");
        
        //Reset Values
        timer = 0f;
        fillImage.timeRemaining = 2f;
        fillImage.fillImage.fillAmount = 0;
        qte_Failed = true;

        //Aktivieren des Fail Objektes (Timeline + Death Screen)
        qte_Failed_Object.SetActive(true);
    }

}
