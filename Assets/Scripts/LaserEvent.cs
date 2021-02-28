using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserEvent : MonoBehaviour
{



    public bool isOnLaser = false;

    public UnityEvent OnLaserReceived;
    public UnityEvent OnLaserReceiving;
    public UnityEvent OnLaserStopped;

    private void LaserReceived()
    {
        OnLaserReceived.Invoke();
    }

    private void LaserReceiving()
    {
        // Each frame laser is received
        OnLaserReceiving.Invoke();
    }
    
    private void LaserStopped()
    {
        OnLaserStopped.Invoke();
    }



    public void LaserIsActive()
    {
        if (!isOnLaser)
        {
            isOnLaser = true;
            LaserReceived();
        }
        else
        {
            LaserReceiving();
        }
    }
    public void LaserIsInactive()
    {
        isOnLaser = false;
        LaserStopped();
    }

}
