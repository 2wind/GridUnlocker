using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(XRRestrictedMovement))]
public class Slot : MonoBehaviour
{
    XRRestrictedMovement component;

    public GameObject centerOfRotation;
    void Awake()
    {
        if (centerOfRotation == null)
        {
            centerOfRotation = transform.Find("CenterOfRotation").gameObject;
        }
        component = GetComponent<XRRestrictedMovement>();

        GetComponent<Rigidbody>().centerOfMass = centerOfRotation.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // TODO: block transformation on the fly, LOCALLY.

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.01f);
    }

    public void SnapToNearestGrid()
    {
        // snap to latest transform up to 0.05 
        // and rotation up to 15 degree
        // smoothly or instantly(for now)


    }

    public void ShowVisualAid()
    {
        if (component.allowRotation)
            centerOfRotation.GetComponent<Renderer>().enabled = true;


    }

    public void HideVisualAid()
    {
        if (component.allowRotation)
            centerOfRotation.GetComponent<Renderer>().enabled = false;
    }
}
