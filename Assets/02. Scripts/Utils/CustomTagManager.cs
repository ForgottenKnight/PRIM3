using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CustomTagManager {
	private static Dictionary<string, List<GameObject>> m_TagDictionary;

	public static void AddToTag(string aTag, GameObject aGameObject) {
		if (m_TagDictionary == null) {
			Init ();
		}
		if (!m_TagDictionary.ContainsKey (aTag)) {
			m_TagDictionary.Add(aTag, new List<GameObject>());
		}
		m_TagDictionary [aTag].Add (aGameObject);
	}

	public static GameObject GetObjectByTag(string aTag) {
		GameObject l_GameObject = null;
		if (m_TagDictionary.ContainsKey (aTag)) {
			if (m_TagDictionary[aTag].Count > 0) {
				l_GameObject = m_TagDictionary[aTag][0];
			}
		}
		return l_GameObject;
	}

	public static List<GameObject> GetObjectsByTag(string aTag) {
		List<GameObject> l_GameObjectList = new List<GameObject>();
		if (m_TagDictionary.ContainsKey (aTag)) {
			if (m_TagDictionary[aTag].Count > 0) {
				l_GameObjectList = m_TagDictionary[aTag];
			}
		}
		return l_GameObjectList;
	}

	public static void RemoveFromTag(string aTag, GameObject aGameObject) {
		if (m_TagDictionary == null) {
			Init ();
		}
		if (m_TagDictionary.ContainsKey (aTag)) {
			m_TagDictionary[aTag].Remove(aGameObject);
		}
	}

	public static void Clear() {
		m_TagDictionary.Clear ();
	}

	private static void Init() {
		m_TagDictionary = new Dictionary<string, List<GameObject>> ();
	}
}
