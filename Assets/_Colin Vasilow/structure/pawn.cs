using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pawn : MonoBehaviour
{

    //public variables
    public GameObject simulation;
    void Start(){
        simulation = GameObject.FindGameObjectWithTag("Universe");
    }

    public void getPawnActions() {
        return;
    }

    public void turnEvents() {
        return;
    }
}
