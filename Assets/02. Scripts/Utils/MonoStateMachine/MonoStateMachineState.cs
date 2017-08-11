using UnityEngine;
using System.Collections.Generic;

public class MonoStateMachineState : MonoBehaviour, IStatable {
    public bool debug;
	[Header("State parameters")]
	public MonoStateMachineState[] subStates;
	private Dictionary<string, MonoStateMachineState> m_SubStates;
	[HideInInspector]
	public MonoStateMachineState m_CurrentSubState;
	[HideInInspector]
	public MonoStateMachineState m_LastState;
	public string stateName;
	public enum StatesLogic {OR, AND};
	public StatesLogic statesLogic = StatesLogic.OR;
	[HideInInspector]
	public MonoStateMachineSystem m_System;
	[HideInInspector]
	public IStatable m_Parent;
	private bool m_FirstEnter;

	void Start () {
		if (subStates.Length > 0) {
			m_SubStates = new Dictionary<string, MonoStateMachineState> ();
			for (int i = 0; i < subStates.Length; ++i) {
				m_SubStates.Add (subStates [i].stateName, subStates [i]);
				subStates[i].m_Parent = this;
				subStates[i].m_System = m_System;
			}
		} else {
			m_CurrentSubState = null;
		}
		m_FirstEnter = false;
		OnStart ();
	}

	public void SubStateUpdate() {
		if (m_CurrentSubState == null && subStates.Length > 0) {
			m_CurrentSubState = subStates [0];
			m_CurrentSubState.OnEnter();
		}
		if (statesLogic == StatesLogic.OR) {
			if (m_CurrentSubState != null) {
				m_CurrentSubState.SubStateUpdate ();
				m_CurrentSubState.StateUpdate ();
			}
		} else {
			for (int i = 0; i < subStates.Length; ++i) {
				subStates[i].SubStateUpdate();
				subStates[i].StateUpdate();
			}
		}
	}

	public bool ChangeState(string aStateName) {
		bool l_Ok = false;
		if (m_SubStates.ContainsKey(aStateName) && statesLogic == StatesLogic.OR) {
			l_Ok = true;
			if (m_CurrentSubState != null) {
				m_CurrentSubState.Exit();
				m_LastState = m_CurrentSubState;
			}
			m_CurrentSubState = m_SubStates[aStateName];
			m_CurrentSubState.FirstEnter();
			m_CurrentSubState.Enter();
		}
		return l_Ok;
	}

	public bool ChangeToPreviousState() {
		return ChangeState(m_LastState.stateName);
	}

	public virtual void StateUpdate() {
	}

	public virtual void OnEnter() {
	}

	public void Enter() {
		OnEnter ();
		if (m_CurrentSubState != null) {
			m_CurrentSubState.Enter ();
		}
	}

	public void FirstEnter() {
		if (m_FirstEnter == false) {
			m_FirstEnter = true;
			OnFirstEnter();
		}
	}

	public virtual void OnFirstEnter() {
	}

	public virtual void OnExit() {
	}

	public void Exit() {
		OnExit ();
		if (m_CurrentSubState != null) {
			m_CurrentSubState.Exit ();
		}
	}

	public virtual void OnStart() {
	}

    protected void StateDebug(string text)
    {
        if (debug)
        {
            Debug.Log(gameObject.name + " - " + stateName +": " + text);
        }
    }

}
