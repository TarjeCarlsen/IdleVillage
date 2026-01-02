using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PopUpTextHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText_txt;
    [SerializeField] public string popUpText;
    [SerializeField] private Animator PopUpAnimation;


    public void RunPopUp(string message){
        displayText_txt.text = message;
        PopUpAnimation.Play("FadeOut");
    }

    public void RunPopUpFadeUp(string message){
        displayText_txt.text = message;
        PopUpAnimation.Play("FadeOutAndMoveUp");
    }

}
