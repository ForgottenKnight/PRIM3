using UnityEngine;
using System.Collections;

public class Explosion1 : MonoBehaviour {
	public float duration = 1.0f;
	public float fadeDuration = 1.0f;
	public float loopduration = 0.5f;
	public Vector3 initialSize = Vector3.zero;
	public Vector3 finalSize = Vector3.one;

	Material mat;
	float timer;

	// Use this for initialization
	void Start () {
		mat = GetComponent<Renderer>().material;
		timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		transform.localScale = Vector3.Lerp (initialSize, finalSize, (timer / duration));
		if (timer >= duration) {
			Destroy (gameObject);
		}
		float r = Mathf.Sin((Time.time / loopduration) * (2 * Mathf.PI)) * 0.5f + 0.25f;
		float g = Mathf.Sin((Time.time / loopduration + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
		float b = Mathf.Sin((Time.time / loopduration + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;
		float correction = 1 / (r + g + b);
		r *= correction;
		g *= correction;
		b *= correction;
		mat.SetVector("_ChannelFactor", new Vector4(r,g,b,0));
	}
}
