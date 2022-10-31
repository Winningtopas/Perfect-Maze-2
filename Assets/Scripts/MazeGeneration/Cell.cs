using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    // Cell information
    public Vector3 groundPosition, wallPosition1, wallPosition2, wallPosition3, wallPosition4;
    public Vector3[] dimensions = new Vector3[] { new Vector3(1f, .1f, 1f), new Vector3(1f, 1f, .1f), new Vector3(.1f, 1f, 1f), new Vector3(1f, 1f, .1f), new Vector3(.1f, 1f, 1f) };
    public Vector3[] positions = new Vector3[] { new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, .9f), new Vector3(0.9f, 0f, 0f) };
    public bool[] dividers = new bool[] {false, false, false, false, false }; // the ground is the first divider
}
