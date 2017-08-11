using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour {
    [Header("Lightning parameters")]
    public float effectTime = 2.5f;
    public float activationTime = 3f;
    public float damageArea = 3f;
    public int damage = 15;
    public ParticleSystem[] activationSystems;

    [Header("Push parameters")]
    public bool push = true;
    public float pushSpeed = 0f;
    public float pushTime = 1f;

	// Use this for initialization
	void Start () {
        StartCoroutine(DoLightning());
	}

    IEnumerator DoLightning() {
        yield return new WaitForSeconds(effectTime);
        for (int i = 0; i < activationSystems.Length; ++i)
        {
            activationSystems[i].Play();
        }
        yield return new WaitForSeconds(activationTime - effectTime);
        
        List<GameObject> l_Players = CustomTagManager.GetObjectsByTag("Player");
        for (int i = 0; i < l_Players.Count; ++i)
        {
            staticAttackCheck.checkAttack(l_Players[i].transform, transform, damageArea, 360f, damageArea, damage);
            if (push == true)
            {
                IPushable l_Pushable = l_Players[i].GetComponent<IPushable>();
                l_Pushable.Push(pushSpeed, pushTime, transform.position);
            }
        }
        GameObject l_SaltShield = CustomTagManager.GetObjectByTag("SaltShield");
        if (l_SaltShield != null)
        {
            staticAttackCheck.checkAttack(l_SaltShield.transform, transform, damageArea, 360f, damageArea, damage);
        }
        Destroy(gameObject);
        yield return null;
    }
}
