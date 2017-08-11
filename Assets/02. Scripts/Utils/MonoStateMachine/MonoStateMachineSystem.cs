using UnityEngine;
using System.Collections.Generic;

public class MonoStateMachineSystem : MonoBehaviour, IStatable, IPausable {
    public bool debug = false;
	public MonoStateMachineState[] states;
	private Dictionary<string, MonoStateMachineState> m_States;
	[HideInInspector]
	public MonoStateMachineState m_CurrentState;
	public enum StatesLogic {OR, AND};
	public StatesLogic statesLogic = StatesLogic.OR;
	[HideInInspector]
	public MonoStateMachineState m_LastState;

	private int m_PauseCount = 0;

	private bool m_Paused = false;

	// Use this for initialization
	void Start () {
		if (states.Length > 0) {
			m_States = new Dictionary<string, MonoStateMachineState> ();
			for (int i = 0; i < states.Length; ++i) {
				m_States.Add (states [i].stateName, states [i]);
				states[i].m_Parent = this;
				states[i].m_System = this;
			}
		} else {
			Debug.LogError("La maquina de estados necesita al menos un estado.");
		}
	}

	// Update is called once per frame
	void Update () {
		if (!m_Paused) {
			if (m_CurrentState == null) {
				m_CurrentState = states [0];
				m_CurrentState.OnEnter ();
			}
			if (statesLogic == StatesLogic.OR) {
				m_CurrentState.SubStateUpdate ();
				m_CurrentState.StateUpdate ();
			} else {
				for (int i = 0; i < states.Length; ++i) {
					states [i].SubStateUpdate ();
					states [i].StateUpdate ();
				}
			}
		}
	}

	public bool ChangeState(string aStateName) {
		bool l_Ok = false;
        if(m_CurrentState.stateName == "Dying")
        {
            return l_Ok;
        }
		if (m_States.ContainsKey(aStateName) && statesLogic == StatesLogic.OR) {
			l_Ok = true;
			m_CurrentState.Exit();
			m_LastState = m_CurrentState;
			m_CurrentState = m_States[aStateName];
            if(m_LastState.stateName == m_CurrentState.stateName && m_LastState.stateName != "Active" && aStateName == "Stunned")
            {
                m_LastState = m_States["Active"];
            }
			m_CurrentState.FirstEnter();
			m_CurrentState.Enter();
		}
		return l_Ok;
	}

	public bool ChangeToPreviousState() {
		return ChangeState(m_LastState.stateName);
	}

    protected void StateDebug(string text)
    {
        if (debug)
        {
            Debug.Log(gameObject.name + ": " + text);
        }
    }

	#region IPausable implementation


	public void Pause ()
	{
		m_Paused = true;
		m_PauseCount++;
	}


	public void Unpause ()
	{
		m_PauseCount--;
		if (m_PauseCount <= 0) {
			m_PauseCount = 0;
			m_Paused = false;
		}
	}


	#endregion

}
