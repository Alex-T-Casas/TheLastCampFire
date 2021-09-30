using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SignScript : Interactable
{
    [SerializeField] Image DialogBG;
    [SerializeField] Text DialogText;
    [SerializeField] float TransitionSpeed = 1f;
    [SerializeField] string[] dialogs;
    int currentDialogIndex = 0;
    Color DialogTextColor;
    Color DialogBGColor;
    float Opacity;
    Coroutine TransitionCoroutine;
    // Start is called before the first frame update

    void GoToNextDialog()
    {
        if(dialogs.Length==0)
        {
            return;
        }
        currentDialogIndex = (currentDialogIndex + 1) % dialogs.Length;
        DialogText.text = dialogs[currentDialogIndex];
    }
    void Start()
    {
        DialogTextColor = DialogText.color;
        DialogBGColor = DialogBG.color;
        setOpacity(0);
        if(dialogs.Length!= 0)
        {
            DialogText.text = dialogs[0];
        }else
        {
            DialogText.text = "";
        }

    }

    void setOpacity(float opacity, float TransitionTime =0)
    {
        opacity = Mathf.Clamp(opacity, 0, 1);
        Color ColorMult = new Color(1f, 1f, 1f, opacity);
        DialogText.color = DialogTextColor * ColorMult;
        DialogBG.color = DialogBGColor * ColorMult;
        Opacity = opacity;
    }


    IEnumerator TransitionOpacityTo(float newOpacity)
    {
        float Dir = newOpacity - Opacity > 0 ? 1 : -1;
        while(Opacity!=newOpacity)
        {
            setOpacity(Opacity + Dir * TransitionSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        setOpacity(newOpacity);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        InteractComponent interactableComp = other.GetComponent<InteractComponent>();
        if(interactableComp!=null)
        {
            if(TransitionCoroutine!=null)
            {
                StopCoroutine(TransitionCoroutine);
                TransitionCoroutine = null;
            }
            TransitionCoroutine = StartCoroutine(TransitionOpacityTo(0));
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        InteractComponent interactableComp = other.GetComponent<InteractComponent>();
        if (interactableComp != null)
        {
            if (TransitionCoroutine != null)
            {
                StopCoroutine(TransitionCoroutine);
                TransitionCoroutine = null;
            }
            TransitionCoroutine = StartCoroutine(TransitionOpacityTo(0));
            currentDialogIndex = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        StartCoroutine(TransitionOpacityTo(1));
        GoToNextDialog();
        if(currentDialogIndex == 4)
        {
            StartCoroutine(TransitionOpacityTo(0));
        }
    }
}
