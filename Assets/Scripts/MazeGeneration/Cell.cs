using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    // Cell information
    public Vector3 groundPosition, wallPosition1, wallPosition2, wallPosition3, wallPosition4;
    public Vector3 groundDimension = new Vector3(1f, .25f, 1f);
    public Vector3 wallDimension1 = new Vector3(1f, 1f, .25f);
    public Vector3 wallDimension2 = new Vector3(1f, 1f, .25f);
    public bool ground, wall1, wall2, wall3, wall4;
}
