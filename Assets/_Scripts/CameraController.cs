using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public Vector3 targetPos;
    private Vector3 xyDampVelocity;
    private float zDampVelocity;
    [Range(0, 0.3f)]
    public float smoothTime = 0.1f;

    [Header("Drag")]
    [Range(0, 1.0f)]
    public float dragSpeed = 0.2f;

    [Header("Zoom")]
    private float zoom;
    [Range(0, 1.0f)]
    public float zoomSpeed = 0.5f;
    public float minZoom = 20.0f;
    public float maxZoom = 100.0f;

    [Header("Pan")]
    [Range(0, 1.0f)]
    public float panSpeed = 0.2f;

    //public float radius = 30.0f;

    void Awake()
    {
        targetPos = transform.position;
        targetPos.z = Camera.main.orthographicSize;
    }

    void Update()
    {

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Vector3 cursorPos = GetWorldPositionOnPlane(Input.mousePosition);
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                cursorPos.z = 0;
            }
            else
            {
                cursorPos.x = 2.0f * targetPos.x - cursorPos.x;
                cursorPos.y = 2.0f * targetPos.y - cursorPos.y;
                cursorPos.z = 2.0f * targetPos.z;
            }
            Vector3 newTargetPos = Vector3.Lerp(targetPos, cursorPos, 0.5f * zoomSpeed);
            if (Mathf.Abs(newTargetPos.z) < minZoom)
            {
                if (Mathf.Abs(targetPos.z) > minZoom)
                {
                    targetPos.x = newTargetPos.x;
                    targetPos.y = newTargetPos.y;
                }
                targetPos.z = minZoom;
            }
            else if (Mathf.Abs(newTargetPos.z) > maxZoom)
            {
                if (Mathf.Abs(targetPos.z) < maxZoom)
                {
                    targetPos.x = newTargetPos.x;
                    targetPos.y = newTargetPos.y;
                }
                targetPos.z = maxZoom;
            }
            else
            {
                targetPos = newTargetPos;
            }
        }

        if (Input.GetAxis("Fire1") != 0)
        {
            float speed = 10.0f * dragSpeed * ((targetPos.z - minZoom) / (maxZoom - minZoom) + 0.5f);
            float x = Input.GetAxis("Mouse X") * speed;
            float y = Input.GetAxis("Mouse Y") * speed;
            targetPos = new Vector3(targetPos.x - x, targetPos.y - y, targetPos.z);
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            float speed = 2.0f * panSpeed * ((targetPos.z - minZoom) / (maxZoom - minZoom) + 0.5f);
            transform.position = new Vector3(transform.position.x + Input.GetAxis("Horizontal") * speed,
                transform.position.y + Input.GetAxis("Vertical") * speed, transform.position.z);
            targetPos = new Vector3(transform.position.x, transform.position.y, targetPos.z);
        }



        /*
        if (Mathf.Sqrt(targetPos.x * targetPos.x + targetPos.y * targetPos.y) > radius)
        {
            float angle = Mathf.Atan2(targetPos.y, targetPos.x);
            targetPos.x = Mathf.Cos(angle) * radius;
            targetPos.y = Mathf.Sin(angle) * radius;
        }
        */

        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(targetPos.x, targetPos.y, transform.position.z), ref xyDampVelocity, smoothTime);

        targetPos = new Vector3(Mathf.Clamp(targetPos.x, -GameManager.singleton.mapWidth / 2.0f, GameManager.singleton.mapWidth / 2.0f),
            Mathf.Clamp(targetPos.y, -GameManager.singleton.mapHeight / 2.0f, GameManager.singleton.mapHeight / 2.0f), targetPos.z);
        
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(targetPos.x, targetPos.y, transform.position.z), ref xyDampVelocity, smoothTime);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -GameManager.singleton.mapWidth / 2.0f, GameManager.singleton.mapWidth / 2.0f),
            Mathf.Clamp(transform.position.y, -GameManager.singleton.mapHeight / 2.0f, GameManager.singleton.mapHeight / 2.0f), transform.position.z);

        Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, targetPos.z, ref zDampVelocity, smoothTime);

        foreach (Starfield starfield in GetComponentsInChildren<Starfield>())
        {
            starfield.transform.localScale = 2.0f * Vector3.one * Camera.main.orthographicSize / starfield.halfHeight;
        }
    }

    /// <summary>
    /// Gets the world position based on screen position.
    /// </summary>
    /// <returns>The world position on plane.</returns>
    /// <param name="screenPosition">Screen position.</param>
    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition) {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
}

/*
* /// Perspective camera version ///

        if (mode == viewMode.starSystem)
        {
            if (Input.GetAxis("Fire1") != 0)
            {
                float x = 5.0f * Input.GetAxis("Mouse X") * dragSpeed * -(targetPos.z + minZoom) / (maxZoom - minZoom);
                float y = 5.0f * Input.GetAxis("Mouse Y") * dragSpeed * -(targetPos.z + minZoom) / (maxZoom - minZoom);
                targetPos = new Vector3(targetPos.x - x, targetPos.y - y, targetPos.z);
            }
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                Vector3 cursorPos = GetWorldPositionOnPlane(Input.mousePosition);
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    cursorPos.z = 0;
                }
                else
                {
                    cursorPos.x = 2.0f * targetPos.x - cursorPos.x;
                    cursorPos.y = 2.0f * targetPos.y - cursorPos.y;
                    cursorPos.z = 2.0f * targetPos.z;
                }
                Vector3 newTargetPos = Vector3.Lerp(targetPos, cursorPos, 0.2f * zoomSpeed);
                if (Mathf.Abs(newTargetPos.z) < minZoom)
                {
                    if (Mathf.Abs(targetPos.z) > minZoom)
                    {
                        targetPos = newTargetPos;
                    }
                    targetPos.z = -minZoom;
                }
                else if (Mathf.Abs(newTargetPos.z) > maxZoom)
                {
                    if (Mathf.Abs(targetPos.z) < maxZoom)
                    {
                        targetPos = newTargetPos;
                    }
                    targetPos.z = -maxZoom;
                }
                else
                {
                    targetPos = newTargetPos;
                }
            }
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                targetPos = new Vector3(targetPos.x + Input.GetAxis("Horizontal") * panSpeed, targetPos.y + Input.GetAxis("Vertical") * panSpeed, targetPos.z);
            }
        }
        else if (mode == viewMode.planet)
        {
        }

        if (Mathf.Sqrt(targetPos.x * targetPos.x + targetPos.y * targetPos.y) > currentStarSystem.radius)
        {
            float angle = Mathf.Atan2(targetPos.y, targetPos.x);
            targetPos.x = Mathf.Cos(angle) * currentStarSystem.radius;
            targetPos.y = Mathf.Sin(angle) * currentStarSystem.radius;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref targetVelocity, smoothTime);
        */
