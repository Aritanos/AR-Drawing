using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private bool isRotating = false;
    private bool isScaling = false;
    //private bool isChanged = false;
    private Vector3 scaleChange = new Vector3(0.05f, 0.05f, 0);
    public Draw drawObject;
    public GameObject obj;
    public GameObject line;
    public GameObject mesh;
    //public GameObject shapeObject;
    //private LineRenderer objectLine;
    private Vector3 objOrigin;
    private float origin = 0;

    private void Start()
    {
        //objectLine = shapeObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3[] vertexes = new Vector3[objectLine.positionCount];
        float delta;
        if (isRotating)
        {
            delta = GetDelta();
            //line.transform.RotateAround(objOrigin, Vector3.forward, delta);
            //mesh.transform.RotateAround(objOrigin, Vector3.forward, delta);
            obj.transform.RotateAround(objOrigin, Vector3.forward, delta);
        }

        if (isScaling)
        {

            delta = GetDelta();
            if (delta>0)
            {
                obj.transform.localScale += scaleChange;
                //line.transform.localScale += scaleChange;
                //mesh.transform.localScale += scaleChange;
                //obj.transform.Translate(-scaleChange);
            }
            else if (delta<0)
            {
                obj.transform.localScale -= scaleChange;
                //line.transform.localScale -= scaleChange;
                //mesh.transform.localScale -= scaleChange;
                //obj.transform.Translate(scaleChange);
            }
        }
    }

    public void StartRotation()
    {
        objOrigin = ShapeCalculation.origin;
        //Vector3 vector = new Vector3(ShapeCalculation.origin.x, ShapeCalculation.origin.y, 0);
        //obj.transform.position = vector;
        //objOrigin = shapeObject.GetComponent<Shaping>().origin;
        if (drawObject.editorTesting)
        {
            origin = Input.mousePosition.x;
        }
        else if (Input.touchCount>0)
        {
            origin = Input.GetTouch(0).position.x;
        }
        isRotating = true;
        obj.GetComponentInChildren<LineRenderer>().useWorldSpace = false;
    }

    public void EndRotation()
    {
        isRotating = false;
        //obj.transform.position = Vector3.zero;
        //objectLine.useWorldSpace = true;
        origin = 0;
    }

    public void StartScaling()
    {
        if (!Draw.isChanged)
        {
            var vector = new Vector3(ShapeCalculation.origin.x, ShapeCalculation.origin.y, 0);
            obj.transform.Translate(vector);
            mesh.transform.Translate(-vector, Space.Self);
            line.transform.Translate(-vector, Space.Self);
            Draw.isChanged = true;
        }
        
        objOrigin = ShapeCalculation.origin;
        //Vector3 vector = new Vector3(ShapeCalculation.origin.x, ShapeCalculation.origin.y, 0); 
        //obj.transform.position = vector;

        //objOrigin = shapeObject.GetComponent<Shaping>().origin;
        isScaling = true;
        if (drawObject.editorTesting)
        {
            origin = Input.mousePosition.x;
        }
        if (Input.touchCount > 0)
        {
            origin = Input.GetTouch(0).position.x;
        }
        obj.GetComponentInChildren<LineRenderer>().useWorldSpace = false;
    }

    public void EndScaling()
    {

        /*var vector = new Vector3(ShapeCalculation.origin.x, ShapeCalculation.origin.y, 0);
        obj.transform.Translate(-vector);
        mesh.transform.Translate(vector, Space.Self);
        line.transform.Translate(vector, Space.Self);*/

        isScaling = false;
        //obj.transform.position = Vector3.zero;
        origin = 0;
    }

    private float GetDelta()
    {
        float delta = 0;
        if (drawObject.editorTesting)
        {
            delta = Input.mousePosition.x - origin;
            origin = Input.mousePosition.x;
        }
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            delta = touch.position.x - origin;
            origin = touch.position.x;
        }
        return delta;
    }
}
