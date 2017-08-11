using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformAnimatorGroup : MonoBehaviour {
	
	public Dictionary<string, PlatformGroup> platformGroup = new Dictionary<string, PlatformGroup>();
	public string[] groups;

	[System.Serializable]
	public class PlatformGroup {
		public List<PlatformAnimator> platforms = new List<PlatformAnimator>();

		public void ActivatePlatforms (Vector3 aPosition, float aRadius) {
			for (int i = 0; i < platforms.Count; i++) {
				if (Vector3.Distance(platforms[i].transform.position, aPosition) < aRadius) {
					platforms[i].playAnimation();
				}
			}
		}

	}
	void Awake() {
		for (int i = 0; i < groups.Length; i++){
			PlatformGroup l_newPlatformGroup = new PlatformGroup();
			platformGroup.Add(groups[i], l_newPlatformGroup);
		}
	}

	public PlatformGroup getGroup(string aGroupName) {
		return platformGroup [aGroupName];
	}


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
