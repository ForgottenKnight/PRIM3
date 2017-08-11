using UnityEngine;
using System.Collections;

public interface IPushable {
	void Push(float aSpeed, float aTime, Vector3 aSource, bool aChangeAnimation = true);
}
