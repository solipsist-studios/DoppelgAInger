using System.Collections.Generic;
using UnityEngine;

#if UNITY_WSA
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
#endif

public class SceneSetup : MonoBehaviour
{
#if UNITY_WSA
    [SerializeField] private AzureSpatialAnchors spatialAnchorsManager;
#endif

    [SerializeField] private GameObject placementObject;

    private bool areAnchorsReceived = false;
    private bool areAnchorsProcessed = false;
    private bool doesAnchorExist = false;

    private void Start()
    {
        // Query spatial anchors
#if UNITY_EDITOR             
        this.areAnchorsReceived = true;
#elif UNITY_WSA
        if (this.spatialAnchorsManager != null)
        {
            this.spatialAnchorsManager.AnchorsReceivedCallback += SpatialAnchorsManager_AnchorsReceivedCallback;
            this.spatialAnchorsManager.AnchorLocatedCallback += SpatialAnchorsManager_AnchorLocatedCallback;
        }
#endif
    }

    private void Update()
    {
        if (this.areAnchorsReceived && !this.areAnchorsProcessed) 
        {
            // If spatial anchors found, exit placement mode
            if (this.doesAnchorExist)
            {
                this.placementObject.SetActive(false);
            }

            this.areAnchorsProcessed = true;
        }
    }

#if UNITY_WSA
    private void SpatialAnchorsManager_AnchorsReceivedCallback(List<AnchorObjectModel> objs)
    {
        this.doesAnchorExist = objs.Count > 0;
        this.areAnchorsReceived = true;
    }

    private void SpatialAnchorsManager_AnchorLocatedCallback(AnchorObjectModel anchorModel, AnchorLocatedEventArgs args)
    {
        // Creating and adjusting GameObjects have to run on the main thread. We are using the UnityDispatcher to make sure this happens.
        UnityDispatcher.InvokeOnAppThread(() =>
        {
            // Instantiate anchored prefab
            GameObject newTextbox = this.spatialAnchorsManager.SpawnAnchoredObject(this.spatialAnchorsManager.AnchoredObjectTemplate, args.Anchor);
        });
    }
#endif
}
