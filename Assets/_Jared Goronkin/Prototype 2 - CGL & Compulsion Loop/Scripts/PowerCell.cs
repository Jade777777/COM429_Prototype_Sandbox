using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCell : MonoBehaviour
{
    Vector3 a = new Vector3(-0.5f, 0, 0);
    Vector3 b = new Vector3(0, Mathf.Sqrt(.75f), 0);
    Vector3 c = new Vector3(0.5f, 0, 0);

    float weaponPower = 0;
    float sheildPower = 0;
    float thrusterPower = 0;

    Vector3 sliderPosition;

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;

    private void Start()
    {
        
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        MeshCollider col = gameObject.AddComponent<MeshCollider>();
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();


        mesh.vertices = new Vector3[] { a, b, c };
        mesh.triangles = new int[] { 0, 1, 2 };


        col.sharedMesh = mesh;
    }


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SetSliderPosition();
        }
    }
    public void SetSliderPosition()
    {
        RaycastHit hit;

        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)
            || hit.collider == null 
            || hit.collider.gameObject != gameObject)
        {
            return;
        }


        Vector3 baryCenter = hit.barycentricCoordinate;
        sheildPower = baryCenter.x;
        weaponPower = baryCenter.y;
        thrusterPower = baryCenter.z;
        Debug.Log(hit.barycentricCoordinate);
    }
}


    

