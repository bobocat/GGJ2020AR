using easyar;
using System;
using System.Collections.Generic;
using UnityEngine;

    public class GameLogic : MonoBehaviour
    {
        [System.Serializable]
        public class TwoSymbolArtifactLink{
            public string symbol1;
            public string symbol2;
            public GameObject artifact;
        }

        public ARSession Session;
        public TwoSymbolArtifactLink[] linkMap;
        private List<string> currentTargets = new List<string>();
        private Dictionary<ImageTargetController, List<TwoSymbolArtifactLink>> controllerMap = new Dictionary<ImageTargetController, List<TwoSymbolArtifactLink>>();
        private Dictionary<ImageTargetController, bool> imageTargetControllers = new Dictionary<ImageTargetController, bool>();
        private ImageTrackerFrameFilter imageTracker;
        private VideoCameraDevice cameraDevice;

        private void Awake()
        {
            imageTracker = Session.GetComponentInChildren<ImageTrackerFrameFilter>();
            cameraDevice = Session.GetComponentInChildren<VideoCameraDevice>();

            // targets from scene
            foreach(ImageTargetController control in Resources.FindObjectsOfTypeAll(typeof(ImageTargetController))){
                imageTargetControllers.Add(control, false);
                AddTargetControllerEvents(control);
            }
        }

        private void Update()
        {
            
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

        void TargetFound(ImageTargetController controller){
            string name = controller.Target.name();
            Debug.LogFormat("Found target {{id = {0}, name = {1}}}", controller.Target.runtimeID(), name);
            currentTargets.Add(name);
            foreach(TwoSymbolArtifactLink link in linkMap){
                if(name == link.symbol1){
                    if(currentTargets.Contains(link.symbol2)){
                        link.artifact.SetActive(true);
                        link.artifact.transform.SetParent(controller.transform);
                    }
                }
                else if(name == link.symbol2){
                    if(currentTargets.Contains(link.symbol1)){
                        link.artifact.SetActive(true);
                        link.artifact.transform.SetParent(controller.transform);

                    }
                }
                else{
                    Debug.Log("not a valid combination");
                }
            }

        }

        void TargetLost(ImageTargetController controller){
            string name = controller.Target.name();
             Debug.LogFormat("Lost target {{id = {0}, name = {1}}}", controller.Target.runtimeID(), controller.Target.name());
             currentTargets.Remove(name);
            foreach(TwoSymbolArtifactLink link in linkMap){
                if(name == link.symbol1){
                    if(currentTargets.Contains(link.symbol2)){
                        link.artifact.SetActive(false);
                        link.artifact.transform.SetParent(null);
                    }
                }
                else if(name == link.symbol2){
                    if(currentTargets.Contains(link.symbol1)){
                        link.artifact.SetActive(false);
                        link.artifact.transform.SetParent(null);
                    }
                }
                else{
                    Debug.Log("not a valid combination");
                }
            }
        }
        private void AddTargetControllerEvents(ImageTargetController controller)
        {
            if (!controller)
            {
                return;
            }

            controller.TargetFound += () => {TargetFound(controller);};
            controller.TargetLost += () => {TargetLost(controller);};
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

