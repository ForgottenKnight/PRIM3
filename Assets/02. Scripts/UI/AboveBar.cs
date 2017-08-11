using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AboveBar : MonoBehaviour {
	public Canvas barPrefab;
	protected Canvas bar;
	public float scaleX = 2.0f;
	public float scaleY = 2.0f;
	public float offsetY = 1.0f;

	virtual protected void Start () {
		bar = (Canvas)Instantiate (barPrefab);
		bar.transform.SetParent(gameObject.transform);
		bar.transform.localPosition = Vector3.up * offsetY;
		bar.transform.localScale = (Vector3.forward + Vector3.right * scaleX + Vector3.up * scaleY) * 0.01f;
	}
}
