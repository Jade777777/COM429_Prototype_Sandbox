using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class PowerCell : MonoBehaviour
{
    public Vector3 a = new Vector3(-0.5f, 0, 0);
    public Vector3 b = new Vector3(0, Mathf.Sqrt(.75f), 0);
    public Vector3 c = new Vector3(0.5f, 0, 0);
    public Color aColor;
    public Color bColor;
    public Color cColor;

    public Vector3 SliderPosition;

    float weaponPower = 0;
    float sheildPower = 0;
    float thrusterPower = 0;


    MeshCollider col;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    private void UpdatePowerCell()
    {

        col = gameObject.GetComponent<MeshCollider>();
        col.convex = false;
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        


        if (meshRenderer.sharedMaterial == null)
        {
            meshRenderer.sharedMaterial = new(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        }
        if (meshFilter.sharedMesh == null)
        {
            mesh = new Mesh();
            meshFilter.mesh = mesh;
        }
        else
        {
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();
        mesh.vertices = new Vector3[] { a, b, c };// (a+b+c)/3+new Vector3(0,0,-1) };
        mesh.colors = new Color[] { aColor, bColor, cColor };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0) };
        mesh.triangles = new int[] { 0, 1, 2 };//, 0, 3, 1, };

        col.sharedMesh = mesh;
    }

    private void Update()
    {
        UpdatePowerCell();

        if (Application.IsPlaying(gameObject))
        {
            
            if (Input.GetMouseButton(0))
            {
                SetSliderPosition();
            }
        }


    }
    public void SetSliderPosition()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane tPlane = new Plane(transform.TransformPoint(a), transform.TransformPoint(b), transform.TransformPoint(c));
        tPlane.Raycast(ray, out float distance);
        Vector3 targetPosition = ray.GetPoint(distance);
        targetPosition = transform.InverseTransformPoint(targetPosition);
        Debug.Log("Hit on plane: " + targetPosition);

        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)
            || hit.collider == null 
            || hit.collider.gameObject != gameObject)
        {
            return;
        }

        SliderPosition = transform.InverseTransformPoint(hit.point);

        Vector3 baryCenter = hit.barycentricCoordinate;
        sheildPower = baryCenter.x;
        weaponPower = baryCenter.y;
        thrusterPower = baryCenter.z;
        Debug.Log(hit.barycentricCoordinate);
    }

}


    

