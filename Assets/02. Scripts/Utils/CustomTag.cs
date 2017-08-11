using UnityEngine;
using System.Collections.Generic;

public class CustomTag : MonoBehaviour {
	[Tooltip("Una o mas etiquetas por la que buscar a este objeto utilizando CustomTagManager")]
	public List<string> customTags;
	
	private void Start () {
		if (customTags == null) {
			customTags = new List<string>();
		}
		if (customTags.Count == 0) {
			customTags.Add(gameObject.tag);
		}
		for (int i = 0; i < customTags.Count; ++i) {
			CustomTagManager.AddToTag (customTags[i], gameObject);
		}
	}
	
	private void OnDestroy() {
		for (int i = 0; i < customTags.Count; ++i) {
			CustomTagManager.RemoveFromTag (customTags[i], gameObject);
		}
	}

}
