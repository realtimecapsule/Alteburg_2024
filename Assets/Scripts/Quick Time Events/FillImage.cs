using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillImage : MonoBehaviour
{
    public Image fillImage; // Das UI-Image, das als Indikator dient
    public float timerDuration = 2f; // Dauer des Timers in Sekunden
    public float timeRemaining;

    

    private void Start()
    {
        // Timer zur�cksetzen und starten
        timeRemaining = timerDuration;
    }

    private void Update()
    {
        // Timer herunterz�hlen, wenn noch Zeit �brig ist
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateFillAmount();
        }
        else
        {
            timeRemaining = 0; // Sicherstellen, dass der Timer bei Null stoppt
            fillImage.fillAmount = 0; // Anzeige auf 0 setzen

        }
    }

    private void UpdateFillAmount()
    {
        // Berechnung des F�llstandes basierend auf der verbleibenden Zeit
        float fillValue = 1 - (timeRemaining / timerDuration);
        fillImage.fillAmount = fillValue;
    }
}
