using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shaping : MonoBehaviour
{
    //0 for circle, 1 for triangle, 2 for square and so on
    //private  Vector3[] vertices;
    public TMPro.TMP_Dropdown dropdown;
    //public Toggle isRegularShape;

    
    private LineRenderer lineRend;

    public static Vector3 origin; 

    // Start is called before the first frame update
    void Start()
    {
        lineRend = GetComponent<LineRenderer>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawLine(GameObject stroke) // circle
    {
        TrailRenderer trailRend = stroke.GetComponent<TrailRenderer>();
        Vector3[] trailVertices = new Vector3[trailRend.positionCount];
        trailRend.GetPositions(trailVertices);
        origin = GetOrigin(trailVertices);
        Debug.Log("Origin: " + origin);
        Vector3 radius = CalculateRadius(trailRend);
        var points = 90;
        lineRend.positionCount = points + 1;
        lineRend.loop = true;
        Vector3[] vertices = new Vector3[points + 1];
        for (int i = 0; i< vertices.Length; ++i)
        {
            float x = Mathf.Cos(i / (float)points * 2 * Mathf.PI);
            float y = Mathf.Sin(i / (float)points * 2 * Mathf.PI);
            vertices[i] = new Vector3(x * radius.x + origin.x, y * radius.y + origin.y, origin.z);
        }
        lineRend.SetPositions(vertices);

    }

    public void DrawLineRec(Vector3[] vertices) // polygon
    {
        origin = GetOrigin(vertices);
        lineRend.positionCount = vertices.Length;
        lineRend.loop = true;
        lineRend.SetPositions(vertices);
    }    

    private Vector3 CalculateRadius(TrailRenderer trailRend)
    {
        return new Vector3((trailRend.bounds.max.x - trailRend.bounds.min.x) / 2, (trailRend.bounds.max.y - trailRend.bounds.min.y) / 2, 0);  
    }

    private Vector3 GetOrigin(Vector3[] vertices)
    {
        Vector3 origin = Vector3.zero;
        for (int i = 0; i < vertices.Length; i++)
        {
            origin += vertices[i];
        }
        Debug.Log("Origin: " + origin);
        origin.x /= vertices.Length;
        origin.y /= vertices.Length;
        origin.z /= vertices.Length;
        Debug.Log("Origin: " + origin);
        return origin;
    }
}
