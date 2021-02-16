using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShooter : MonoBehaviour
{

    public Color color = Color.red;

    public bool isLaserFiring = true;
    LineRenderer lineRenderer;

    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.right * 1000);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isLaserFiring)
        {
            // cast ray toward x position
            lineRenderer.enabled = true;

            // layermask 9 is Hand; hands should not block laser (for easy playing)
            int layerMask = 1 << 9;
            layerMask = ~layerMask;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity, layerMask))
            {
                // if ray hit object with tag "Interactable":
                // then try to activate it.
                if (hit.transform.CompareTag("Interactable") && hit.collider.gameObject.GetComponent<LaserEvent>() != null)
                {
                    // target is set and interactable
                    target = hit.transform.gameObject;
                    target.GetComponent<LaserEvent>().LaserIsActive();

                }
                else
                {
                    // something else is hit; set target to null
                    if (target != null)
                    {
                        target.GetComponent<LaserEvent>().LaserIsInactive();
                        target = null;
                    }

                }
                lineRenderer.SetPosition(1, Vector3.right * hit.distance);

                // draw line with linerenderer from here to transform ray just hit
            }
            else
            {
                // Not hit.
                // nothing is hit; set target to null;
                if (target != null)
                {
                    target.GetComponent<LaserEvent>().LaserIsInactive();
                    target = null;
                }

                // draw line from here to long distance;
                lineRenderer.SetPosition(1, Vector3.right * 1000);
            }

        }
        else
        {
            // stop drawing line
            lineRenderer.enabled = false;
            if (target != null)
            {
                target.GetComponent<LaserEvent>().LaserIsInactive();
                target = null;
            }


        }
    }

    public void FireLaser()
    {
        isLaserFiring = true;
    }

    public void StopLaser()
    {
        isLaserFiring = false;
    }

    public void ToggleLaser()
    {
        isLaserFiring = !isLaserFiring;
    }
}
