using UnityEngine;
using System.Collections.Generic;
using Vectrosity;

public class Orbit : MonoBehaviour
{
    /// <summary>
    /// One of two focuses of an elliptical orbit.
    /// </summary>
    public Vector2 primary;
    /// <summary>
    /// Closest position to primary.
    /// </summary>
    public Vector2 periapsis;
    /// <summary>
    /// Angle from zero to periapsis.
    /// </summary>
    public float periapsisAngle;
    /// <summary>
    /// Ratio of primary distance to semi-major axis distance. 0 = perfect circle, 1 = parabola.
    /// </summary>
    public float eccentricity;
    /// <summary>
    /// Angle relative to primary and periapsis.
    /// </summary>
    public float currentAngle;
    public float targetAngle;
    /// <summary>
    /// Distance from center to periapsis.
    /// </summary>
    public float semiMajorAxis;
    public float c;

    private float orbitDampVelocity;
    public static float orbitDampSmoothTime = 0.2f;

    /// <summary>
    /// Arbitrary time unit used to solve Kepler equation.
    /// </summary>
    public float currentTime = 0.0f;
    //public static float turnTime = 1.0f;

    public VectorLine orbitLine = null;
    public Color32 orbitColor;

    /// <summary>
    /// Time (in game turns) to complete one orbit. 0 = no orbit.
    /// </summary>
    public float period;

    public void Initialize(Vector2 _primary, Vector2 _periapsis, float _eccentricity, float _period, float _currentAngle)
    {
        primary = _primary;
        periapsis = _periapsis;
        eccentricity = _eccentricity;
        period = _period;
        currentAngle = _currentAngle;
        currentTime = GetTime(currentAngle);
        targetAngle = currentAngle;
        periapsisAngle = Mathf.Atan2(periapsis.y - primary.y, periapsis.x - primary.x);
        float b = Vector2.Distance(primary, periapsis);
        semiMajorAxis = (eccentricity * b) / (1 - eccentricity) + b;
        c = 1 - eccentricity * eccentricity;
        transform.position = GetPosition(currentAngle);
        RotateSprites();
        //Debug.Log(gameObject.name + " " + GetAngle(2.0f));
        //UpdateLineWidths();
    }

    void Start()
    {
        if (period != 0)
        {
            DrawOrbitLine();
        }
    }

    public void EndTurn()
    {
        if (period != 0)
        {
            currentTime += GameManager.singleton.turnTime;
            //targetAngle = GetAngle(currentTime);

            float newAngle = GetAngle(currentTime);
            if (newAngle < 0)
            {
                newAngle += 2.0f * Mathf.PI;
            }
            targetAngle = newAngle + 2.0f * Mathf.PI * Mathf.Floor(currentTime / period);
        }
    }

    void Update()
    {
        if (Mathf.Abs(currentAngle - targetAngle) > 0.0001f)
        {
            currentAngle = Mathf.SmoothDamp(currentAngle, targetAngle, ref orbitDampVelocity, orbitDampSmoothTime);
            Vector3 pos = GetPosition(currentAngle);
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            RotateSprites();
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Rad2Deg * (currentAngle + periapsisAngle));
            if (period != 0)
            {
                orbitLine.SetWidths(GetLineWidths());
                orbitLine.Draw3D();
            }
        }
    }

    void OnDestroy()
    {
        VectorLine.Destroy(ref orbitLine);
    }

    private void RotateSprites()
    {
        Transform body = transform.Find("Body");
        Transform shadow = transform.Find("Shadow");
        if (body != null)
        {
            if (body.gameObject.GetComponent<SpriteRenderer>() != null)
                body.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Rad2Deg * (currentAngle + periapsisAngle));
        }
        if (shadow != null)
        {
            if (shadow.gameObject.GetComponent<SpriteRenderer>() != null)
                shadow.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Rad2Deg * (currentAngle + periapsisAngle) - 90.0f);
        }
    }

    private Vector3 GetPosition(float angle)
    {
        
        float distanceFromPrimary = semiMajorAxis * (c / (1 + eccentricity * Mathf.Cos(angle)));
        return Quaternion.Euler(0, 0, periapsisAngle * Mathf.Rad2Deg) * (new Vector3((primary.x + Mathf.Cos(angle) * distanceFromPrimary),
            (primary.y + Mathf.Sin(angle) * distanceFromPrimary), 0));
    }

    private float GetTime(float angle)
    {
        float eccentricAnomaly = Mathf.Atan2(Mathf.Sqrt(1 - eccentricity * eccentricity) * Mathf.Sin(angle), eccentricity + Mathf.Cos(angle));
        float time = (period / (2.0f * Mathf.PI)) * (eccentricAnomaly - eccentricity * Mathf.Sin(eccentricAnomaly));
        return time;
    }

    private float GetAngle(float time)
    {
        if (period != 0)
        {
            
            //float meanAnomaly = 2 * Mathf.PI * time / period;
            // Inverse Kepler equation
            float eccentricAnomaly = SolveKeplerEquation(time);
            float trueAnomaly = 2.0f * Mathf.Atan2(Mathf.Sqrt(1 + eccentricity) * Mathf.Sin(eccentricAnomaly / 2.0f),
                                    Mathf.Sqrt(1 - eccentricity) * Mathf.Cos(eccentricAnomaly / 2.0f));
            return trueAnomaly;
        }
        else
        {
            return currentAngle;
        }
    }

    private float SolveKeplerEquation(float t)
    {
        int maxIterations = 15;
        float accuracy = 0.001f;
        float M = 2.0f * Mathf.PI * t / period;
        float E = M;
        float e = eccentricity;
        float f;
        for (int i = 0; i < maxIterations; i++)
        {
            f = E - e * Mathf.Sin(E) - M;
            if (f < accuracy)
            {
                break;
            }
            E = E - f / (1 - e * Mathf.Cos(E));
        }
        return E;
    }

    private void DrawOrbitLine()
    {
        /*
        float angle = Mathf.Atan2(primary.y - periapsis.y, primary.x - periapsis.x);
        float a = Vector2.Distance(primary, periapsis) / (1 - eccentricity);
        float b = Mathf.Sqrt(a * a * (1 - eccentricity * eccentricity));
        Vector3 orbitOrigin = new Vector3(periapsis.x + a * Mathf.Cos(angle), periapsis.y + a * Mathf.Sin(angle), 10);
        VectorLine orbitLine = new VectorLine(gameObject.name + " Orbit", new List<Vector3>(GameManager.singleton.orbitLineSegments + 1), 
            Resources.Load<Texture>("thinline"), 5.0f, LineType.Continuous);
        

        orbitLine.MakeEllipse(orbitOrigin, a, b, 180, pointRotation: angle);
        orbitLine.SetColor(new Color32(255, 255, 255, 64));
        orbitLine.Draw3D();
        */
        orbitLine = new VectorLine(gameObject.name + " Orbit", GetLineVertices(), Resources.Load<Texture>("thinline"), 5.0f, LineType.Continuous);
        orbitLine.SetColor(orbitColor);
        orbitLine.SetWidths(GetLineWidths());
        orbitLine.Draw3D();
        //List<float> lineWidths = new List<float>();
    }

    private List<Vector3> GetLineVertices()
    {
        List<Vector3> lineVertices = new List<Vector3>();
        for (int i = 0; i < GameManager.singleton.orbitLineSegments + 1; i++)
        {
            lineVertices.Add(GetPosition(2.0f * Mathf.PI * (float)i / (float)GameManager.singleton.orbitLineSegments) + new Vector3(0, 0, 5));
        }
        return lineVertices;
    }

    private List<float> GetLineWidths()
    {
        float[] lineWidths = new float[GameManager.singleton.orbitLineSegments];
        int startSegment = (int)(GameManager.singleton.orbitLineSegments * (Mathf.Repeat(currentAngle, 2.0f * Mathf.PI) / (2.0f * Mathf.PI)));
        for (int i = 0; i < GameManager.singleton.orbitLineSegments; i++)
        {
            int index = (startSegment + i) % GameManager.singleton.orbitLineSegments;
            lineWidths[index] = GameManager.singleton.orbitLineWidth * (float)i / (float)GameManager.singleton.orbitLineSegments;
        }
        return new List<float>(lineWidths);
    }

    public void HohmannTransfer(Orbit target)
    {
        if (target.primary != primary)
        {
            Debug.Log("Orbits have different primaries. Cannot calculate Hohmann transfer.");
            return;
        }
        Vector3 transferApsis = target.GetPosition(currentAngle + Mathf.PI + periapsisAngle - target.periapsisAngle);    //apsis opposite current position
        //Debug.Log("Apsis " + transferApsis);
        Vector2 transferCenter = (Vector2)(transferApsis + transform.position) / 2.0f;
        //Debug.Log("Center " + transferCenter);
        float transferEccentricity = Vector2.Distance(transferCenter, primary) / Vector2.Distance(transferCenter, (Vector2)transform.position);
        //float transferC = Vector2.Distance(transferCenter, primary);
        //float transferSemiMajorAxis = Vector2.Distance(transferCenter, (Vector2)transform.position);
        //float transferPeriod = (period + target.period) / 2.0f;
        Vector2 transferPeriapsis;
        float transferAngle;
        if (Vector2.Distance((Vector2)transform.position, primary) < Vector2.Distance(transferApsis, primary))
        {
            transferPeriapsis = transform.position;
            transferAngle = 0.0f;
            //Debug.Log(transferStartingAngle);
        }
        else
        {
            //Debug.Log("inward at " + transferApsis);
            transferPeriapsis = transferApsis;
            transferAngle = Mathf.PI;
            //Debug.Log(transferPeriapsis);
        }
        //GameObject obj = new GameObject();
        //obj.SetActive(false);
        //Orbit transferOrbit = obj.AddComponent<Orbit>();
        VectorLine.Destroy(ref orbitLine);
        Initialize(primary, transferPeriapsis, transferEccentricity, period, transferAngle);
        currentTime = -currentTime;
        DrawOrbitLine();
        //obj.SetActive(true);
    }
}
