using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollected : MonoBehaviour
{
    public float secondsToWait;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("UI_ItemCollected_FadeOut");

        StartCoroutine(WaitForSeconds());
    }

    



    IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(secondsToWait);

        gameObject.SetActive(false);    


    }
}
