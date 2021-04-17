using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AI_Brain : MonoBehaviour
{
	public DataBase Data;
	[SerializeField] private AI_State[] States;
	
	private AI_State _activeState;
	private Animator _animator;
	
	private void OnEnable() => AI_Behaviour.OnStateChange += OnStateChange;
	private void OnDisable() => AI_Behaviour.OnStateChange -= OnStateChange;
	
	private void Start()
	{
		_animator = GetComponent<Animator>();
		foreach(AI_State ai in States)
		{
			ai.Init(_animator, this);
		}
	}
	
	private void OnStateChange(AI_Behaviour behaviour)
	{
		foreach(AI_State ai in States)
		{
			if(behaviour.GetType().Name == ai.GetType().Name.Substring(3))
			{
				if(_activeState != null)
				{
					_activeState.DisableState();
				}
				_activeState = ai;
				_activeState.EnableState();
				break;
			}
		}
	}
}
