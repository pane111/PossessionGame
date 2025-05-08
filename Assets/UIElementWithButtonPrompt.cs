using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElementWithButtonPrompt : MonoBehaviour
{
    public string buttonToPress; //Name of button or axis
    public float axisVal;
    public bool aboveAxisVal; //Trigger when axis is ABOVE axisVal, otherwise trigger when axis is BELOW
   
    public bool isButtonOrToggle = true; //True = button false = toggle
    public bool isAxis = false;
    bool canTrigger = true;
    public float initDelay;
    private void Start()
    {
        DisableThis();
        StartCoroutine(EnableDelay(initDelay));
    }
    void Update()
    {
        if (!isAxis && Input.GetButtonDown(buttonToPress))
        {
            if (canTrigger)
                Trigger();
        }
        if (isAxis)
        {
            if (aboveAxisVal)
            {
                if (Input.GetAxisRaw(buttonToPress) > axisVal)
                {
                    if (canTrigger)
                        Trigger();
                }
            }
            else
            {
                if (Input.GetAxisRaw(buttonToPress) < axisVal)
                {
                    if (canTrigger)
                        Trigger();
                }
            }
        }
    }

    IEnumerator EnableDelay(float delay)
    {
        
        yield return new WaitForSecondsRealtime(delay);
        EnableThis();
        yield return null;
    }

    void DisableThis()
    {
        if (isButtonOrToggle)
        {
            GetComponent<Button>().interactable = false;
        }
        canTrigger = false;
    }
    void EnableThis()
    {
        if (isButtonOrToggle)
        {
            GetComponent<Button>().interactable = true;
        }
        canTrigger = true;
    }
    private void OnEnable()
    {
        DisableThis();
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(EnableDelay(initDelay));
        }
    }

    private void Trigger()
    {
        if (isButtonOrToggle)
        {
            GetComponent<Button>().onClick.Invoke();
        }
        else
        {
            GetComponent<Toggle>().isOn = !GetComponent<Toggle>().isOn;
        }
        DisableThis();
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(EnableDelay(0.15f));
        }
        else
        {
            EnableThis();
        }
        
    }
}
