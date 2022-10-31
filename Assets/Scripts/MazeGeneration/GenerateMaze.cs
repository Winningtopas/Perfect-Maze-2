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
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

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

            newVertices.Add(cells[i].position);
            newVertices.Add(cells[i].position + Vector3.right);
            newVertices.Add(cells[i].position + new Vector3(1f, 1f, 0f));
            newVertices.Add(cells[i].position + Vector3.up);

            newVertices.Add(cells[i].position + new Vector3(0f, 1f, 1f));
            newVertices.Add(cells[i].position + new Vector3(1f, 1f, 1f));
            newVertices.Add(cells[i].position + new Vector3(1f, 0f, 1f));
            newVertices.Add(cells[i].position + Vector3.forward);

            newVertices.Add(cells[i].position);
            newVertices.Add(cells[i].position + Vector3.right);
            newVertices.Add(cells[i].position + new Vector3(1f, 1f, 0f));
            newVertices.Add(cells[i].position + Vector3.up);

            newVertices.Add(cells[i].position + new Vector3(0f, 1f, 1f));
            newVertices.Add(cells[i].position + new Vector3(1f, 1f, 1f));
            newVertices.Add(cells[i].position + new Vector3(1f, 0f, 1f));
            newVertices.Add(cells[i].position + Vector3.forward);
        }

        mesh.vertices = newVertices.ToArray();

        List<int> newTriangles = new List<int>();

        for (int i = 0; i < cells.Count; i++)
        {
            // Front Quad
            newTriangles.Add(i * 24); // BottomLeftVertex
            newTriangles.Add(i * 24 + 2); // TopLeftVertex
            newTriangles.Add(i * 24 + 1); // BottomRightVertex
            newTriangles.Add(i * 24); // BottomRightVertex
            newTriangles.Add(i * 24 + 3); // TopLeftVertex
            newTriangles.Add(i * 24 + 2); // TopRightVertex

            // Top Quad
            newTriangles.Add(i * 24 + 10); // BottomLeftVertex
            newTriangles.Add(i * 24 + 11); // TopLeftVertex
            newTriangles.Add(i * 24 + 12); // BottomRightVertex
            newTriangles.Add(i * 24 + 10); // BottomRightVertex
            newTriangles.Add(i * 24 + 12); // TopLeftVertex
            newTriangles.Add(i * 24 + 13); // TopRightVertex

            // Right Quad
            newTriangles.Add(i * 24 + 9); // BottomLeftVertex
            newTriangles.Add(i * 24 + 18); // TopLeftVertex
            newTriangles.Add(i * 24 + 5); // BottomRightVertex
            newTriangles.Add(i * 24 + 9); // BottomRightVertex
            newTriangles.Add(i * 24 + 5); // TopLeftVertex
            newTriangles.Add(i * 24 + 6); // TopRightVertex

            // Left Quad
            newTriangles.Add(i * 24 + 8); // BottomLeftVertex
            newTriangles.Add(i * 24 + 7); // TopLeftVertex
            newTriangles.Add(i * 24 + 20); // BottomRightVertex
            newTriangles.Add(i * 24 + 8); // BottomRightVertex
            newTriangles.Add(i * 24 + 20); // TopLeftVertex
            newTriangles.Add(i * 24 + 19); // TopRightVertex

            // Back Quad
            newTriangles.Add(i * 24 + 21); // BottomLeftVertex
            newTriangles.Add(i * 24 + 20); // TopLeftVertex
            newTriangles.Add(i * 24 + 15); // BottomRightVertex
            newTriangles.Add(i * 24 + 21); // BottomRightVertex
            newTriangles.Add(i * 24 + 15); // TopLeftVertex
            newTriangles.Add(i * 24 + 14); // TopRightVertex

            // Bottom Quad
            newTriangles.Add(i * 24 + 16); // BottomLeftVertex
            newTriangles.Add(i * 24 + 22); // TopLeftVertex
            newTriangles.Add(i * 24 + 23); // BottomRightVertex
            newTriangles.Add(i * 24 + 16); // BottomRightVertex
            newTriangles.Add(i * 24 + 17); // TopLeftVertex
            newTriangles.Add(i * 24 + 22); // TopRightVertex
        }

        mesh.triangles = newTriangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
}
