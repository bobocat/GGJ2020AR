using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Role {Sane, Insane}
    public Role role; // sane or insane
    public string artifactToFind;

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

    public void SetArtifactToFind(string artifact){
        artifactToFind = artifact;
    }
}
