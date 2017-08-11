using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MovementManager))]
public class AbovePlayer : ActionBehaviour {
	// Componentes obligatorios
	private MovementManager mm;
	public float slideSpeed = 10.0f;
	public bool active;
	public AbovePlayerCheck aboveChecker;

	void Start () {
		mm = GetComponent<MovementManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/* Obtiene los estados o valores del Input y los asigna a las diferentes acciones */
	override public void GetInput() {
	} 

	/* Calcula y guarda la direccion y cantidad de movimiento con la cual se movera el personaje sobre el eje Y */
	override public void ActionUpdate() {
		if (active) {
			mm.movement = transform.forward * Time.deltaTime * slideSpeed;
		}
	}

	override public bool isActionActive() {
		active = aboveChecker.abovePlayer;
		return active;
	}
}
