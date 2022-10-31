using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    // Maze information
    [SerializeField]
    private int mazeSizeX, mazeSizeY, mazeSizeZ;

    // Cell information
    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private List<Cell> cells = new List<Cell>();

    // Start is called before the first frame update
    void Start()
    {
        SpawnMaze();
        DrawMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // This functtion starts the generation of a new maze, the function can be called by a UI button.
    public void SpawnMaze()
    {
        DestroyMaze();
        GameObject Floor;

        for (int y = 0; y < mazeSizeY; y++)
        {
            Floor = new GameObject();
            Floor.name = "Floor" + y;
            Floor.transform.SetParent(transform);

            for (int x = 0; x < mazeSizeX; x++)
            {
                for (int z = 0; z < mazeSizeZ; z++)
                {
                    //GameObject Cell = Instantiate(cellPrefab, new Vector3(x, y, z), transform.rotation, Floor.transform);
                    Cell newCel = new Cell();
                    newCel.position = new Vector3(x, y, z);
                    cells.Add(newCel);
                }
            }
        }
    }

    // This function destroys the current maze.
    private void DestroyMaze()
    {

    }

    // Draw a maze mesh using the informations from the individual cells
    private void DrawMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        List<Vector3> newVertices = new List<Vector3>();

        for (int i = 0; i < cells.Count; i++)
        {
            newVertices.Add(cells[i].position);
            newVertices.Add(cells[i].position + Vector3.right);
            newVertices.Add(cells[i].position + new Vector3(1f, 1f, 0f));
            newVertices.Add(cells[i].position + Vector3.up);

            newVertices.Add(cells[i].position + new Vector3(0f, 1f, 1f));
            newVertices.Add(cells[i].position + new Vector3(1f, 1f, 1f));
            newVertices.Add(cells[i].position + new Vector3(1f, 0f, 1f));
            newVertices.Add(cells[i].position + Vector3.forward);

            Debug.Log("i: " + i + " position: " + cells[i].position);
        }

        mesh.vertices = newVertices.ToArray();

        List<int> newTriangles = new List<int>();

        for (int i = 0; i < cells.Count; i++)
        {
            // Front Quad
            newTriangles.Add(i * 8); // BottomLeftVertex
            newTriangles.Add(i * 8 + 2); // TopLeftVertex
            newTriangles.Add(i * 8 + 1); // BottomRightVertex
            newTriangles.Add(i * 8); // BottomRightVertex
            newTriangles.Add(i * 8 + 3); // TopLeftVertex
            newTriangles.Add(i * 8 + 2); // TopRightVertex

            // Top Quad
            newTriangles.Add(i * 8 + 2); // BottomLeftVertex
            newTriangles.Add(i * 8 + 3); // TopLeftVertex
            newTriangles.Add(i * 8 + 4); // BottomRightVertex
            newTriangles.Add(i * 8 + 2); // BottomRightVertex
            newTriangles.Add(i * 8 + 4); // TopLeftVertex
            newTriangles.Add(i * 8 + 5); // TopRightVertex

            // Right Quad
            newTriangles.Add(i * 8 + 1); // BottomLeftVertex
            newTriangles.Add(i * 8 + 2); // TopLeftVertex
            newTriangles.Add(i * 8 + 5); // BottomRightVertex
            newTriangles.Add(i * 8 + 1); // BottomRightVertex
            newTriangles.Add(i * 8 + 5); // TopLeftVertex
            newTriangles.Add(i * 8 + 6); // TopRightVertex

            // Left Quad
            newTriangles.Add(i * 8); // BottomLeftVertex
            newTriangles.Add(i * 8 + 7); // TopLeftVertex
            newTriangles.Add(i * 8 + 4); // BottomRightVertex
            newTriangles.Add(i * 8); // BottomRightVertex
            newTriangles.Add(i * 8 + 4); // TopLeftVertex
            newTriangles.Add(i * 8 + 3); // TopRightVertex

            // Back Quad
            newTriangles.Add(i * 8 + 5); // BottomLeftVertex
            newTriangles.Add(i * 8 + 4); // TopLeftVertex
            newTriangles.Add(i * 8 + 7); // BottomRightVertex
            newTriangles.Add(i * 8 + 5); // BottomRightVertex
            newTriangles.Add(i * 8 + 7); // TopLeftVertex
            newTriangles.Add(i * 8 + 6); // TopRightVertex

            // Bottom Quad
            newTriangles.Add(i * 8); // BottomLeftVertex
            newTriangles.Add(i * 8 + 6); // TopLeftVertex
            newTriangles.Add(i * 8 + 7); // BottomRightVertex
            newTriangles.Add(i * 8); // BottomRightVertex
            newTriangles.Add(i * 8 + 1); // TopLeftVertex
            newTriangles.Add(i * 8 + 6); // TopRightVertex
        }

        mesh.triangles = newTriangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        //if (i % 2 == 0) // If i is even
        //{
        //// Front triangle
        //newTriangles.Add(i * 2); // BottomLeftVertex
        //newTriangles.Add(i * 2 + 2); // TopLeftVertex
        //newTriangles.Add(i * 2 + 1); // BottomRightVertex
        //newTriangles.Add(i * 2 + 1); // BottomRightVertex
        //newTriangles.Add(i * 2 + 2); // TopLeftVertex
        //newTriangles.Add(i * 2 + 3); // TopRightVertex

        //else
        //{
        //    // Front triangle
        //    newTriangles.Add(i * 2 - 1);
        //    newTriangles.Add(i * 2 + 1);
        //    newTriangles.Add(i * 2 + 2);
        //    newTriangles.Add(i * 2 + 2);
        //    newTriangles.Add(i * 2 + 1);
        //    newTriangles.Add(i * 2 + 4);
        //}

        // Drawing a single quad

        //Mesh mesh = GetComponent<MeshFilter>().mesh;
        //mesh.vertices = new Vector3[] {
        //Vector3.zero, Vector3.right, Vector3.up, new Vector3(1f, 1f)
        //};

        //mesh.triangles = new int[] {
        //    0, 2, 1, 1, 2, 3
        //};
    }
}
