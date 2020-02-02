using easyar;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [System.Serializable]
    public class TwoSymbolArtifactLink
    {
        public ImageTargetController symbol1;
        public ImageTargetController symbol2;
        public GameObject artifact;
        public int comboBit;
        public bool firstTimeFound = false;
    }

    public ARSession Session;
    public TwoSymbolArtifactLink[] linkMap;
    public float maxDistance = 3f; // distance between the two symbols
    private Dictionary<ImageTargetController, bool> imageTargetControllers = new Dictionary<ImageTargetController, bool>();
    private Dictionary<ImageTargetController, int> controllerMap = new Dictionary<ImageTargetController, int>(); // controller map to bit
    private ImageTrackerFrameFilter imageTracker;
    private VideoCameraDevice cameraDevice;

    [SerializeField] int detectedTargets = 0;

    private void Awake()
    {
        imageTracker = Session.GetComponentInChildren<ImageTrackerFrameFilter>();
        cameraDevice = Session.GetComponentInChildren<VideoCameraDevice>();

        int power = 1;
        // targets from scene
        foreach (ImageTargetController control in Resources.FindObjectsOfTypeAll(typeof(ImageTargetController)))
        {
            imageTargetControllers.Add(control, false);
            AddTargetControllerEvents(control);
            controllerMap.Add(control, (int)Math.Pow(2, power)); // set controller to a bit number (power of 2)
            power++;
        }
        // calculate combination bits for each combo of symbols that trigger an artifact
        foreach (TwoSymbolArtifactLink link in linkMap)
        {
            int bit1 = controllerMap[link.symbol1];
            int bit2 = controllerMap[link.symbol2];
            link.comboBit = bit1 | bit2;
        }
    }

    private void Update()
    {
        if (GameManager.instance.gamePhase == GameManager.State.Playing)
        {
            // check if combination has been detected
            foreach (TwoSymbolArtifactLink link in linkMap)
            {
                int combo = link.comboBit; // int operators are destructive
                if ((combo & detectedTargets) == link.comboBit)
                {
                    ActivateArtifact(link, link.symbol1.transform);

                    // mainly for first phase of game: can only raise and lower gate levels once
                    if (!link.firstTimeFound)
                    {
                        /*                        switch(GameManager.instance.game.gameLevel){
                                                    case 1:
                                                        GameManager.instance.game.artifact1 = link.artifactName;
                                                        break;
                                                    case 2:
                                                        GameManager.instance.game.artifact2 = link.artifactName;
                                                        break;
                                                    case 3:
                                                        GameManager.instance.game.artifact3 = link.artifactName;
                                                        break;
                                                }
                                                GameManager.instance.game.gameLevel++;
                        */
                        // write the role of the player sane or insane who found the artifact to the artifact database

                        // write the name of the role that found the artifact to the database and update the gate level
                        link.firstTimeFound = true;
                        GameManager.instance.UpdateGateLevel(GameManager.instance.player.role);
                        DataManager.instance.WriteGameDataToFirebase();
                    }
                    if (link.artifact.name == GameManager.instance.player.artifactToFind)
                    {
                        GameManager.instance.game.foundArtifact = true;
                        GameManager.instance.player.won = true;
                        DataManager.instance.WriteGameDataToFirebase();
                        GameManager.instance.EndGame();
                    }
                }
                else
                {
                    link.artifact.transform.SetParent(null);
                }
            }
        }
    }

    public void Tracking(bool on)
    {
        imageTracker.enabled = on;
    }

    public void UnloadTargets()
    {
        foreach (var item in imageTargetControllers)
        {
            item.Key.Tracker = null;
        }
    }

    public void LoadTargets()
    {
        foreach (var item in imageTargetControllers)
        {
            item.Key.Tracker = imageTracker;
        }
    }

    public void EnableCamera(bool enable)
    {
        cameraDevice.enabled = enable;
    }

    void ActivateArtifact(TwoSymbolArtifactLink link, Transform parent)
    {
        float distance = (link.symbol1.transform.position - link.symbol2.transform.position).magnitude;
        Debug.Log($"distance {distance}");
        if (distance < maxDistance)
        {
            link.artifact.SetActive(true);
            link.artifact.transform.position = parent.position;
            link.artifact.transform.SetParent(parent);
        }
        else
        {
            link.artifact.SetActive(false);
            link.artifact.transform.SetParent(null);
        }

    }
    void TargetFound(ImageTargetController controller)
    {
        string name = controller.Target.name();
        Debug.LogFormat("Found target {{id = {0}, name = {1}}}", controller.Target.runtimeID(), name);
        detectedTargets |= controllerMap[controller]; //add controller to list of detected controllers
    }

    void TargetLost(ImageTargetController controller)
    {
        string name = controller.Target.name();
        Debug.LogFormat("Lost target {{id = {0}, name = {1}}}", controller.Target.runtimeID(), controller.Target.name());
        detectedTargets = detectedTargets & ~controllerMap[controller]; //remove controller from list of detected controllers
    }
    private void AddTargetControllerEvents(ImageTargetController controller)
    {
        if (!controller)
        {
            return;
        }

        controller.TargetFound += () => { TargetFound(controller); };
        controller.TargetLost += () => { TargetLost(controller); };
        controller.TargetLoad += (Target target, bool status) =>
        {
            imageTargetControllers[controller] = status ? true : imageTargetControllers[controller];
            Debug.LogFormat("Load target {{id = {0}, name = {1}, size = {2}}} into {3} => {4}", target.runtimeID(), target.name(), controller.Size, controller.Tracker.name, status);
        };
        controller.TargetUnload += (Target target, bool status) =>
        {
            imageTargetControllers[controller] = status ? false : imageTargetControllers[controller];
            Debug.LogFormat("Unload target {{id = {0}, name = {1}}} => {2}", target.runtimeID(), target.name(), status);
        };
    }
}

