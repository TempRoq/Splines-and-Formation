using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierPath))]
public class BezierPathEditor : Editor
{
    public bool showingPath;
    public float pathWidth;
    public bool forceConnected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override void OnInspectorGUI()
    {
        showingPath = GUILayout.Toggle(showingPath, "Show Path?");

        if (showingPath) {
            pathWidth = EditorGUILayout.FloatField("Path Width", pathWidth);
           

        }

        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        /*
        if (showingPath) {
            BezierPath BP = (BezierPath)target;

            for (int i = 0; i < BP.Nodes.Length; i++)
            {

                Handles.color = Color.white;

                Handles.DrawBezier(BP.transform.position + BP.Nodes[i].curve.AnchorA, BP.transform.position + BP.Nodes[i].curve.AnchorB, BP.Nodes[i].curve.Point1 - BP.Nodes[i].curve.AnchorA, BP.Nodes[i].curve.Point2 - BP.Nodes[i].curve.AnchorB, Color.white, Texture2D.blackTexture, .5f);
                Handles.DrawLine(BP.transform.position + BP.Nodes[i].curve.AnchorA, BP.transform.position + BP.Nodes[i].curve.Point1, pathWidth);
                Handles.DrawLine(BP.transform.position + BP.Nodes[i].curve.AnchorB, BP.transform.position + BP.Nodes[i].curve.Point2, pathWidth) ;

                BP.Nodes[i].curve.AnchorA = Handles.PositionHandle(BP.Nodes[i].curve.AnchorA, Quaternion.identity);
                BP.Nodes[i].curve.AnchorB = Handles.PositionHandle(BP.Nodes[i].curve.AnchorB, Quaternion.identity);
                BP.Nodes[i].curve.Point1 = Handles.PositionHandle(BP.Nodes[i].curve.Point1, Quaternion.identity);
                BP.Nodes[i].curve.Point2 = Handles.PositionHandle(BP.Nodes[i].curve.Point2, Quaternion.identity);
                if (forceConnected && i > 0)
                {
                    BP.Nodes[i].curve.AnchorA = BP.Nodes[i-1].curve.AnchorB;
                }
            }
        }
        */

    }

}

