using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class XRRotationOnly : XRBaseInteractable
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
    Quaternion m_GrabbedRotation;

    // rotation axis is x axis (1, 0, 0)



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
                m_Rb.angularVelocity = Vector3.zero;
                Quaternion grabRotation = m_Rb.transform.rotation;
                Quaternion difference = m_GrabbingInteractor.attachTransform.rotation * Quaternion.Inverse(grabRotation);
                
                var rotationX = Quaternion.AngleAxis(difference.eulerAngles.x, Vector3.right);
                
                m_Rb.MoveRotation(grabRotation * rotationX);
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
            m_GrabbingInteractor = interactor;
            m_GrabbedRotation = m_GrabbingInteractor.transform.rotation.normalized;

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

        transform.parent = m_OriginalSceneParent;

        m_GrabbingInteractor = null;

        base.OnSelectExit(interactor);
    }

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        int interactorLayerMask = 1 << interactor.gameObject.layer;
        return base.IsSelectableBy(interactor) && (interactionLayerMask.value & interactorLayerMask) != 0;
    }
}
