using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BearController : MonoBehaviour
{
    public float speed = 3.0f;

    private bool idle = false;
    private bool walk = false;
    private bool run = false;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(Control());
    }

    IEnumerator Control()
    {
        while (true)
        {
            walk = Input.GetKey(KeyCode.UpArrow);
            run = Input.GetKey(KeyCode.LeftShift);

            idle = true;
            if (walk || run)
            {
                idle = false;
            }

            if (idle)
            {
                anim.SetFloat("Speed", 0f);
            }
            else if (walk && !run)
            {
                anim.SetFloat("Speed", 2f);
            }
            else if (walk && run)
            {
                anim.SetFloat("Speed", 5f);
            }

            float turn = Input.GetAxis("Horizontal");
            anim.SetFloat("Direction", turn);

            yield return null;
        }
    }

    void HandleInput()
    {
        walk = Input.GetKeyDown(KeyCode.UpArrow);
        run = Input.GetKeyDown(KeyCode.LeftShift);

        idle = true;
        if(walk || run)
        {
            idle = false;
        }
    }

    void Update()
    {
        HandleInput();
        if (idle)
        {
            anim.SetFloat("Speed", 0f);
        }
        else if (walk && !run)
        {
            anim.SetFloat("Speed", 2f);
        }
        else if (walk && run)
        {
            anim.SetFloat("Speed", 5f);
        }
    }
}
