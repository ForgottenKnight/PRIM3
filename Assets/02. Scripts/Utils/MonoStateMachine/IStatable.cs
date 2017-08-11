using UnityEngine;
using System.Collections;

public interface IStatable {
	bool ChangeState(string aStateName);
	bool ChangeToPreviousState();
}
