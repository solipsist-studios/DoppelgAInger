using System.Collections;
using System.Collections.Generic;
using PolySpatial.Template;
using Unity.PolySpatial.InputDevices;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.Interaction.Toolkit;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchableCharacter : XRGrabInteractable
{
    private IXRSelectInteractor interactor;

    public GameObject HandCollider;

    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(StartGrab);
        selectExited.AddListener(EndGrab);
    }

    protected override void OnDisable()
    {
        selectEntered.RemoveListener(StartGrab);
        selectExited.RemoveListener(EndGrab);
        base.OnDisable();
    }

    private void StartGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;

        UpdateCollider();
    }

    private void EndGrab(SelectExitEventArgs args)
    {
        interactor = null;
    }

    private void UpdateCollider()
    {
        var interactorTransform = interactor.GetAttachTransform(this);

        if (interactorTransform == null || HandCollider == null)
        {
            return;
        }

        HandCollider.gameObject.transform.position = interactorTransform.position;
    }

    private void Update()
    {
        var activeTouches = Touch.activeTouches;

        if (activeTouches.Count > 0)
        {
            SpatialPointerState primaryTouchData = EnhancedSpatialPointerSupport.GetPointerState(activeTouches[0]);
            var touchPhase = activeTouches[0].phase;

            if (touchPhase == TouchPhase.Began && primaryTouchData.Kind == SpatialPointerKind.IndirectPinch ||
                                                  primaryTouchData.Kind == SpatialPointerKind.DirectPinch)
            {
                if (primaryTouchData.targetObject != null && HandCollider != null)
                {
                    //if (primaryTouchData.targetObject.TryGetComponent(out MeshRenderer renderer))
                    //{
                    //    renderer.enabled = true; //transform.SetPositionAndRotation(primaryTouchData.interactionPosition, primaryTouchData.inputDeviceRotation);
                    //}
                        
                }
            }
            else if (activeTouches[0].phase == TouchPhase.Moved)
            {
                if (primaryTouchData.targetObject != null && HandCollider != null)
                {
                    HandCollider.transform.SetPositionAndRotation(primaryTouchData.interactionPosition, primaryTouchData.inputDeviceRotation);
                    HandCollider.transform.Rotate(Vector3.up, 90); // The device rotation is offset from the mesh 90 degrees.
                }
            }
            else if (activeTouches[0].phase == TouchPhase.Ended)
            {
                if (HandCollider != null)
                {
                    //if (primaryTouchData.targetObject.TryGetComponent(out MeshRenderer renderer))
                    //{
                    //    renderer.enabled = false; //transform.SetPositionAndRotation(primaryTouchData.interactionPosition, primaryTouchData.inputDeviceRotation);
                    //}
                }
            }
        }
    }
} 