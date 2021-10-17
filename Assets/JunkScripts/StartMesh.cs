using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMesh : MonoBehaviour
{
    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;

    void Start()
    {
        Vector3[] vert = {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 0),
        }
        ;

        newVertices = vert;

        gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
    }
}
