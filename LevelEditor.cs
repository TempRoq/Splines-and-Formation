using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public int toShow = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnInspectorGUI()
    {
        
        toShow = EditorGUILayout.IntField("ShowPath", toShow);   
        base.OnInspectorGUI();
        
    }
    private void DrawBezierPath(BezierPath bp, Vector3 worldPosition)
    {

        for (int i = 0; i < bp.Nodes.Length; i++)
        {
            float lineThickness = toShow == i || toShow == -1 ? .5f : .25f;
            Color c = (toShow == i || toShow != -1 ? Color.gray : Color.white);
           // Handles.DrawBezier(worldPosition + bp.Nodes[i].curve.AnchorA, worldPosition + bp.Nodes[i].curve.AnchorB, bp.Nodes[i].curve.Point1 - bp.Nodes[i].curve.AnchorA, bp.Nodes[i].curve.Point2 - bp.Nodes[i].curve.AnchorB, c, Texture2D.whiteTexture, lineThickness * 2);
            Handles.DrawLine(worldPosition + bp.startPoint + bp.Nodes[i].curve.AnchorA, worldPosition + bp.startPoint + bp.Nodes[i].curve.Point1, lineThickness);
            Handles.DrawLine(worldPosition + bp.startPoint + bp.Nodes[i].curve.AnchorB, worldPosition + bp.startPoint + bp.Nodes[i].curve.Point2, lineThickness);

            if (i == 0)
            {
                bp.Nodes[i].curve.AnchorA = Handles.PositionHandle(bp.Nodes[i].curve.AnchorA + bp.startPoint, Quaternion.identity) - worldPosition - bp.startPoint;
            }
            else
            {
                bp.Nodes[i].curve.AnchorA = bp.Nodes[i - 1].curve.AnchorB;
            }
            bp.Nodes[i].curve.AnchorB = Handles.PositionHandle(bp.Nodes[i].curve.AnchorB + bp.startPoint, Quaternion.identity) - worldPosition - bp.startPoint;
            bp.Nodes[i].curve.Point1 = Handles.PositionHandle(bp.Nodes[i].curve.Point1 + bp.startPoint, Quaternion.identity) - worldPosition - bp.startPoint;
            bp.Nodes[i].curve.Point2 = Handles.PositionHandle(bp.Nodes[i].curve.Point2 + bp.startPoint, Quaternion.identity) - worldPosition - bp.startPoint;

        }
    }

    private void OnSceneGUI()
    {
        Level l = (Level)target;
        for (int i = 0; i < l.pathPool.Length; i++)
        {
            DrawBezierPath(l.pathPool[i], l.worldPosition);
        }


    }

    
}