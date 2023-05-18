using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSetter : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Title", true);
    }
}
