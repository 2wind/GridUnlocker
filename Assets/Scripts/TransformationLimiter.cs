using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// https://answers.unity.com/questions/1220999/rigidbody-local-position-constraints.html
public class TransformationLimiter : MonoBehaviour
{
    
    [Header("Freeze Local Position")]
    [SerializeField]
    bool positionX;
    [SerializeField]
    bool positionY;
    [SerializeField]
    bool positionZ;

    [Header("Freeze Local Rotation")]
    [SerializeField]
    bool rotationX;
    [SerializeField]
    bool rotationY;
    [SerializeField]
    bool rotationZ;


    Vector3 originalLocalPosition;
    Quaternion originalLocalRotation;



    // Start is called before the first frame update
    void Start()
    {
        SetOriginalTransform();
    }

    private void SetOriginalTransform()
    {
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        SetLocalPosition();
        SetLocalRotation();
    }

    private void SetLocalPosition()
    {
        float x, y, z;

        x = positionX ? originalLocalPosition.x : transform.localPosition.x;
        y = positionY ? originalLocalPosition.y : transform.localPosition.y;
        z = positionZ ? originalLocalPosition.z : transform.localPosition.z;

        transform.localPosition = new Vector3(x, y, z);
    }

    private void SetLocalRotation()
    {
        float x, y, z;
        x = rotationX ? originalLocalRotation.eulerAngles.x : transform.localRotation.eulerAngles.x;
        y = rotationY ? originalLocalRotation.eulerAngles.y : transform.localRotation.eulerAngles.y;
        z = rotationZ ? originalLocalRotation.eulerAngles.z : transform.localRotation.eulerAngles.z;
        Vector3 rotation = new Vector3(x, y, z);
        transform.localRotation = Quaternion.Euler(rotation);
    }
    
}