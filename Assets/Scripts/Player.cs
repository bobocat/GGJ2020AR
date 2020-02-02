using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Role {Sane, Insane}
    public Role role; // sane or insane
    public GameObject artifactToFind;
    public bool won = false;
    public GameObject artifactToFind_go;

    public void SetRole(string n){
        Player.Role _role;
        switch(n){
            case "Sane":
            _role = Player.Role.Sane;
            break;
            case "Insane":
            _role = Player.Role.Insane;
            break;
            default:
            _role = Player.Role.Insane;
            break;
        }
        role = _role;
    }

    public void SetArtifactToFind(){
        switch(role){
            case Player.Role.Insane:
            artifactToFind = GameLogic.instance.insaneArtifact;
            break;
            case Player.Role.Sane:
            artifactToFind = GameLogic.instance.saneArtifact;
            break;
            default:
            artifactToFind = GameLogic.instance.saneArtifact;
            break;
        }
                artifactToFind_go = Instantiate(artifactToFind, GameLogic.instance.worldRoot.transform.position, Quaternion.identity);
                artifactToFind_go.transform.SetParent(GameLogic.instance.worldRoot.transform);
                artifactToFind_go.SetActive(true);

    }
}
