using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    
    public float radius = 3f;
    public float lineWidth = 0.2f;
    public Color color  = new(1f, 0.8f, 0.3f, 0.5f);

    private const float ThetaScale = 0.01f;
    private int Size;
    private float Theta;
    LineRenderer lineRenderer;
    void Start()
    {
        Material mat = new(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.color = color;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = mat;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        SetValue();
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
    public void SetValue()
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        Theta = 0f;
        Size = (int)((1f / ThetaScale) + 1f);
        lineRenderer.positionCount = Size;

        for (int i = 0; i < Size; i++)
        {
            Theta += (2.0f * Mathf.PI * ThetaScale);
            float x = radius * Mathf.Cos(Theta);
            float y = radius * Mathf.Sin(Theta);
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
