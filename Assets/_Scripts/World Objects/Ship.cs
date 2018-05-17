using UnityEngine;
using System.Collections;

public class Ship : WorldObject
{
    public int owner;
    public int location;    // 0 if following its own orbit
    public Vector3 targetPos;

    public void Initialize(int _id, int _owner, int _location)
    {
        id = _id;
        owner = _owner;
        location = _location;
        transform.position = new Vector3(transform.position.x, transform.position.y, -5.0f);
        GetComponent<Orbit>().orbitColor = new Color(1.0f, 0.1f, 0.1f, 0.5f);
        /*
        if (location != 0)
        {
            GetComponent<Orbit>().enabled = false;
            transform.parent = GameManager.singleton.Find(location).transform;
            transform.localPosition = new Vector3(0, 0, -5.0f);
        }
        */
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            GetComponent<Orbit>().HohmannTransfer(GameManager.singleton.Find(5).GetComponent<Orbit>());
        }
        if (Input.GetMouseButtonUp(1))
        {
            GetComponent<Orbit>().HohmannTransfer(GameManager.singleton.Find(4).GetComponent<Orbit>());
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            EnterPlanet(GameManager.singleton.Find(4).GetComponent<Planet>());
        }
    }

    public void EnterPlanet(Planet target)
    {
        Destroy(GetComponent<Orbit>());
        transform.parent = target.transform;
        transform.localPosition = new Vector3(target.size, 0, transform.localPosition.z);
        transform.Find("Body").eulerAngles = new Vector3(0, 0, 90.0f);
        //transform.localPosition;
    }
}
