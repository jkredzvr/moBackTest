using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * Given a curve:
 *  - has a start point at (x0, y0) and end point at (x1, y1)
 *  - defined by a cubic equation: f(x): y = a * x^3 + b * x^2 + c * x + d
 * 
 * Compute a bezier curve passing through all points on the curve
 * 
 * Use the code bellow as your template. Feel free to re-write any codes or implement your own solution!
 */


/// <summary>
/// Add this script on an empty gameobject. It will draw a bezier curve (star at x0, end at x1) and pass through all the points on the curve defined by
/// a cubic equation.
/// Change a, b, c, d, x0, and x1 to test your answer
/// </summary>
public class CubicEquation : MonoBehaviour {

    /// <summary>
    /// coefficients of a cubic equation.
    /// </summary>
    public float a, b, c, d;

    /// <summary>
    /// X position of start point and end point.
    /// </summary>
    public float x0, x1;

    public Vector3 startPoint = new Vector3(-0.0f, 0.0f, 0.0f);
    public Vector3 endPoint = new Vector3(-2.0f, 2.0f, 0.0f);
    public Vector3 startTangent = Vector3.zero;
    public Vector3 endTangent = Vector3.zero;
    public int numIntervals = 11;
    [HideInInspector]
    public float sqErr = 4.0f;

    public Vector2[] cubicPoints;

    /// <summary>
    /// Compute a bezier curve going through all the points on a curve defined by a cubic equation. 
    /// The curve has a start point at x0 and an end point at x1
    /// </summary>
    public Bezier GetBezier(float a, float b, float c, float d, float x0, float x1)
    {
        float y0 = GetY(x0, a, b, c, d);
        float y1 = GetY(x1, a, b, c, d);

        /// for simplicity, assign y value to z to see the curve from TopDown view in Unity Inspector
        Vector3 p0 = new Vector3(x0, 0, y0);
        Vector3 p3 = new Vector3(x1, 0, y1);

        /// for demo purposes, use p0 and p3 for p1 and p2
        
        //Calculate points from cubic equation between x0 and x1
        cubicPoints = new Vector2[numIntervals];
        float interVal = (x1 - x0) / (numIntervals - 1.0f);
        for (int i = 0; i < numIntervals; i++)
        {
            cubicPoints[i] = new Vector2(x0 + i * interVal, GetY(x0 + i * interVal, a, b, c, d));
            Handles.Label(new Vector3(cubicPoints[i].x, 0, cubicPoints[i].y), "p");
        }


        //Estimate p1 and p2 based on Least Fit Approx fitting on 1/3 of beginning and end of cubic points
        //using estimated p1 and p2, fit the Bezier values, onto the cubic 
        BezierCalculations.BezierCurve bezCurve = BezierCalculations.FitCurve(cubicPoints, cubicPoints.Length, sqErr);
        startPoint = new Vector3(bezCurve.v0.x, 0.0f, bezCurve.v0.y);
        startTangent = new Vector3(bezCurve.v1.x, 0.0f, bezCurve.v1.y);
        endTangent = new Vector3(bezCurve.v2.x, 0.0f, bezCurve.v2.y);
        endPoint = new Vector3(bezCurve.v3.x, 0.0f, bezCurve.v3.y);
        return new Bezier(startPoint, startTangent, endTangent, endPoint);
    }

    float GetY(float x, float a, float b, float c, float d)
    {
        float x2 = x * x;
        float x3 = x2 * x;

        return a * x3 + b * x2 + c * x + d;
    }

}

[CustomEditor(typeof(CubicEquation))]
public class MathQuestionEditor: Editor
{
    /// <summary>
    /// For more info on OnSceneGUI and Handles.DrawBezier visit this link: https://docs.unity3d.com/ScriptReference/Handles.DrawBezier.html
    /// </summary>
    private void OnSceneGUI()
    {
        CubicEquation equation = target as CubicEquation;

        Bezier bezier = equation.GetBezier(equation.a, equation.b, equation.c, equation.d, equation.x0, equation.x1);

        Handles.DrawBezier(bezier.p0, bezier.p3, bezier.p1, bezier.p2, Color.yellow, null, 2);
       
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CubicEquation equation = target as CubicEquation;
        equation.sqErr = EditorGUILayout.Slider(new GUIContent("Squared Error"),equation.sqErr, 1.0F, 10.0F);
    }

}

public struct Bezier
{
    public Vector3 p0, p1, p2, p3;

    public Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p0 = p0;
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }
}