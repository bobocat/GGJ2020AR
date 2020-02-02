using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Game {

    public string code;
    public Vector3 playerPosition;
    public string artifactTrigger1; // the artifact to find from the VR guy
    public string artifactTrigger2;
    public string artifactTrigger3;
    public float gateLevel;
    public Game()
    {
        gateLevel = 50f;
    }

}
