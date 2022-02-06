using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossfader : MonoBehaviour
{

    private Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //anim = GetComponentInChildren<Animator>();   
    }

    public void FadeIn()
    {
        anim.SetTrigger("CrossfadeIn");
    }
    
    public void FadeOut()
    {
        anim.SetTrigger("CrossfadeOut");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
