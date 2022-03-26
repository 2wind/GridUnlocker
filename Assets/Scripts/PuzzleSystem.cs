using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UIEventDelegate;

public class PuzzleSystem : MonoBehaviour
{
    public string puzzleName;

    public List<LaserEvent> checkList;

    public LaserEvent checker;

    private XRRestrictedMovement[] grabbableChildren;


    void Awake()
    {
        grabbableChildren = GetComponentsInChildren<XRRestrictedMovement>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        checker.OnLaserReceived.AddListener(CheckVictoryCondition);

    }

    private void OnDisable()
    {
        checker.OnLaserReceived.RemoveListener(CheckVictoryCondition);

    }

    public void CheckVictoryCondition()
    {
        if (IsVictoryCondition())
            PuzzleManager.instance.PuzzleSolved();
    }

    public bool IsVictoryCondition()
    {

        return checkList.TrueForAll(element => element.isOnLaser);


    }

    public void EnableGrabbableChildren()
    {
        foreach (var child in grabbableChildren)
        {
            child.enabled = true;
        }
    }

    public void DisableGrabbableChildren()
    {
        foreach (var child in grabbableChildren)
        {
            child.enabled = false;
        }
    }
}
