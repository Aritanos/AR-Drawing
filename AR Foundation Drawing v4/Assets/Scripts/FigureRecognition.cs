using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FigureRecognition: MonoBehaviour
{
    public struct TrailPoints : IComparable<TrailPoints>
    {
        public Vector3 coords;
        public float magnutude;

        public int CompareTo(TrailPoints other)
        {
            return magnutude.CompareTo(other.magnutude);
        }
    }

    [SerializeField]
    private Shaping shape;

    private List<TrailPoints> trailPointsList = new List<TrailPoints>();
    private List<Vector3> vertexesList = new List<Vector3>();
    private List<Vector3> maxVertexesList = new List<Vector3>();



    public void GetMaxPoints(GameObject currentStroke)
    {
        Vector3[] maxVertexes;
        if (currentStroke==null)
        {
            return;
        }

        var trail = currentStroke.GetComponent<TrailRenderer>();
        GetVertexes(trail);       
        FindMax(trailPointsList);
        maxVertexes = new Vector3[maxVertexesList.Count];
        for (int i = 0; i < maxVertexes.Length; i++)
        {
            maxVertexes[i] = maxVertexesList[i];
        }
        shape.DrawLineRec(maxVertexes);
    }

    private void GetVertexes(TrailRenderer trail)
    {
        vertexesList.Clear();
        trailPointsList.Clear();
        Vector3 origin = Vector3.zero;
        for (int i = 0; i < trail.positionCount; i++)
        {
            var sum = trail.GetPosition(i);
            vertexesList.Add(sum);
            origin += sum;
        }
        origin /= vertexesList.Count;

        for (int i = 0; i < vertexesList.Count; i++)
        {
            TrailPoints point;
            point.coords = vertexesList[i];
            point.coords.z = origin.z;
            point.magnutude = (point.coords - origin).magnitude;
            trailPointsList.Add(point);
        }
        CheckIfCircle(trailPointsList);
    }

    private void CheckIfCircle(List<TrailPoints> trailPoints)
    {
        int deviateCount = 0;
        for (int i = 1; i < trailPoints.Count-1; i++)
        {
            var angle = Vector3.Angle(trailPoints[i - 1].coords - trailPoints[i].coords, trailPoints[i + 1].coords - trailPoints[i].coords);
            if (angle<150)
            {
                deviateCount++; // from 3 to 8 angles must be
            }
        }
    }

    private void FindMax(List<TrailPoints> pointsList)
    {
        maxVertexesList.Clear();
        int tempIndex = 0;
        int maxIndex = 0;

        var d = Vector3.Distance(pointsList[0].coords, pointsList[1].coords);
        maxVertexesList.Add(pointsList[0].coords);
        for (int i = 0; i < pointsList.Count-1; i++)
        {
            if (pointsList[i+1].magnutude > pointsList[i].magnutude && Vector3.Distance(pointsList[maxIndex].coords, pointsList[i].coords) > d * 5)//if 
            {
                tempIndex = i + 1;
            }
            else if (pointsList[i + 1].magnutude < pointsList[i].magnutude && Vector3.Distance(pointsList[maxIndex].coords, pointsList[i].coords) > d * 5)
            {
                
                if (maxIndex!=tempIndex)
                {
                    maxVertexesList.Add(pointsList[tempIndex].coords);
                }
                maxIndex = tempIndex;
            }
        }
        AnglesCheck(maxVertexesList, pointsList.Last().coords);
    }

    private void AnglesCheck(List<Vector3> vectorList, Vector3 lastPosition)
    {
        var count = vectorList.Count();
        for (int i = 1; i< count -1; i++)
        {
            var angle = Vector3.Angle(vectorList[i - 1] - vectorList[i], vectorList[i + 1] - vectorList[i]);
            if (angle>150)
            {
                vectorList.RemoveAt(i);
                count--;
            }

        }
        var lastAngle = Vector3.Angle(vectorList[count - 2] - vectorList[count - 1], lastPosition - vectorList[count - 1]);
        if (lastAngle > 150)
        {
            vectorList.RemoveAt(count-1);
        }
    }
}
