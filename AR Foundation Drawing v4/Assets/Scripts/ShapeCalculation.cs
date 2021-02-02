using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;


public static class ShapeCalculation
{
    public static Vector3 origin = Vector3.zero;
    private const float MOVE = 0.01f; 
    private const float ANGLE = 150;
    private static TrailRenderer trail;

    private struct TrailPoints : IComparable<TrailPoints>
    {
        public Vector3 coords;
        public float magnutude;

        public int CompareTo(TrailPoints other)
        {
            return magnutude.CompareTo(other.magnutude);
        }
    }

    private static List<TrailPoints> trailPointsList = new List<TrailPoints>();

    public static void DrawObject(GameObject currentStroke, LineRenderer lineRend)
    {
        if (currentStroke == null)
        {
            Debug.LogError("Current Stroke is null");
        }

        trailPointsList.Clear();
        trail = currentStroke.GetComponent<TrailRenderer>();

        origin = GetOrigin(trail);

        for (int i = 0; i < trail.positionCount; i++)
        {
            TrailPoints point;
            point.coords = trail.GetPosition(i);
            point.coords.z = origin.z;
            point.magnutude = (point.coords - origin).magnitude;
            trailPointsList.Add(point);
        }
        if (trailPointsList == null)
        {
            Debug.LogError("Trail List is null");
        }
        else
        {
            DrawLine(GetVertexes(trailPointsList), lineRend);
        }

    }

    private static Vector3 GetOrigin(TrailRenderer trail)
    {
        Vector3 origin = Vector3.zero;
        for (int i = 0; i < trail.positionCount; i++)
        {
            origin += trail.GetPosition(i);
        }
        origin /= trail.positionCount;
        return origin;
    }


    private static Vector3[] GetVertexes(List<TrailPoints> trailPointsList)
    {
        List<Vector3> vertexesList = new List<Vector3>();
        int count = 0;//count of polygon vertexes
                      //var count = trailPointsList.Count;

        //Debug.LogError("Trail List points count: " + trailPointsList.Count);

        vertexesList.Add(trailPointsList[0].coords);
        for (int i = 1; i < trailPointsList.Count - 1; i++)
        {
            var angle = Vector3.Angle(trailPointsList[i - 1].coords - trailPointsList[i].coords, trailPointsList[i + 1].coords - trailPointsList[i].coords);
            if (angle < ANGLE)
            {
                count++;
                vertexesList.Add(trailPointsList[i].coords);
            }
        }

        if (count < 2 || count > 20)
        {
            //circle
            return null;
        }
        else
        {
            //FindMax(trailPointsList);
            return FindMax(trailPointsList).ToArray();
        }

    }

    private static void DrawLine(Vector3[] vertexesArray, LineRenderer lineRend)
    {
        if (vertexesArray == null)
        {
            Debug.Log("Circle");
            //Drawing Circle
            DrawCircle(lineRend);
        }
        else
        {
            Debug.Log("Polygon");
            lineRend.positionCount = vertexesArray.Length;
            lineRend.loop = true;
            lineRend.SetPositions(vertexesArray);
        }
    }

    private static Vector3 CalculateRadius(List<TrailPoints> trailPointsList)
    {
        return new Vector3((trail.bounds.max.x - trail.bounds.min.x) / 2, (trail.bounds.max.y - trail.bounds.min.y) / 2, 0);

    }

    private static void DrawCircle(LineRenderer lineRend)
    {
        var radius = CalculateRadius(trailPointsList);
        var points = 90;
        lineRend.positionCount = points + 1;
        lineRend.loop = true;
        Vector3[] vertices = new Vector3[points + 1];
        for (int i = 0; i < vertices.Length; i++)
        {
            float x = Mathf.Cos(i / (float)points * 2 * Mathf.PI);
            float y = Mathf.Sin(i / (float)points * 2 * Mathf.PI);
            vertices[i] = new Vector3(x * radius.x + origin.x, y * radius.y + origin.y, origin.z);
        }
        lineRend.SetPositions(vertices);
    }

    private static void AnglesCheck(List<Vector3> vectorList, Vector3 lastPosition)
    {
        var d = Vector3.Distance(trailPointsList[0].coords, trailPointsList[1].coords);
        
        var count = vectorList.Count();
        Debug.Log("Count: " + count);
        for (int i = 1; i < count - 1; i++)
        {
            var angle = Vector3.Angle(vectorList[i - 1] - vectorList[i], vectorList[i + 1] - vectorList[i]);
            Debug.Log("Angle " + i + ": " + angle);
            Debug.Log("Count: " + count);
            if (angle > ANGLE)
            {
                vectorList.RemoveAt(i);
                count--;
            }
        }
        var lastAngle = Vector3.Angle(vectorList[count - 2] - vectorList[count - 1], lastPosition - vectorList[count - 1]);
        if (lastAngle > ANGLE)
        {
            vectorList.RemoveAt(count - 1);
            count--;
        }
    }

    private static List<Vector3> FindMax(List<TrailPoints> pointsList)
    {
        List<Vector3> maxVertexesList = new List<Vector3>();
        int tempIndex = 0;
        int maxIndex = 0;

        var d = Vector3.Distance(pointsList[0].coords, pointsList[1].coords);
        maxVertexesList.Add(pointsList[0].coords);
        for (int i = 0; i < pointsList.Count - 1; i++)
        {
            if (pointsList[i + 1].magnutude > pointsList[i].magnutude && Vector3.Distance(pointsList[maxIndex].coords, pointsList[i].coords) > d * 10)//if 
            {
                tempIndex = i + 1;
            }
            else if (pointsList[i + 1].magnutude < pointsList[i].magnutude && Vector3.Distance(pointsList[maxIndex].coords, pointsList[i].coords) > d * 10)
            {

                if (maxIndex != tempIndex)
                {
                    maxVertexesList.Add(pointsList[tempIndex].coords);
                }
                maxIndex = tempIndex;
            }
        }
        AnglesCheck(maxVertexesList, pointsList.Last().coords);
        return maxVertexesList;
    }

    private static void CalculateMesh(GameObject mesh, Material colorMaterial, LineRenderer lineRend, string meshType)
    {
        Mesh newMesh = new Mesh();
        Vector3[] vertices = new Vector3[lineRend.positionCount];

        List<int> triangles = new List<int>();
        lineRend.GetPositions(vertices);

        for (int i = 0, j = vertices.Length - 1; i < vertices.Length / 2; i++, j--)
        {
            triangles.Add(j);
            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(j);
            triangles.Add(i + 1);
            triangles.Add(j - 1);
        }

        if (vertices.Length % 2 != 0)//нечетн
        {
            var i = vertices.Length / 2;
            triangles.Add(i - 1);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        List<Vector2> uv = new List<Vector2>();
        for (int i = 0; i < vertices.Length; i++)
        {
            float x = vertices[i].x;
            float y = vertices[i].y;
            float z = vertices[i].z;
            Vector2 vector = new Vector2();
            vector.x = x * 5;
            vector.y = y * 5;
            uv.Add(vector);
            Debug.Log("UV " + i + ": " + uv[i]);
        }
        lineRend.SetWidth(0.003f, 0.003f);
        newMesh.vertices = vertices;
        newMesh.uv = uv.ToArray();

        newMesh.triangles = triangles.ToArray();
        if (meshType == "back")
        {
            newMesh.triangles = newMesh.triangles.Reverse().ToArray();
        }
        

        mesh.GetComponent<MeshFilter>().mesh = newMesh;
        mesh.GetComponent<MeshRenderer>().material = colorMaterial;
    }

    

    public static void GenerateOuterColorMesh(GameObject mesh, Material colorMaterial, LineRenderer lineRend)
    {
        Mesh newMesh = new Mesh();
        Vector3[] vertices = new Vector3[lineRend.positionCount];

        List<int> triangles = new List<int>();
        lineRend.GetPositions(vertices);

        for (int i = 0, j = vertices.Length - 1; i < vertices.Length / 2; i++, j--)
        {
            triangles.Add(i);
            triangles.Add(j);
            triangles.Add(i + 1);
            triangles.Add(j);
            triangles.Add(j - 1);
            triangles.Add(i + 1);
        }

        if (vertices.Length % 2 != 0)//нечетн
        {
            var i = vertices.Length / 2;
            triangles.Add(i);
            triangles.Add(i - 1);
            triangles.Add(i + 1);
        }

        List<Vector2> uv = new List<Vector2>();

        for (int i = 0; i < vertices.Length; i++)
        {
            float x = vertices[i].x;
            float y = vertices[i].y;
            float z = vertices[i].z;
            Vector2 vector = new Vector2();
            vector.x = x*5;
            vector.y = y*5;
            uv.Add(vector);
            Debug.Log("UV " + i + ": " + uv[i]);
        }

        lineRend.SetWidth(0.003f, 0.003f);
        newMesh.vertices = vertices;
        newMesh.uv = uv.ToArray();
        newMesh.triangles = triangles.ToArray();

        mesh.GetComponent<MeshFilter>().mesh = newMesh;
        mesh.GetComponent<MeshRenderer>().material = colorMaterial;
    }
    public static void GenerateInnerColorMesh2(GameObject mesh, Material colorMaterial, LineRenderer lineRend)
    {
        Mesh newMesh = new Mesh();
        Vector3[] vertices = new Vector3[lineRend.positionCount];

        List<int> triangles = new List<int>();
        lineRend.GetPositions(vertices);
        //Debug.Log("Before: " + vertices[0]);
        for (int i=0; i<vertices.Length; i++)
        {
            vertices[i].z += MOVE;
        }
        //Debug.Log("After: " + vertices[0]);

        for (int i = 0, j = vertices.Length - 1; i < vertices.Length / 2; i++, j--)
        {
            triangles.Add(j);
            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(j);
            triangles.Add(i + 1);
            triangles.Add(j - 1);
        }

        if (vertices.Length % 2 != 0)//нечетн
        {
            var i = vertices.Length / 2;
            triangles.Add(i - 1);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        List<Vector2> uv = new List<Vector2>();
        for (int i = 0; i < vertices.Length; i++)
        {
            float x = vertices[i].x;
            float y = vertices[i].y;
            float z = vertices[i].z;
            Vector2 vector = new Vector2();
            vector.x = x * 5;
            vector.y = y * 5;
            uv.Add(vector);
            Debug.Log("UV " + i + ": " + uv[i]);
        }
        lineRend.SetWidth(0.003f, 0.003f);
        newMesh.vertices = vertices;
        newMesh.uv = uv.ToArray();
        newMesh.triangles = triangles.ToArray();

        mesh.GetComponent<MeshFilter>().mesh = newMesh;
        mesh.GetComponent<MeshRenderer>().material = colorMaterial;
    }

    public static void GenerateBorderMesh(GameObject outerMesh, Material outerMaterial, LineRenderer lineRend)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();

        List<int> outerTriangles = new List<int>();

        Vector3[] verticesTop = new Vector3[lineRend.positionCount];//mesh vertices

        lineRend.GetPositions(verticesTop);
        for (int i = 0; i < verticesTop.Length; i++)//set vertices in order top -> bottom
        {
            vertices.Add(verticesTop[i]);
            var bottomVertice = verticesTop[i];
            bottomVertice.z += MOVE;
            vertices.Add(bottomVertice);
        }

        for (int i = 0; i < vertices.Count; i += 2)
        {
            outerTriangles.Add(i);
            outerTriangles.Add(i + 2);
            outerTriangles.Add(i + 1);
            outerTriangles.Add(i + 1);
            outerTriangles.Add(i + 2);
            outerTriangles.Add(i + 3);
        }

        vertices.Add(vertices[0]);
        vertices.Add(vertices[1]);

        outerTriangles.Add(vertices.Count - 2);
        outerTriangles.Add(0);
        outerTriangles.Add(vertices.Count - 1);
        outerTriangles.Add(vertices.Count - 1);
        outerTriangles.Add(0);
        outerTriangles.Add(1);
        


        
        mesh.vertices = vertices.ToArray();

        List<Vector2> uv = new List<Vector2>();

        for (int i = 0; i < vertices.Count; i++)
        {
            float x = vertices[i].x;
            float y = vertices[i].y;
            float z = vertices[i].z;
            Vector2 vector = new Vector2();
            vector.x = Mathf.Sqrt(x * x + y * y) * 5;
            vector.y = z * 5;
            uv.Add(vector);
            
        }
        Debug.Log("UV count: " + uv.Count);
        Debug.Log("Vertices Count: " + vertices.Count);
        mesh.uv = uv.ToArray();
        mesh.triangles = outerTriangles.ToArray();

        outerMesh.GetComponent<MeshFilter>().mesh = mesh;
        outerMesh.GetComponent<MeshRenderer>().material = outerMaterial;
    }

    public static float MeshExtrusion(Slider slider, Mesh mesh)
    {
        List<Vector3> meshVertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        uvs.AddRange(mesh.uv);
        float zPos = 0;
        mesh.GetVertices(meshVertices);//2 4 6 8
        for (int i=1;i<meshVertices.Count; i+=2)
        {
            var move = new Vector3(0, 0, MOVE * slider.value);
            if (zPos==0)
            {
                zPos = move.z - MOVE;
            }           
            meshVertices[i] = meshVertices[i - 1] + move;
            
            var x = meshVertices[i].x;
            var y = meshVertices[i].y;
            var z = meshVertices[i].z;
            uvs[i] = new Vector2(Mathf.Sqrt(x * x + y * y)*5, z*5);
        }
        
        mesh.uv = uvs.ToArray();
        mesh.vertices = meshVertices.ToArray();

        return zPos;
    }

}