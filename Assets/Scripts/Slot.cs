using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // TODO: block transformation on the fly, LOCALLY.

    private void OnDrawGizmosSelected()
    {
        // Draw gizmo at the position of slot with center yellow
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(0.05f, 0.5f, 0.5f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.01f);
    }

    public void SnapToNearestGrid()
    {
        // snap to latest transform up to 0.05 
        // and rotation up to 15 degree
        // smoothly or instantly(for now)


    }
}
