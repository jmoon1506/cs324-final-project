using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Vectrosity;

/// <summary>
/// Loads game states.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject starPrefab;
    public GameObject planetPrefab;
    public GameObject shipPrefab;

    // Map Parameters
    [Header("Map Parameters")]
    public float mapWidth;
    public float mapHeight;

    // Game Settings
    [Header("Game Settings")]
    public float toolTipDelay = 0.0f;
    public int orbitLineSegments = 180;
    public float orbitLineWidth = 7.0f;
    public float turnTime = 1.0f;

    // World Objects
    private GameObject worldObjects;    // GameObject with parent transform
    public List<Player> players;
    public List<Star> stars;
    public List<Planet> planets;
    public List<Ship> ships;
    public List<Orbit> orbits;

    public void Awake()
    {
        if (this != singleton)
        {
            singleton = this;
        }
    }

    public void LoadMap(string path)
    {
        if (this != singleton)
        {
            singleton = this;
        }
        if (!File.Exists(path))
        {
            Debug.Log("Map file does not exist at: " + path);
        }
        else
        {
            ResetWorldObjects();
            worldObjects = new GameObject("World Objects");
            XmlDocument mapFile = new XmlDocument();
            mapFile.Load(path);
            if (!float.TryParse(mapFile.DocumentElement.Attributes["width"].Value, out mapWidth))
            {
                Debug.Log("Invalid map width at: " + mapFile.DocumentElement.OuterXml);
            }
            if (!float.TryParse(mapFile.DocumentElement.Attributes["height"].Value, out mapHeight))
            {
                Debug.Log("Invalid map height at: " + mapFile.DocumentElement.OuterXml);
            }
            foreach (XmlNode node in mapFile.DocumentElement.ChildNodes)
            {
                GameObject obj;

                // Parse ID attribute
                int objID = 0;
                if (!int.TryParse(node.Attributes["id"].Value, out objID))
                {
                    Debug.Log("Invalid ID for node: " + node.OuterXml);
                    continue;
                }

                // Parse color property
                Color objColor = Color.magenta;
                if (node["color"] != null)
                {
                    if (!ColorUtility.TryParseHtmlString(node["color"].InnerText, out objColor))
                    {
                        Debug.Log("Invalid color for node: " + node.OuterXml);
                        continue;
                    }
                }

                // Parse orbit property
                Vector2 orbitPrimary = new Vector2();
                Vector2 orbitPeriapsis = new Vector2();
                float orbitEccentricity = 0.0f;
                float orbitPeriod = 0.0f;
                float orbitAngle = 0.0f;
                if (node["orbit"] != null)
                {
                    orbitPrimary = new Vector2(float.Parse(node["orbit"]["primary"]["x"].InnerText), float.Parse(node["orbit"]["primary"]["y"].InnerText));
                    orbitPeriapsis = new Vector2(float.Parse(node["orbit"]["periapsis"]["x"].InnerText), float.Parse(node["orbit"]["periapsis"]["y"].InnerText));
                    orbitEccentricity = float.Parse(node["orbit"]["eccentricity"].InnerText);
                    orbitPeriod = float.Parse(node["orbit"]["period"].InnerText);
                    orbitAngle = float.Parse(node["orbit"]["currentAngle"].InnerText);
                }

                // Parse name property
                string name = node["name"].InnerText;

                // Parse type properties
                switch (node.Name)
                {
                    case "player":
                        obj = GameObject.Instantiate(playerPrefab);
                        obj.name = name;
                        obj.transform.parent = worldObjects.transform;
                        Player player = obj.GetComponent<Player>();
                        player.Initialize(objID, objColor);
                        players.Add(player);
                        break;
                    case "star":
                        obj = GameObject.Instantiate(starPrefab);
                        obj.name = name;
                        obj.transform.parent = worldObjects.transform;
                        Star star = obj.GetComponent<Star>();
                        star.Initialize(objID, objColor, float.Parse(node["size"].InnerText));
                        Orbit starOrbit = obj.GetComponent<Orbit>();
                        starOrbit.Initialize(orbitPrimary, orbitPeriapsis, orbitEccentricity, orbitPeriod, orbitAngle);
                        stars.Add(star);
                        orbits.Add(starOrbit);
                        break;
                    case "planet":
                        obj = GameObject.Instantiate(planetPrefab);
                        obj.name = name;
                        obj.transform.parent = worldObjects.transform;
                        Planet planet = obj.GetComponent<Planet>();
                        planet.Initialize(objID, objColor, float.Parse(node["size"].InnerText));
                        Orbit planetOrbit = obj.GetComponent<Orbit>();
                        planetOrbit.Initialize(orbitPrimary, orbitPeriapsis, orbitEccentricity, orbitPeriod, orbitAngle);
                        planets.Add(planet);
                        orbits.Add(planetOrbit);
                        break;
                    case "ship":
                        obj = GameObject.Instantiate(shipPrefab);
                        obj.name = name;
                        obj.transform.parent = worldObjects.transform;
                        Orbit shipOrbit = obj.GetComponent<Orbit>();
                        shipOrbit.Initialize(orbitPrimary, orbitPeriapsis, orbitEccentricity, orbitPeriod, orbitAngle);
                        Ship ship = obj.GetComponent<Ship>();
                        ship.Initialize(objID, int.Parse(node["owner"].InnerText), int.Parse(node["location"].InnerText));
                        ships.Add(ship);
                        orbits.Add(shipOrbit);
                        break;
                }
            }
        }
    }

    private void ResetWorldObjects()
    {
        if (worldObjects != null)
        {
            GameObject.DestroyImmediate(worldObjects);
        }
        if (GameObject.Find("World Objects") != null)
        {
            GameObject.DestroyImmediate(GameObject.Find("World Objects"));
        }
        players = new List<Player>();
        stars = new List<Star>();
        planets = new List<Planet>();
        orbits = new List<Orbit>();
        ships = new List<Ship>();
    }

    /*
    /// <summary>
    /// Creates game objects from XML map file at path.
    /// </summary>
    /// <param name="path">Path.</param>
    public void LoadMap(string path)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Serializable"))
        {
            GameObject.DestroyImmediate(obj);
        }
        players = new List<Player>();
        stars = new List<Star>();
        planets = new List<Planet>();
        orbits = new List<Orbit>();
        ships = new List<Ship>();
        if (File.Exists(path))
        {
            XmlDocument mapFile = new XmlDocument();
            mapFile.Load(path);
            foreach (XmlNode node in mapFile.DocumentElement.ChildNodes)
            {
                GameObject obj = new GameObject(node["name"].InnerText);
                obj.tag = "Serializable";

                // Parse ID attribute
                int objID;
                if (!int.TryParse(node.Attributes["id"].Value, out objID))
                {
                    Debug.Log("Invalid ID for node: " + node.OuterXml);
                    continue;
                }
                // Parse color property
                Color color = Color.magenta;
                if (node["color"] != null)
                {
                    if (!ColorUtility.TryParseHtmlString(node["color"].InnerText, out color))
                    {
                        Debug.Log("Invalid color for node: " + node.OuterXml);
                        continue;
                    }
                }

                // Parse orbit property
                if (node["orbit"] != null)
                {
                    Orbit orbit = obj.AddComponent<Orbit>();
                    orbit.primary = new Vector2(float.Parse(node["orbit"]["primary"]["x"].InnerText), float.Parse(node["orbit"]["primary"]["y"].InnerText));
                    orbit.periapsis = new Vector2(float.Parse(node["orbit"]["periapsis"]["x"].InnerText), float.Parse(node["orbit"]["periapsis"]["y"].InnerText));
                    orbit.eccentricity = float.Parse(node["orbit"]["eccentricity"].InnerText);
                    orbit.period = float.Parse(node["orbit"]["period"].InnerText);
                    orbit.currentAngle = float.Parse(node["orbit"]["currentAngle"].InnerText);
                    orbits.Add(orbit);
                }

                // Parse type properties
                switch (node.Name)
                {
                    case "player":
                        Debug.Log("Player " + node["name"].InnerText);
                        Player player = obj.AddComponent<Player>();
                        player.id = objID;
                        player.color = color;
                        players.Add(player);
                        break;
                    case "star":
                        Debug.Log("Star " + node["name"].InnerText);
                        Star star = obj.AddComponent<Star>();
                        star.id = objID;
                        star.color = color;
                        stars.Add(star);
                        break;
                    case "planet":
                        Debug.Log("Planet " + node["name"].InnerText);
                        Planet planet = obj.AddComponent<Planet>();
                        planet.id = objID;
                        planet.color = color;
                        planets.Add(planet);
                        break;
                    case "ship":
                        Debug.Log("Ship " + node["name"].InnerText);
                        Ship ship = obj.AddComponent<Ship>();
                        ship.id = objID;
                        ships.Add(ship);
                        break;
                }
            }
        }
        else
        {
            Debug.Log("Map file does not exist at: " + path);
        }
    }

    public void SaveMap(string path)
    {
        XmlDocument mapFile = new XmlDocument();
        foreach (Player player in players)
        {
        }
        foreach (Star star in stars)
        {
        }
        foreach (Planet planet in planets)
        {
        }
        mapFile.Save(path);
    }
    */



    public void SetupTest()
    {
        LoadMap(Application.dataPath + "/Data/Maps/Test.xml");
    }

    public void EndTurn()
    {
        foreach (Orbit orbit in orbits)
        {
            orbit.EndTurn();
        }
    }

    public GameObject Find(int _id)
    {
        foreach (Player player in players)
        {
            if (player.id == _id)
                return player.gameObject;
        }
        foreach (Planet planet in planets)
        {
            if (planet.id == _id)
                return planet.gameObject;
        }
        foreach (Star star in stars)
        {
            if (star.id == _id)
                return star.gameObject;
        }
        foreach (Ship ship in ships)
        {
            if (ship.id == _id)
                return ship.gameObject;
        }
        return null;
    }
}
