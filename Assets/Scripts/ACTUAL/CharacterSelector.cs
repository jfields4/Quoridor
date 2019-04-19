using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public GameObject[] character;

    // Start is called before the first frame update
    void Start()
    {
        character[AppController.Character].SetActive(true);
        /*Debug.Log(AppController.Character);
        Mesh mesh = characterMeshes[AppController.Character];
        Mesh mesh2 = Instantiate(mesh);
        GetComponent<MeshFilter>().sharedMesh = mesh2;*/
    }

    // Update is called once per frame
    void Update()
    {
        /*Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int p = 0;
        while (p < vertices.Length)
        {
            vertices[p] += new Vector3(0, Random.Range(-0.3F, 0.3F), 0);
            p++;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();*/
    }
}
