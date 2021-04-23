using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AI_Brain : MonoBehaviour
{
	[SerializeField] private AI_State[] States;
	
	private AI_State _activeState;
	private Animator _animator;
	
	private void OnEnable() => AI_Behaviour.OnStateChange += OnStateChange;
	private void OnDisable() => AI_Behaviour.OnStateChange -= OnStateChange;
	
	private void Start()
	{
		_animator = GetComponent<Animator>();
		AI_Behaviour[] behaviours = _animator.GetBehaviours<AI_Behaviour>();
		
		foreach(AI_State ai in States)
		{
			foreach(AI_Behaviour beh in behaviours)
			{
				if(IsLinked(beh, ai))
				{
					ai.Init(_animator, this, beh);
				}
			}
			if(ai._initialized == false)
			{
				ai.Init(_animator, this, null);
			}
		}
	}
	
	private bool IsLinked(AI_Behaviour behaviour, AI_State state)
	{
		if(behaviour.GetType().Name == state.GetType().Name.Substring(3)
			|| behaviour.LinkedStateName == state.GetType().Name)
		{
			return true;
		}
		return false;
	}
	
	private void OnStateChange(Animator animator, AI_Behaviour behaviour)
	{
		if (animator.gameObject != gameObject)
			return;
		foreach(AI_State ai in States)
		{
			if(IsLinked(behaviour, ai))
			{
				if (_activeState != null)
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
