using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAnimations : MonoBehaviour
{
    public Animator PlayerAnimator;
    public string KnockTriggerParam = "KnockedOff";
    public Button KnockDownButton;
    public Toggle PushingToggle;
    public Slider SpeedSlider;
    public CharacterState characterState;
    private PlayerGruntEmitter gruntEmitter;

    private void Awake()
    {
        gruntEmitter = characterState.gameObject.GetComponent<PlayerGruntEmitter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PushingToggle.onValueChanged.AddListener((t) => { PlayerAnimator.SetBool("Pushing", t); });
        KnockDownButton.onClick.AddListener(() => { StartCoroutine(KnockdownPlayer()); });
        SpeedSlider.onValueChanged.AddListener((s) => { PlayerAnimator.SetFloat("Speed",s); });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetPlayerSpeed()
    {
        
    }

    IEnumerator KnockdownPlayer()
    {
        characterState.Bail();
        //PlayerAnimator.SetTrigger("KnockedOff");
        gruntEmitter.PlayRandom();
        yield return new WaitForSeconds(5);
        characterState.Respawn();
        //PlayerAnimator.ResetTrigger("KnockedOff");
       // PlayerAnimator.Play("Pushing");
    }

}
