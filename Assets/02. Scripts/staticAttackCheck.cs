using UnityEngine;
using System.Collections;

public class staticAttackCheck
{
	public static float checkAttack(Transform target, Transform attacker, float attackLength, float attackAngle, float attackHeight,  
	                         float attackForce, float damageBonus = default(float), int currentPhase =  default(int)  )
	{
		float DamageDone = -1.0f;

		float heightDif = Mathf.Abs(target.position.y - attacker.position.y);

		// CALCULOS SEGUN BOUNDING BOX
		Vector3 scale = target.localScale;
		scale /= 2.0f;
		Collider c = target.GetComponent<Collider>();
		Bounds bounds = new Bounds();
		bool foundBounds = false;
		if (c != null) {
			bounds = c.bounds;
			foundBounds = true;
		} else {
			Renderer r = target.GetComponent<Renderer> ();
			if (r != null) {
				bounds = r.bounds;
				foundBounds = true;
			}
		}
		if (foundBounds) {
			scale = bounds.extents;
		}
		// FIN DE CALCULOS SEGUN BOUNDING BOX

		if(heightDif-scale.y <= attackHeight)
		{



			//Transform enemy = enemies.GetChild(i);
			//Vector3 scale = target.localScale;
			//scale /= 2.0f;
			scale.y = 0f;
			Vector3 targetPosition = target.position;
			targetPosition.y = 0f;
			Vector3 attackerPosition = attacker.position;
			attackerPosition.y = 0f;
			Vector3 attackerForward = attacker.forward;
			attackerForward.y = 0f;
			float dist = Vector3.Distance(attackerPosition, targetPosition);
			float dist1 = Vector3.Distance(attackerPosition, targetPosition + scale);
			float dist2 = Vector3.Distance(attackerPosition, targetPosition - scale);
			scale.x *= -1.0f;
			float dist3 = Vector3.Distance(attackerPosition, targetPosition + scale);
			float dist4 = Vector3.Distance(attackerPosition, targetPosition - scale);
			scale.x *= -1.0f;
			
			//	Debug.Log("Distancia al Enemigo " + dist);
			if(dist <= attackLength || dist1 <= attackLength || dist2 <= attackLength ||
			   dist3 <= attackLength || dist4 <= attackLength)//Comprueba si el enemigo esta dentro de mi distancia de ataque
			{
				//	Debug.Log("Enemigo dentro de rango");
				float Angl = Vector3.Angle(targetPosition - attackerPosition , attackerForward);
				//scale = target.localScale;
				//scale /= 2.0f;
				//scale.y = 0;
				float Angl2 = Vector3.Angle(targetPosition + scale - attackerPosition, attackerForward);
				float Angl3 = Vector3.Angle(targetPosition - scale - attackerPosition, attackerForward);
				scale.x *= -1.0f;
				float Angl4 = Vector3.Angle(targetPosition + scale - attackerPosition, attackerForward);
				float Angl5 = Vector3.Angle(targetPosition - scale - attackerPosition, attackerForward);
				
				/*	Debug.Log("Angulo del enemigo: " + Angl);
			
						Debug.Log("Angulo del enemigo + localScale: " + Angl2);
						Debug.Log("Angulo del enemigo - localScale: " + Angl3);
						
						Debug.Log("Angulo del enemigo + localScale con x* -1: " + Angl4);
						Debug.Log("Angulo del enemigo - localScale con x* -1: " + Angl5);*/

				
				
				if(Angl <= attackAngle/2.0f || Angl2 <= attackAngle/2.0f || Angl3 <= attackAngle/2.0f 
				   || Angl4 <= attackAngle/2.0f || Angl5 <= attackAngle/2.0f)
				{
					//	Debug.Log("Enemigo dentro de angulo");
					Damageable h = target.GetComponent<Damageable>();
					if(h)
						DamageDone = h.Damage (attackForce + attackForce*((float)currentPhase*damageBonus/100.0f));
				}
			}
		}
		return DamageDone;
		
	}

	public static void CheckPush(Transform aTarget, Vector3 aCenter, float aRadius, float aPushForce, float aPushTime) {
		IPushable l_Pushable = aTarget.GetComponent<IPushable> ();
		if (l_Pushable != null) {
			Vector3 l_TargetPos = aTarget.position;
			Vector3 l_Scale = aTarget.localScale;
			l_Scale /= 2.0f;
			l_Scale.y = 0;
			float l_Dist = Vector3.Distance (aCenter, l_TargetPos);
			float l_Dist1 = Vector3.Distance (aCenter, l_TargetPos + l_Scale);
			float l_Dist2 = Vector3.Distance (aCenter, l_TargetPos - l_Scale);
			l_Scale.x *= -1.0f;
			float l_Dist3 = Vector3.Distance (aCenter, l_TargetPos + l_Scale);
			float l_Dist4 = Vector3.Distance (aCenter, l_TargetPos - l_Scale);

			if (l_Dist <= aRadius || l_Dist1 <= aRadius || l_Dist2 <= aRadius || l_Dist3 <= aRadius || l_Dist4 <= aRadius) {
				l_Pushable.Push(aPushForce, aPushTime, aCenter);
			}
		}
	}

	public static void CheckPush(Transform aTarget, Vector3 aCenter, float aRadius, float aPushForce, float aPushTime, Vector3 aCustomCenter) {
		IPushable l_Pushable = aTarget.GetComponent<IPushable> ();
		if (l_Pushable != null) {
			Vector3 l_TargetPos = aTarget.position;
			Vector3 l_Scale = aTarget.localScale;
			l_Scale /= 2.0f;
			l_Scale.y = 0;
			float l_Dist = Vector3.Distance (aCenter, l_TargetPos);
			float l_Dist1 = Vector3.Distance (aCenter, l_TargetPos + l_Scale);
			float l_Dist2 = Vector3.Distance (aCenter, l_TargetPos - l_Scale);
			l_Scale.x *= -1.0f;
			float l_Dist3 = Vector3.Distance (aCenter, l_TargetPos + l_Scale);
			float l_Dist4 = Vector3.Distance (aCenter, l_TargetPos - l_Scale);
			
			if (l_Dist <= aRadius || l_Dist1 <= aRadius || l_Dist2 <= aRadius || l_Dist3 <= aRadius || l_Dist4 <= aRadius) {
				l_Pushable.Push(aPushForce, aPushTime, aCustomCenter);
			}
		}
	}

	public static void CheckPull(Transform aTarget, Vector3 aCenter, float aRadius, float aPushForce, float aPushTime, float aPullMinDistance) {
		IPullable l_Pullable = aTarget.GetComponent<IPullable> ();
		if (l_Pullable != null) {
			Vector3 l_TargetPos = aTarget.position;
			Vector3 l_Scale = aTarget.localScale;
			l_Scale /= 2.0f;
			l_Scale.y = 0;
			float l_Dist = Vector3.Distance (aCenter, l_TargetPos);
			float l_Dist1 = Vector3.Distance (aCenter, l_TargetPos + l_Scale);
			float l_Dist2 = Vector3.Distance (aCenter, l_TargetPos - l_Scale);
			l_Scale.x *= -1.0f;
			float l_Dist3 = Vector3.Distance (aCenter, l_TargetPos + l_Scale);
			float l_Dist4 = Vector3.Distance (aCenter, l_TargetPos - l_Scale);
			
			if (l_Dist <= aRadius || l_Dist1 <= aRadius || l_Dist2 <= aRadius || l_Dist3 <= aRadius || l_Dist4 <= aRadius) {
				l_Pullable.Pull(aPushForce, aPushTime, aCenter, aPullMinDistance);
			}
		}
	}
}
