using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class planetaryStructure : MonoBehaviour
{
    public GameObject parentSystem;
    public abstract string giveDescription();
}
