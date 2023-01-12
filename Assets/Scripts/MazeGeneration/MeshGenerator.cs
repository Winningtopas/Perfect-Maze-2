using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public List<Cell> cells;
    public List<List<int>> trianglesByDividerList;
    public Mesh targetMesh;

    public void DrawMesh(List<Cell> newCells, List<List<int>> newTrianglesByDividerList)
    {
        cells = newCells;
        trianglesByDividerList = newTrianglesByDividerList;

        targetMesh = GetComponent<MeshFilter>().mesh;
        targetMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        List<Vector3> verticesList = CreateVertices();
        Vector3[] vertices = verticesList.ToArray();
        targetMesh.vertices = vertices;

        CreateTriangles();

        List<int> allTriangles = new List<int>();

        foreach (List<int> list in trianglesByDividerList)
            allTriangles.AddRange(list);

        int[] triangles = allTriangles.ToArray();
        targetMesh.triangles = triangles;

        Vector2[] uvs = CreateUVs(vertices, triangles);
        targetMesh.uv = uvs;

        targetMesh.subMeshCount = 5; //targetMesh counter
        targetMesh.SetTriangles(trianglesByDividerList[0], 0);
        targetMesh.SetTriangles(trianglesByDividerList[1], 1);
        targetMesh.SetTriangles(trianglesByDividerList[2], 2);
        targetMesh.SetTriangles(trianglesByDividerList[3], 3);
        targetMesh.SetTriangles(trianglesByDividerList[4], 4);

        targetMesh.Optimize();
        targetMesh.RecalculateNormals();
    }

    private List<Vector3> CreateVertices()
    {
        List<Vector3> newVertices = new List<Vector3>();
        int number = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < cells[i].dividers.Length; j++)
            {
                if (cells[i].dividers[j])
                {
                    if (cells[i].isStartCell && j == 0 && cells[i].positions[0].y != 0) { } // if the startcell is not on the ground, leave it empty so the fp camera can move through it.
                    else
                    {
                        Vector3 dimensions = cells[i].dimensions[j];
                        for (int k = 1; k <= 3; k++) // Use 24 vertices instead of 8 on a cell so the lighting gets rendered correctly.
                        {
                            newVertices.Add(cells[i].positions[j]);
                            newVertices.Add(cells[i].positions[j] + new Vector3(1f * dimensions.x, 0f * dimensions.y, 0f * dimensions.z));
                            newVertices.Add(cells[i].positions[j] + new Vector3(1f * dimensions.x, 1f * dimensions.y, 0f * dimensions.z));
                            newVertices.Add(cells[i].positions[j] + new Vector3(0f * dimensions.x, 1f * dimensions.y, 0f * dimensions.z));

                            newVertices.Add(cells[i].positions[j] + new Vector3(0f * dimensions.x, 1f * dimensions.y, 1f * dimensions.z));
                            newVertices.Add(cells[i].positions[j] + new Vector3(1f * dimensions.x, 1f * dimensions.y, 1f * dimensions.z));
                            newVertices.Add(cells[i].positions[j] + new Vector3(1f * dimensions.x, 0f * dimensions.y, 1f * dimensions.z));
                            newVertices.Add(cells[i].positions[j] + new Vector3(0f * dimensions.x, 0f * dimensions.y, 1f * dimensions.z));
                            number += 8;
                        }
                    }
                }
            }
        }
        return newVertices;
    }

    private void CreateTriangles()
    {
        int lastIndex = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < cells[i].dividers.Length; j++)
            {
                if (cells[i].dividers[j])
                {
                    if (cells[i].isStartCell && j == 0 && cells[i].positions[0].y != 0) // if the startcell of this floor is not on the ground floor, then leave it empty.
                    { }
                    else
                    {
                        trianglesByDividerList[j].Add(lastIndex); // BottomLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 2); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 1); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex + 3); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 2); // TopRightVertex

                        // Top Quad
                        trianglesByDividerList[j].Add(lastIndex + 10); // BottomLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 11); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 12); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex + 10); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex + 12); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 13); // TopRightVertex

                        // Right Quad
                        trianglesByDividerList[j].Add(lastIndex + 9); // BottomLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 18); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 5); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex + 9); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex + 5); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 6); // TopRightVertex

                        // Left Quad
                        trianglesByDividerList[j].Add(lastIndex + 8); // BottomLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 7); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 4); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex + 8); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex + 4); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 19); // TopRightVertex

                        // Back Quad
                        trianglesByDividerList[j].Add(lastIndex + 21); // BottomLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 20); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 15); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex + 21); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex + 15); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 14); // TopRightVertex

                        // Bottom Quad
                        trianglesByDividerList[j].Add(lastIndex + 16); // BottomLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 22); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 23); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex + 16); // BottomRightVertex
                        trianglesByDividerList[j].Add(lastIndex + 17); // TopLeftVertex
                        trianglesByDividerList[j].Add(lastIndex + 22); // TopRightVertex

                        lastIndex += 24;
                    }
                }
            }
        }
    }

    private Vector2[] CreateUVs(Vector3[] vertices, int[] triangles)
    {
        Vector2[] newUvs = new Vector2[vertices.Length];
        Vector3 v1, v2, v3;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            v1 = vertices[triangles[i]];
            v2 = vertices[triangles[i + 1]];
            v3 = vertices[triangles[i + 2]];

            if (v1.x == v2.x && v1.x == v3.x) // red/blue
            {
                newUvs[triangles[i]] = new Vector2(v1.z, v1.y);
                newUvs[triangles[i + 1]] = new Vector2(v2.z, v2.y);
                newUvs[triangles[i + 2]] = new Vector2(v3.z, v3.y);
            }

            if (v1.y == v2.y && v1.y == v3.y) // top
            {
                newUvs[triangles[i]] = new Vector2(v1.x, v1.z);
                newUvs[triangles[i + 1]] = new Vector2(v2.x, v2.z);
                newUvs[triangles[i + 2]] = new Vector2(v3.x, v3.z);
            }

            if (v1.z == v2.z && v1.z == v3.z) // green/purple
            {
                newUvs[triangles[i]] = new Vector2(v1.x, v1.y);
                newUvs[triangles[i + 1]] = new Vector2(v2.x, v2.y);
                newUvs[triangles[i + 2]] = new Vector2(v3.x, v3.y);
            }
        }
        return newUvs;
    }
}
