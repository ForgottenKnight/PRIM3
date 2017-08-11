using UnityEngine;
using System.Collections;

public class AnimationSulphur : MonoBehaviour {
	Animator anim;
	public SulphurController sulphurController;
	public Transform enemies;
	public float pushTime;
	public float pushSpeed;
	
	void Start() {
		anim = GetComponent<Animator> ();
		enemies = GameObject.FindGameObjectWithTag ("EnemiesContainer").transform;
	}
	
	public void SpawnFireBall() {

		if (sulphurController.tp)
			sulphurController.tp.lookAtTargetRequest ();
		sulphurController.proyectil = (GameObject)Instantiate (sulphurController.proyectilPrefab, sulphurController.transform.position + sulphurController.transform.forward * sulphurController.forwardOffset + sulphurController.transform.up * sulphurController.yOffset + sulphurController.transform.right * sulphurController.zOffset, sulphurController.transform.rotation);
		ProjectileController l_PC = sulphurController.proyectil.GetComponent<ProjectileController> ();
		l_PC.source = sulphurController.gameObject;
		if (sulphurController.particlePrefab != null) {
			Instantiate (sulphurController.particlePrefab, sulphurController.transform.position + sulphurController.transform.forward * sulphurController.forwardOffset + sulphurController.transform.up * sulphurController.yOffset + sulphurController.transform.right * sulphurController.zOffset, sulphurController.transform.rotation);
		}
		IPushable l_Pushable = sulphurController.gameObject.GetComponent<IPushable> ();
		if (l_Pushable != null) {
			l_Pushable.Push (pushSpeed, pushTime, sulphurController.transform.position + sulphurController.transform.forward, false);
		}
	}
}
