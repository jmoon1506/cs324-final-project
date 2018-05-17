using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadScreen : MonoBehaviour {

    private GameObject loadIcon;
    public float speed = 5.0f;
    public float acceleration = 0.05f;
    private float delta;

    void Awake()
    {
        loadIcon = transform.Find("Loading Circle").gameObject;
        delta = acceleration;
		Invoke ("SwitchScene", 3.0f);
    }

	void SwitchScene()
	{
		Application.LoadLevel ("Game");

	}
	
	// Update is called once per frame
	void Update () {
        if (loadIcon != null)
        {
            if (speed < 3.0f)
            {
                delta = acceleration;
            }
            else if (speed > 7.0f)
            {
                delta = -acceleration;
            }
            speed += delta;
            loadIcon.transform.eulerAngles = new Vector3(0, 0, loadIcon.transform.eulerAngles.z + speed);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene("Game");
        }
	}
}
