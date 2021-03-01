using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class XRRestrictedMovement : XRBaseInteractable
{
    public bool allowRotation = true;
    [SerializeField]
    public bool allowTranslateY = true;
    public bool allowTranslateZ = true;

    // limit range +- translationRange * 0.05f + -0.025f(for margin)
    // if 3 ==> -0.175 ~ 0.175(will snap to 0.15)
    [Range(1, 5)]
    public int translationRange = 3;


    float m_TranslationRangeMin => -1 * (translationRange * k_Step + k_Step / 2);
    float m_TranslationRangeMax => m_TranslationRangeMin * -1;

    public bool snapTranslation = true;
    public bool snapRotation = true;



    const float k_Step = 0.05f; // 0.05 unit snap
    const float k_SnapRotation = 15; // 15 degree snap

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

    Vector3 m_GrabbedPosition;
    // rotation axis is x axis (1, 0, 0)
    


    protected override void Awake()
    {
        base.Awake();

        //the base class already grab it but don't expose it so have to grab it again
        m_Rb = GetComponent<Rigidbody>();
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        // In this step, object has no parent. if must refer original parent, use m_OriginalSceneParent
        if (isSelected)
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                if (allowTranslateY || allowTranslateZ)
                {
                    m_Rb.velocity = Vector3.zero;
                    // get center of mass relative to transform center
                    Vector3 rigidBodyPosition = m_Rb.worldCenterOfMass;
                    // change world location of grabber relative to this transform and calc difference
                    // difference of point == direction
                    Vector3 difference = m_OriginalSceneParent.transform.InverseTransformDirection(m_GrabbingInteractor.attachTransform.position - rigidBodyPosition);
                    // Find the local position difference in local location
                    var diffy = allowTranslateY ? difference.y : 0f;
                    var diffz = allowTranslateZ ? difference.z : 0f;
                    // calculate final position delta and check against original parent
                    var positionDelta = new Vector3(0, diffy, diffz);
                    var finalPosition = m_OriginalSceneParent.transform.InverseTransformPoint(m_Rb.position) + positionDelta;

                    // Limit desired movement within specified length
                    finalPosition.y = Mathf.Clamp(finalPosition.y, m_TranslationRangeMin, m_TranslationRangeMax);
                    finalPosition.z = Mathf.Clamp(finalPosition.z, m_TranslationRangeMin, m_TranslationRangeMax);

                    //var worldPositionDelta = transform.TransformDirection(positionDelta);
                    // apply location difference

                    // change back to world position
                    var finalWorldPosition = m_OriginalSceneParent.transform.TransformPoint(finalPosition);

                    m_Rb.MovePosition(finalWorldPosition);
                }

                if (allowRotation)
                {
                    // check world rotaion or local rotation(is desired)
                    m_Rb.angularVelocity = Vector3.zero;

                    // 1. 필요한 변수들을 가져온다
                    // 이 오브젝트의 현재 Rotation (Quaternion)
                    Quaternion rigidBodyRotation = m_Rb.transform.rotation;
                    // 오브젝트의 Red arrow가 향하는 각도
                    Vector3 xAxisOfThis = Vector3.right;

                    // Hand의 직전 위치 m_GrabbedPosition
                    // Hand의 현재 위치 m_GrabbingInteractor.attachTransform.position
                    Vector3 currentInteractorPosition = m_GrabbingInteractor.attachTransform.position;
                    // m_Rb의 현재 위치를 중심으로 삼아야 하지 않나.
                    Vector3 oldCenterToController = m_GrabbedPosition - transform.position;
                    oldCenterToController.Normalize();
                    Vector3 centeroToController = currentInteractorPosition - transform.position;
                    centeroToController.Normalize();

                    // 2. 오브젝트 현재 위치에 대비해 각도를 계산한다.

                    // 이 오브젝트의 각도 대비 전과 현재 hand의 이동에 의해 달라진 각도(float)
                    float relativeInteractorAngle = Vector3.SignedAngle(oldCenterToController, centeroToController, xAxisOfThis);
                    if (relativeInteractorAngle < 0) relativeInteractorAngle = 360 + relativeInteractorAngle;

                    // 3. 이 각도만큼 위에서 구한 axis 대비로 회전해준다.
                    Quaternion requiredRotation = Quaternion.AngleAxis(relativeInteractorAngle, xAxisOfThis);

                    // 4. Rigidbody를 (현재 회전한 각도 * 3에서 구한 각도)만큼 회전시킨다.
                    m_Rb.MoveRotation(rigidBodyRotation * requiredRotation);

                    // 5. Position을 Update해준다.
                    m_GrabbedPosition = currentInteractorPosition;
                }

            }
        }
    }

    /// <inheritdoc />
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        // Stub to make things work
        OnSelectEnter(args.interactor);
    }
    void OnSelectEnter(XRBaseInteractor interactor)
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
            m_GrabbedPosition = m_GrabbingInteractor.attachTransform.position;
            m_GrabbedRotation = m_GrabbingInteractor.attachTransform.rotation;


        }

    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        // Stub to make things work
        OnSelectExit(args.interactor);
    }

    void OnSelectExit(XRBaseInteractor interactor)
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

        // Snap position and rotation to nearest 0.05 and 15 degree if enabled
        TrySnapTransform();

    }

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        int interactorLayerMask = 1 << interactor.gameObject.layer;
        return base.IsSelectableBy(interactor) && (interactionLayerMask.value & interactorLayerMask) != 0;
    }

    public void TrySnapTransform()
    {
        // meant only called once, at the time of select exit.
        if (snapRotation)
        {
            var currentRotation = m_Rb.rotation;


        }
        if (snapTranslation)
        {
            var currentPosition = transform.localPosition;
            currentPosition.y = RoundToNearest(currentPosition.y, k_Step);
            currentPosition.z = RoundToNearest(currentPosition.z, k_Step);
            // Use Tween to snap animate;


        }
    }

    static float RoundToNearest(float number, float unit)
    {
        float remainder = number % unit;
        float result = remainder > unit / 2 ? number + (unit - remainder): number - remainder;
        return result;
    }

    private void OnDrawGizmos()
    {
        GizmoHelpers.DrawAxisArrows(transform, 3);
        Ray ray = new Ray(transform.position, transform.right);
        Gizmos.color = Color.white;
        Gizmos.DrawRay(ray);
        
        
    }


}
