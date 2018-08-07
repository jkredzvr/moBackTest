using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCalculations : MonoBehaviour {

    // Use this for initialization
    void Start() {
        Vector2[] z = {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(0.0f, 2.0f),
            new Vector2(0.0f, 3.0f),
            new Vector2(0.0f, 4.0f),
            new Vector2(0.0f, 5.0f),
            new Vector2(0.0f, 6.0f),
            new Vector2(0.0f, 7.0f),
            new Vector2(1.0f, 8.0f),
            new Vector2(0.0f, 9.0f),
            new Vector2(0.0f, 10.0f)
        };
        float[] ans = ChordLengthParameterize(z, 0, 10);
        Debug.Log(ans);
        foreach (float i in ans)
        {
            Debug.Log(i);
        }
        float error = 4.0f;     /*  Squared error */
        FitCurve(z, 11, error);		/*  Fit the Bezier curves */
    }

    // Update is called once per frame
    void Update() {

    }

    /*
     *  ComputeMaxError :
     *	Find the maximum squared distance of digitized points
     *	to fitted curve.
    */
    static float ComputeMaxError(Vector2[] d, int first, int last, BezierCurve bezCurve, float[] u, ref int splitPoint)
    {
        int i;
        float maxDist;		/*  Maximum error		*/
        float dist;		/*  Current error		*/
        Vector2 P;			/*  Point on curve		*/
        Vector2 v;			/*  Vector from point to curve	*/
        splitPoint = (last - first + 1)/2;
        maxDist = 0.0f;
        for (i = first + 1; i<last; i++) {
		    P = BezierII(3, bezCurve, u[i - first]);
            v = P - d[i];
            dist = v.sqrMagnitude;
		    if (dist >= maxDist) {
	    	    maxDist = dist;
	    	    splitPoint = i;
		    }
        }
        return (maxDist);
    }

    /*
     *  Bezier :
     *  	Evaluate a Bezier curve at a particular parameter value
     *  	int degree;		The degree of the bezier curve	
     *      BezierCurve V   Array of control points		
     *      float t		 Parametric value to find point for	
     * 
    */
    static Vector2 BezierII(int degree, BezierCurve V, float t)
    {
        int i, j;
        Vector2 Q;	        /* Point on curve at parameter t	*/
        Vector2[] Vtemp;		/* Local copy of control points		*/

        /* Copy array	*/
        Vtemp = new Vector2[degree + 1];
        if (degree == 3)
        {
            Vtemp[0] = V.v0;
            Vtemp[1] = V.v1;
            Vtemp[2] = V.v2;
            Vtemp[3] = V.v3;
        }
        if (degree == 2)
        {
            Vtemp[0] = V.v0;
            Vtemp[1] = V.v1;
            Vtemp[2] = V.v2;
        }
        if (degree == 1)
        {
            Vtemp[0] = V.v0;
            Vtemp[1] = V.v1;
        }



        /* Triangle computation	*/
        for (i = 1; i <= degree; i++) {	
		    for (j = 0; j <= degree-i; j++) {
	    	    Vtemp[j].x = (1.0f - t) * Vtemp[j].x + t* Vtemp[j + 1].x;
	    	    Vtemp[j].y = (1.0f - t) * Vtemp[j].y + t* Vtemp[j + 1].y;
		    }
        }

        Q = Vtemp[0];
        return Q;
    }

    const int MAXPOINTS = 1000;
    /*
       FitCurve :
   	    Fit a Bezier curve to a set of digitized points 
        Vector2 d;			  Array of digitized points	
        int nPts;		      Number of digitized points	
        double error;		  User-defined error squared
    */
    public static BezierCurve FitCurve(Vector2[] d, int nPts, float error)
    {
        Vector2 tHat1, tHat2;	/*  Unit tangent vectors at endpoints */

        tHat1 = ComputeLeftTangent(d, 0);
        tHat2 = ComputeRightTangent(d, nPts - 1);
        return FitCubic(d, 0, nPts - 1, tHat1, tHat2, error);
    }
    
    static float[] parametizer(Vector2[] points)
    {
        float numIntervals = points.Length;
        float x0 = points[0].x;
        float interVal = ( - points[points.Length-1].x) / (numIntervals - 1.0f);
        //cubicPoints[0] = new Vector2(x0, y0);
        float[] paramValues = new float[points.Length];
        for (int i = 0; i < numIntervals; i++)
        {
            paramValues[i] = x0 + i * interVal;
        }
        return paramValues;
    }

    /*
     *  FitCubic :
     *  	Fit a Bezier curve to a (sub)set of digitized points
     *      float error;		  User-defined error squared	   
    */
    static BezierCurve FitCubic(Vector2[] d, int first, int last, Vector2 tHat1, Vector2 tHat2, float error) {
        BezierCurve bezCurve; /*Control points of fitted Bezier curve*/
        float[] u;		/*  Parameter values for point  */
        float[] uPrime;	/*  Improved parameter values */
        float maxError;	/*  Maximum fitting error	 */
        int splitPoint=0;	/*  Point to split point set at	 */
        int nPts;		/*  Number of points in subset  */
        float iterationError; /*Error below which you try iterating  */
        int maxIterations = 30; /*  Max times to try iterating  */
        Vector2 tHatCenter;   	/* Unit tangent vector at splitPoint */
        int i;

        iterationError = error * error;
        nPts = last - first + 1;

        /*  Parameterize points, and attempt to fit curve */

        //u = parametizer(d);
        u = ChordLengthParameterize(d, first, last);
        bezCurve = GenerateBezier(d, first, last, u, tHat1, tHat2);

        /*  Find max deviation of points to fitted curve */
        maxError = ComputeMaxError(d, first, last, bezCurve, u, ref splitPoint);
        if (maxError < error) {
            //Draw bezier curve method
            //Debug.Log("v0" + bezCurve.v0+ "\n");

            //Debug.Log("v1" + bezCurve.v1 + "\n");

            //Debug.Log("v2" + bezCurve.v2 + "\n");

            //Debug.Log("v3" + bezCurve.v3 + "\n");


            return bezCurve;
        }

        

        /*  If error not too large, try some reparameterization  */
        /*  and iteration */
        if (maxError < iterationError) {
            
            for (i = 0; i < maxIterations; i++) {
                Debug.Log("iterating times: " + i);
                uPrime = Reparameterize(d, first, last, u, bezCurve);
                /*
                Debug.Log("before v0" + bezCurve.v0+ "\n");
                Debug.Log("before v1" + bezCurve.v1 + "\n");
                Debug.Log("before v2" + bezCurve.v2 + "\n");
                Debug.Log("before v3" + bezCurve.v3 + "\n");
                */
                bezCurve = GenerateBezier(d, first, last, uPrime, tHat1, tHat2);
                /*
                Debug.Log("after v0" + bezCurve.v0 + "\n");
                Debug.Log("after v1" + bezCurve.v1 + "\n");
                Debug.Log("after v2" + bezCurve.v2 + "\n");
                Debug.Log("after v3" + bezCurve.v3 + "\n");
                */
                maxError = ComputeMaxError(d, first, last, bezCurve, uPrime, ref splitPoint);
                if (maxError < error) {
                    return bezCurve;
                }
                u = uPrime;
            }

        }

    Debug.Log("shouldnt get to here2");
    return new BezierCurve(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
}

/*
*  Reparameterize:
*	Given set of points and their parameterization, try to find
*   a better parameterization.
*
*/
                static float[] Reparameterize(Vector2[] d, int first, int last, float[] u, BezierCurve bezCurve)
    {
        int nPts = last - first + 1;
        int i;
        float[] uPrime;		/*  New parameter values	*/

        uPrime = new float[nPts];
        for (i = first; i <= last; i++) {
		    uPrime[i - first] = NewtonRaphsonRootFind(bezCurve, d[i], u[i - first]);
        }
        return (uPrime);
    }



    /*
     *  NewtonRaphsonRootFind :
     *	Use Newton-Raphson iteration to find better root.
     */
    static float NewtonRaphsonRootFind(BezierCurve Q, Vector2 P, float u)
    {
        float numerator, denominator;
        Vector2[] Q1 = new Vector2[3];    /*  Q' and Q''			*/
        Vector2[] Q2 = new Vector2[2];
        Vector2 Q_u, Q1_u, Q2_u; /*u evaluated at Q, Q', & Q''	*/
        float uPrime;      /*  Improved u			*/
        int i;

        /* Compute Q(u)	*/
        Q_u = BezierII(3, Q, u);
    
        /* Generate control vertices for Q'	*/
        for (i = 0; i <= 2; i++) {
            if(i+1 == 1)
            {
                Q1[i].x = (Q.v1.x - Q.v0.x) * 3.0f;
                Q1[i].y = (Q.v1.y - Q.v0.y) * 3.0f;
            }
            if (i + 1 == 2)
            {
                Q1[i].x = (Q.v2.x - Q.v1.x) * 3.0f;
                Q1[i].y = (Q.v2.y - Q.v1.y) * 3.0f;
            }
            if (i + 1 == 3)
            {
                Q1[i].x = (Q.v3.x - Q.v2.x) * 3.0f;
                Q1[i].y = (Q.v3.y - Q.v2.y) * 3.0f;
            }
        }
    
        /* Generate control vertices for Q'' */
        for (i = 0; i <= 1; i++) {
		    Q2[i].x = (Q1[i + 1].x - Q1[i].x) * 2.0f;
		    Q2[i].y = (Q1[i + 1].y - Q1[i].y) * 2.0f;
        }
    
        /* Compute Q'(u) and Q''(u)	*/
        Q1_u = BezierII(2, new BezierCurve(Q1[0],Q1[1],Q1[2],new Vector2(0,0)) , u);
        Q2_u = BezierII(1, new BezierCurve(Q2[0], Q2[1], new Vector2(0, 0), new Vector2(0, 0)), u);

        /* Compute f(u)/f'(u) */
        numerator = (Q_u.x - P.x) * (Q1_u.x) + (Q_u.y - P.y) * (Q1_u.y);
        denominator = (Q1_u.x) * (Q1_u.x) + (Q1_u.y) * (Q1_u.y) + (Q_u.x - P.x) * (Q2_u.x) + (Q_u.y - P.y) * (Q2_u.y);
        if (denominator == 0.0f) return u;
        /* u = u - f(u)/f'(u) */
        uPrime = u - (numerator/denominator);
        return (uPrime);
    }


    public struct BezierCurve {
        public Vector2 v0;
        public Vector2 v1;
        public Vector2 v2;
        public Vector2 v3;
        public BezierCurve(Vector2 pt0, Vector2 pt1, Vector2 pt2, Vector2 pt3){
            v0 = pt0;
            v1 = pt1;
            v2 = pt2;
            v3 = pt3;
        }
    }
    
    /*
     *  GenerateBezier :
     *  Use least-squares method to find Bezier control points for region.
     *  //uPrime, param values for the curve between [0,1]
     */
    static BezierCurve GenerateBezier(Vector2[] d, int first, int last, float[] uPrime, Vector2 tHat1, Vector2 tHat2)
    {
        int i;
        Vector2[,] A = new Vector2 [MAXPOINTS,2];	/* Precomputed rhs for eqn	*/
        int nPts;           /* Number of pts in sub-curve */
        float[,] C = new float[2, 2]; /* Matrix C		*/
        float[] X = new float[2];	  /* Matrix X		*/
        float det_C0_C1,       /* Determinants of matrices	*/
                det_C0_X,
                   det_X_C1;
        float alpha_l,     /* Alpha values, left and right	*/
                alpha_r;
        Vector2 tmp;            /* Utility variable		*/
        BezierCurve bezCurve;   /* RETURN bezier curve ctl pts	*/
        float segLength;
        float epsilon;
        
        nPts = last - first + 1;

 
        /* Compute the A's	*/
        for (i = 0; i<nPts; i++) {
		    Vector2 v1, v2;
            v1 = tHat1;
		    v2 = tHat2;
            A[i,0] = v1* B1(uPrime[i]);
		    A[i,1] = v2* B2(uPrime[i]);
        }

        /* Create the C and X matrices	*/
        C[0,0] = 0.0f;
        C[0,1] = 0.0f;
        C[1,0] = 0.0f;
        C[1,1] = 0.0f;
        X[0]   = 0.0f;
        X[1]   = 0.0f;

        for (i = 0; i<nPts; i++) {
            C[0,0] += Vector2.Dot(A[i,0], A[i,0]);
            C[0,1] += Vector2.Dot(A[i,0], A[i,1]);
            /*C[1][0] += V2Dot(&A[i][0], &A[i][1]);*/
            C[1,0] = C[0,1];
		    C[1,1] += Vector2.Dot(A[i,1], A[i,1]);

            tmp = d[first + i] - (d[first] * B0(uPrime[i]) + d[first] * B1(uPrime[i]) + d[last] * B2(uPrime[i]) + d[last] * B3(uPrime[i]));
                
               
	
            //Right hand side eq on pg 619  
	        X[0] += Vector2.Dot(A[i,0], tmp);
            X[1] += Vector2.Dot(A[i,1], tmp);
        }

        /* Compute the determinants of C and X	*/
        det_C0_C1 = C[0,0] * C[1,1] - C[1,0] * C[0,1];
        det_C0_X  = C[0,0] * X[1]    - C[1,0] * X[0];
        det_X_C1  = X[0]   * C[1,1] - X[1]    * C[0,1];

        /* Finally, derive alpha values	*/
        alpha_l = (det_C0_C1 == 0.0f) ? 0.0f : det_X_C1 / det_C0_C1;
        alpha_r = (det_C0_C1 == 0.0f) ? 0.0f : det_C0_X / det_C0_C1;

        /* If alpha negative, use the Wu/Barsky heuristic (see text) */
        /* (if alpha is 0, you get coincident control points that lead to
         * divide by zero in any subsequent NewtonRaphsonRootFind() call. */
        segLength = Vector2.Distance(d[last], d[first]);
        epsilon = Mathf.Pow(10.0f,-6.0f) * segLength;

        if (alpha_l<epsilon || alpha_r<epsilon)
        {
		    /* fall back on standard (probably inaccurate) formula, and subdivide further if needed. */
		    float dist = segLength / 3.0f;
            return (new BezierCurve(d[first], d[first] + (tHat1 * dist), d[last] + tHat2 * dist, d[last]));
        }

        /*  First and last control points of the Bezier curve are */
        /*  positioned exactly at the first and last data points */
        /*  Control points 1 and 2 are positioned an alpha distance out */
        /*  on the tangent vectors, left and right, respectively */
        Vector2 newV1 = d[first] + tHat1 * alpha_l;
        Vector2 newV2 = d[last] + tHat2 * alpha_r;
        return ( new BezierCurve(d[first], newV1, newV2, d[last]) );
    }


   
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
            //Debug.Log(u[i - first]);
        }

        for (i = first + 1; i <= last; i++) {
		    u[i - first] = u[i - first] / u[last - first];
            //Debug.Log(u[i - first]);
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

    /*
     *  B0, B1, B2, B3 :
     *	Bezier multipliers
    */
    static float B0(float u)
    {
        float tmp = 1.0f - u;
        return (tmp * tmp * tmp);
    }


    static float B1(float u)
    {
        float tmp = 1.0f - u;
        return (3 * u * (tmp * tmp));
    }

    static float B2(float u)
    {
        float tmp = 1.0f - u;
        return (3 * u * u * tmp);
    }

    static float B3(float u)
    {
        return (u * u * u);
    }

}

