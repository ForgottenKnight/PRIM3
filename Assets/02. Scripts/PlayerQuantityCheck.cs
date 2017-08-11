using UnityEngine;
using System.Collections;

public class PlayerQuantityCheck : MonoBehaviour {
	int players;
	public int playersNumberTarget;
	public enum Condition {Equal, NotEqual, Less, More}
	public Condition condition = Condition.Equal;
	public enum Effect {Disappear, CallEvent}
	public Effect effect = Effect.Disappear;
	public string eventName;

	// Use this for initialization
	void Start () {
      //  if (StaticParemeters.Init)
       // {
            players = StaticParemeters.numPlayers;
       // }
       // else
       // {
        //   players = GameObject.FindGameObjectsWithTag("Player").Length;
       // }

		bool conditionMet = false;
		switch(condition) {
		case Condition.Equal:
			if (players == playersNumberTarget) {
				conditionMet = true;
			}
			break;
		case Condition.Less:
			if (players < playersNumberTarget) {
				conditionMet = true;
			}
			break;
		case Condition.More:
			if (players > playersNumberTarget) {
				conditionMet = true;
			}
			break;
		case Condition.NotEqual:
			if (players != playersNumberTarget) {
				conditionMet = true;
			}
			break;
		}
		if (conditionMet) {
			switch(effect) {
			case Effect.CallEvent:
				SimpleEvent.eventsDictionary[eventName].ExternalTriggerFunction();
				break;
			case Effect.Disappear:
				Destroy (gameObject);
				break;
			}
		}
		Destroy (this);
	}
}
