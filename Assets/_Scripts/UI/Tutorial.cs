using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {
	public float wait_time = 0.0f;
	public bool waiting_for_pan = true;
	public bool waiting_for_drag = true;
	public bool waiting_for_zoom = true;

	public GameObject panning;
	public GameObject dragging;
	public GameObject zooming;

	public bool panned_w = false;
	public bool panned_a = false;
	public bool panned_s = false;
	public bool panned_d = false;

	public bool dragged = false;
	public bool zoomed = false;

	enum tutorial_state { none, pan, drag, zoom, final };
	private tutorial_state state = tutorial_state.none;
	private float delta_color = 1;

	void Start () {
		Color c = dragging.GetComponent<Image> ().color;
		dragging.GetComponent<Image> ().color = new Color (c.r, c.g, c.b, 0);
		c = zooming.GetComponent<Image> ().color;
		zooming.GetComponent<Image>().color = new Color (c.r, c.g, c.b, 0);
	}

	IEnumerator SwitchState(tutorial_state s, float delayTime, bool play_fadeout)
	{
		yield return new WaitForSeconds(delayTime);
		state = s;
		if (s == tutorial_state.pan) {
			panning.GetComponent<Animator> ().Play ("panning_fade");
		} else if (s == tutorial_state.drag) {
			AnimatorStateInfo anim_state = panning.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0);
			if (anim_state.IsName ("panning_fade") || anim_state.IsName ("panning_cycle")) {
				if (anim_state.normalizedTime < 1.0) {
					StartCoroutine (SwitchState (tutorial_state.drag, 0.3f, true));
					yield break;
				}
			}
			if (play_fadeout)
				panning.GetComponent<Animator> ().Play ("panning_fadeout");
			dragging.GetComponent<Animator> ().Play ("dragging_fade");
		} else if (s == tutorial_state.zoom) {
			AnimatorStateInfo anim_state = dragging.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0);
			if (anim_state.IsName ("dragging_fade") || anim_state.IsName ("dragging_cycle")) {
				if (anim_state.normalizedTime < 1.0) {
					StartCoroutine (SwitchState (tutorial_state.zoom, 0.3f, true));
					yield break;
				}
			}
			if (play_fadeout)
				dragging.GetComponent<Animator> ().Play ("dragging_fadeout");
			zooming.GetComponent<Animator> ().Play ("zooming_fade");
		} else if (s == tutorial_state.final) {
			AnimatorStateInfo anim_state = zooming.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0);
			if (anim_state.IsName ("zooming_fade") || anim_state.IsName ("zooming_cycle")) {
				if (anim_state.normalizedTime < 1.0) {
					StartCoroutine (SwitchState (tutorial_state.final, 0.3f, true));
					yield break;
				}
			}
			zooming.GetComponent<Animator> ().Play ("zooming_fadeout");
		}
	}

	void Update () {
		if (waiting_for_pan) {
			if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0) {
				waiting_for_pan = false;
			}
			else if (wait_time > 3.5f) {
				waiting_for_pan = false;
				waiting_for_drag = false;
				waiting_for_zoom = false;
				StartCoroutine(SwitchState(tutorial_state.pan, 0.5f, false));
			}
			wait_time += Time.deltaTime;
		}
		else if (waiting_for_drag) {
			if (Input.GetAxis("Fire1") != 0 && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)) {
				waiting_for_drag = false;
			}
			else if (wait_time > 3.5f) {
				waiting_for_pan = false;
				waiting_for_drag = false;
				waiting_for_zoom = false;
				StartCoroutine(SwitchState(tutorial_state.drag, 0.5f, false));
			}
			wait_time += Time.deltaTime;
		}
		else if (waiting_for_zoom) {
			if (Input.GetAxis("Mouse ScrollWheel") != 0) {
				waiting_for_zoom = false;
			}
			else if (wait_time > 3.5f) {
				waiting_for_pan = false;
				waiting_for_drag = false;
				waiting_for_zoom = false;
				StartCoroutine(SwitchState(tutorial_state.zoom, 0.5f, false));
			}
			wait_time += Time.deltaTime;
		}
		
		if (state == tutorial_state.pan) {
			if (Input.GetAxis ("Horizontal") < 0) {
				panned_a = true;
			}
			if (Input.GetAxis ("Horizontal") > 0) {
				panned_d = true;
			}
			if (Input.GetAxis ("Vertical") > 0) {
				panned_w = true;
			}
			if (Input.GetAxis ("Vertical") < 0) {
				panned_s = true;
			}
			if (panned_a && panned_d && panned_w && panned_s) {
				StartCoroutine(SwitchState(tutorial_state.drag, 0.3f, true));
			}
		}
		else if (state == tutorial_state.drag) {
			if (Input.GetAxis("Fire1") != 0 && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)) {
				StartCoroutine(SwitchState(tutorial_state.zoom, 0.3f, true));
			}
		}
		else if (state == tutorial_state.zoom) {
			if (Input.GetAxis("Mouse ScrollWheel") != 0) {
				StartCoroutine(SwitchState(tutorial_state.final, 0.3f, true));
			}
		}
	}
}
