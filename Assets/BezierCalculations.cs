using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCalculations : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector2[] z = {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(0.0f, 2.0f),
            new Vector2(0.0f, 3.0f),
            new Vector2(0.0f, 4.0f),
            new Vector2(0.0f, 5.0f),
            new Vector2(0.0f, 6.0f),
            new Vector2(0.0f, 6.5f),
            new Vector2(1.0f, 7.0f),
            new Vector2(0.0f, 9.0f),
            new Vector2(0.0f, 10.0f)
        };
        float[] ans = ChordLengthParameterize(z,0, 10);
        Debug.Log(ans);
        foreach (float i in ans)
        {
            Debug.Log(i);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /*
   FitCurve :
   	Fit a Bezier curve to a set of digitized points 
    Vector2 d;			  Array of digitized points	
    int nPts;		      Number of digitized points	
    double error;		  User-defined error squared
 */
    void FitCurve(Vector2[] d, int nPts, float error) 
    {
        Vector2 tHat1, tHat2;	/*  Unit tangent vectors at endpoints */

        tHat1 = ComputeLeftTangent(d, 0);
        tHat2 = ComputeRightTangent(d, nPts - 1);
        //FitCubic(d, 0, nPts - 1, tHat1, tHat2, error);
    }

    /*
     *  FitCubic :
     *  	Fit a Bezier curve to a (sub)set of digitized points
    */


   
    /*
     *  ChordLengthParameterize :
     *	Assign parameter values to digitized points 
     *	using relative distances between points.
     */
    static float[] ChordLengthParameterize(Vector2[] d, int first, int last)
    {
        int i;
        float[] u = new float[last - first+1];
        

        u[0] = 0.0f;
        for (i = first+1; i <= last; i++) {
            //u[next index] = u[prev index] + Distance between next two points
		    u[i - first] = u[i - first - 1] + Vector2.Distance(d[i],d[i-1]);
            Debug.Log(u[i - first]);
        }

        for (i = first + 1; i <= last; i++) {
		    u[i - first] = u[i - first] / u[last - first];
            Debug.Log(u[i - first]);
        }

        return(u);
    }
    /* 
     * ComputeLeftTangent, ComputeRightTangent, ComputeCenterTangent :
     * Approximate unit tangents at endpoints and "center" of digitized curve
     * Point2* d;			  Digitized points
     * int end;		          Index to "left" end of region 
     */
    static Vector2 ComputeLeftTangent(Vector2[] d, int end)
    {
        Vector2 tHat1;
        tHat1 = d[end + 1] - d[end];
        tHat1 = tHat1.normalized; 
        return tHat1;
    }

    static Vector2 ComputeRightTangent(Vector2[] d, int end)
    {
        Vector2 tHat2;
        tHat2 = d[end - 1] - d[end];
        tHat2 = tHat2.normalized;
        return tHat2;
    }



}
