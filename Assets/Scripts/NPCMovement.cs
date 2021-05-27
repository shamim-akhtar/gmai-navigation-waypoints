using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public Animator mAnimator;
    public Transform[] mWayPoints;
    public float mStoppingDistance = 1.0f;
    public float mSpeed = 2.50f;

    void Start()
    {
        //mAnimator.SetFloat("Speed", 5.0f);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            MoveTo(mWayPoints[0]);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            MoveTo_StopGradual(mWayPoints[0]);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PatrolWapoints();
        }
    }

    private void MoveTo(Transform wayPoint)
    {
        // We cannot implement the movement code here in one function.
        // this is because this movement function needs to be called 
        // every frame until the NPC reaches its destination (or stops).
        //
        // There are many ways of implementing it,
        // 1. One of the ways will be to use a FSM and create a MoveTo State.
        // 2. Second way will be to implement as a aTask for your BT
        // 3. Third way will be to use a simple coroutine. (However, this is 
        // not an elegant way of doing it if this movement is part of a 
        // bigger and more complex project.
        //
        // Since our job here is to demonstrate the movement to waypoint code
        // so we can simply use coroutines.

        StartCoroutine(Coroutine_MoveTo(wayPoint));
    }

    private void MoveTo_StopGradual(Transform wayPoint)
    {
        StartCoroutine(Coroutine_MoveTo_StopGradual(wayPoint));
    }

    bool ReachedTarget(Transform target)
    {
        float dist = (transform.position - target.position).magnitude;
        if(dist < mStoppingDistance)
        {
            return true;
        }
        return false;
    }

    IEnumerator Coroutine_MoveTo(Transform target)
    {
        yield return StartCoroutine(Coroutine_MoveToWayPoint(target));

        // Abrupt change of speed (or set to zero) will look ugly.
        // Our next objective is to make the stop gradual.
        mAnimator.SetFloat("Speed", 0.0f);
    }

    IEnumerator Coroutine_MoveTo_StopGradual(Transform target)
    {
        yield return StartCoroutine(Coroutine_StartOverTime(2.0f));
        yield return StartCoroutine(Coroutine_MoveToWayPoint(target));
        yield return StartCoroutine(Coroutine_StopOverTime(2.0f));
    }

    // Refactoring
    IEnumerator Coroutine_MoveToWayPoint(Transform target)
    {
        while (!ReachedTarget(target))
        {
            Vector3 mpos = transform.position;
            Vector3 tpos = target.position;

            Vector3 currentDir = transform.forward;
            Vector3 wantedDir = (tpos - mpos).normalized;

            Vector3 crossProd = Vector3.Cross(currentDir, wantedDir);

            mAnimator.SetFloat("Speed", mSpeed);
            mAnimator.SetFloat("Direction", crossProd.y);

            //yield return null;
            //yield return WaitForEndOfFrame;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator Coroutine_StopOverTime(float stopTime)
    {
        float startTime = Time.time;
        float dt = Time.time - startTime;
        while(dt < stopTime)
        {
            float timeDil = dt / stopTime;
            Debug.Log("Time Dil: " + timeDil);

            float factor = 1.0f - dt / stopTime;
            float speed = mSpeed * factor;

            mAnimator.SetFloat("Speed", speed);

            dt = Time.time - startTime;
            yield return null;
        }

        mAnimator.SetFloat("Speed", 0.0f);
    }

    IEnumerator Coroutine_StartOverTime(float duration)
    {
        float startTime = Time.time;
        float dt = Time.time - startTime;
        while (dt < duration)
        {
            float timeDil = dt / duration;
            Debug.Log("Time Dil: " + timeDil);

            float factor = dt / duration;
            float speed = mSpeed * factor;

            mAnimator.SetFloat("Speed", speed);

            dt = Time.time - startTime;
            yield return null;
        }

        mAnimator.SetFloat("Speed", mSpeed);
    }


    IEnumerator Coroutine_PatrolWapoints()
    {
        yield return StartCoroutine(Coroutine_StartOverTime(2.0f));
        int index = 0;
        while (index < mWayPoints.Length)
        {
            yield return StartCoroutine(Coroutine_MoveToWayPoint(mWayPoints[index]));
            index += 1;
        }
        yield return StartCoroutine(Coroutine_StopOverTime(2.0f));
    }
    void PatrolWapoints()
    {
        StartCoroutine(Coroutine_PatrolWapoints());
    }
}
