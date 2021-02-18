using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class XRRestrictedMovement : XRBaseInteractable
{

    // variables from offset grabbable
    class SavedTransform
    {
        public Vector3 OriginalPosition;
        public Quaternion OriginalRotation;
    }


    Dictionary<XRBaseInteractor, SavedTransform> m_SavedTransforms = new Dictionary<XRBaseInteractor, SavedTransform>();

    Transform m_OriginalSceneParent;
    Rigidbody m_Rb;

    // variable from dial interactable
    XRBaseInteractor m_GrabbingInteractor;

    // rotation axis is x axis (1, 0, 0)

    public bool allowRotation = true;
    public bool allowTranslateY = true;
    public bool allowTranslateZ = true;


    protected override void Awake()
    {
        base.Awake();

        //the base class already grab it but don't expose it so have to grab it again
        m_Rb = GetComponent<Rigidbody>();
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (isSelected)
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                if (allowTranslateY || allowTranslateZ)
                {
                    // Limit desired movement within specified length?

                    // get center of mass relative to transform center
                    Vector3 rigidBodyPosition = m_Rb.worldCenterOfMass;
                    // change world location of grabber relative to this transform and calc difference
                    // difference of point == direction
                    Vector3 difference = transform.InverseTransformDirection(m_GrabbingInteractor.attachTransform.position - rigidBodyPosition);
                    // Find the local position difference in local location
                    var diffy = allowTranslateY ? difference.y : 0f;
                    var diffz = allowTranslateZ ? difference.z : 0f;
                    // calculate final position delta and translate back to world location
                    var positionDelta = new Vector3(0, diffy, diffz);
                    var worldPositionDelta = transform.TransformDirection(positionDelta);
                    // apply location difference
                    m_Rb.velocity = Vector3.zero;
                    m_Rb.MovePosition(m_Rb.position + worldPositionDelta);
                }

                if (allowRotation)
                {
                    // check world rotaion or local rotation(is desired)
                    m_Rb.angularVelocity = Vector3.zero;
                    // find rotation and find difference
                    Quaternion rigidBodyRotation = m_Rb.transform.rotation;
                    Quaternion difference = m_GrabbingInteractor.attachTransform.rotation * Quaternion.Inverse(rigidBodyRotation);

                    var rotationX = Quaternion.AngleAxis(difference.eulerAngles.x, Vector3.right);

                    m_Rb.MoveRotation(rigidBodyRotation * rotationX);
                }

            }
        }
    }

    protected override void OnSelectEnter(XRBaseInteractor interactor)
    {
        if (interactor is XRDirectInteractor)
        {
            
            SavedTransform savedTransform = new SavedTransform();

            savedTransform.OriginalPosition = interactor.attachTransform.localPosition;
            savedTransform.OriginalRotation = interactor.attachTransform.localRotation;

            m_SavedTransforms[interactor] = savedTransform;


            interactor.attachTransform.position = m_Rb.worldCenterOfMass;
            interactor.attachTransform.rotation = m_Rb.rotation;

            if (m_GrabbingInteractor == null)
            {
                m_OriginalSceneParent = transform.parent;
                transform.parent = null;
            }
            // save grabbing interactor for future use(i.e. calculating angle)
            m_GrabbingInteractor = interactor;
            // m_GrabbedRotation = m_GrabbingInteractor.transform.rotation.normalized;

        }

        base.OnSelectEnter(interactor);
    }

    protected override void OnSelectExit(XRBaseInteractor interactor)
    {
        if (interactor is XRDirectInteractor)
        {
            SavedTransform savedTransform = null;
            if (m_SavedTransforms.TryGetValue(interactor, out savedTransform))
            {
                interactor.attachTransform.localPosition = savedTransform.OriginalPosition;
                interactor.attachTransform.localRotation = savedTransform.OriginalRotation;

                m_SavedTransforms.Remove(interactor);
            }
        }

        // Retain original transform parent
        transform.parent = m_OriginalSceneParent;

        // Set grabbing interactor to null again
        m_GrabbingInteractor = null;

        // Snap position and rotation to nearest 0.05 and 15 degree


        base.OnSelectExit(interactor);
    }

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        int interactorLayerMask = 1 << interactor.gameObject.layer;
        return base.IsSelectableBy(interactor) && (interactionLayerMask.value & interactorLayerMask) != 0;
    }
}
