using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Draw : MonoBehaviour
{
    
    private float x = 0;
    private float y = 0;
    private GameObject currentStroke;
    public static bool isChanged = false;

    [SerializeField]
    //private GameObject innerMesh;
    //[SerializeField]
    private GameObject outerMesh;
    [SerializeField]
    private GameObject innerMesh2;
    //[SerializeField]
    //private GameObject outerMesh2;
    //[SerializeField]
    //private GameObject innerMeshBorder;
    [SerializeField]
    private GameObject outerMeshBorder;

    public GameObject mesh;
    public GameObject obj;

    public Material colorMaterial;
    public Material textureMaterial;

    public Material boardsmaterial;
    public bool editorTesting;
    public GameObject penPoint;
    public GameObject stroke;
    public Shaping shape;
    public FigureRecognition figure;
    public LineRenderer lineRend;
    public TMPro.TMP_Dropdown dropdown;

    public static bool drawing = false;
    // Start is called before the first frame update
    private void Start()
    {
        penPoint.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (editorTesting)
        {
            x += Input.GetAxis("Mouse X");
            y += Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(-y, x, 0);
        }
    }

    public void StartStroke()
    {
        drawing = true;
        penPoint.SetActive(true);
        currentStroke = Instantiate(stroke, penPoint.transform.position, penPoint.transform.rotation);
        
    }

    public void EndStroke()
    {
        penPoint.SetActive(false);
        drawing = false;
    }

    public void ChangeShape()
    {
        ReturnObjectToInitialState();
        DrawFigure();
        DrawMeshes();
        Destroy(currentStroke);
        isChanged = false;
        ChangeExtrusionShape(GameObject.Find("Slider").GetComponent<Slider>());
        lineRend.gameObject.SetActive(false);
    }

    private void DrawFigure()
    {
        ShapeCalculation.DrawObject(currentStroke, lineRend);
    }


    private void DrawMeshes()
    {
        ShapeCalculation.GenerateOuterColorMesh(outerMesh, colorMaterial, lineRend);
        ShapeCalculation.GenerateInnerColorMesh2(innerMesh2, colorMaterial, lineRend);
        ShapeCalculation.GenerateBorderMesh(outerMeshBorder, boardsmaterial, lineRend);
        
    }

    private void ReturnObjectToInitialState()
    {
        if (obj.transform.localScale!=Vector3.one)
        {
            obj.transform.localScale = Vector3.one;
            obj.transform.rotation = Quaternion.identity;
        }
        obj.transform.position = Vector3.zero;
        mesh.transform.position = Vector3.zero;
        lineRend.gameObject.transform.position = Vector3.zero;
    }

    public void ChangeExtrusionShape(Slider slider)
    {
        var z = ShapeCalculation.MeshExtrusion(slider, outerMeshBorder.GetComponent<MeshFilter>().mesh);
        ShapeCalculation.MeshExtrusion(slider, outerMeshBorder.GetComponent<MeshFilter>().mesh);
        var pos = innerMesh2.transform.position;
        pos.z = z;
        innerMesh2.transform.position = pos;
        //outerMesh.transform.position = pos;
    }

    public void ChangeColorToGreen()
    {
        foreach (MeshRenderer mesh in mesh.GetComponentsInChildren<MeshRenderer>())
        {
            mesh.material = colorMaterial;
            mesh.material.color = Color.green;           
        }
    }

    public void ChangeColorToBlue()
    {
        foreach (MeshRenderer mesh in mesh.GetComponentsInChildren<MeshRenderer>())
        {
            mesh.material = colorMaterial;
            mesh.material.color = Color.blue;
        }
    }

    public void ChangeTexture(Texture texture)
    {
        
        foreach (MeshRenderer mesh in mesh.GetComponentsInChildren<MeshRenderer>())
        {
            mesh.material = textureMaterial;
            mesh.material.mainTexture = texture;
        }
    }
}

