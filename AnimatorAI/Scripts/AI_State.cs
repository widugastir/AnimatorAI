using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Reflection;
using System.ComponentModel;

public abstract class AI_State : MonoBehaviour
{
	[Header("Settings")]
	public float SyncUpdateSpeed = 1f;
	public DisableType Disable = DisableType.script;
	[SerializeField] protected AI_State[] SyncStatesActivity;
	
	protected Animator _animator;
	private bool _initialized = false;
	private AI_Brain _brain;
	
	private FieldInfo[] m_fields;
	private FieldInfo[] t_fields;
	private EventInfo[] t_events;
	private MethodInfo[] m_eventMethods;
	private AI_Behaviour[] _behaviour;
	
	#region Short Expressions
	protected void SetBoolState(string name, bool value) => _animator.SetBool(name, value);
	protected void SetFloatState(string name, float value) => _animator.SetFloat(name, value);
	protected void SetIntState(string name, int value) => _animator.SetInteger(name, value);
	protected void SetTriggerState(string name) => _animator.SetTrigger(name);
	#endregion
	
	protected virtual void OnEnable() 
	{
		if(_initialized)
			StartCoroutine(PropertyChange());
	}
	
	public void EnableState(bool ignoreSync = false)
	{
		enabled = true;
		gameObject.SetActive(true);
		if(ignoreSync == false)
		{
			foreach(AI_State state in SyncStatesActivity)
			{
				state.EnableState(true);
			}
		}
	}
	
	public void DisableState(bool ignoreSync = false)
	{
		if(Disable == DisableType.gameObject || Disable == DisableType.both)
			gameObject.SetActive(false);
		if(Disable == DisableType.script || Disable == DisableType.both)
			enabled = false;
		if(ignoreSync == false)
		{
			foreach(AI_State state in SyncStatesActivity)
			{
				state.DisableState(false);
			}
		}
	}
	
	public virtual void Init(Animator animator, AI_Brain brain)
	{
		if(_initialized == false)
		{
			DisableState();
			_brain = brain;
			_animator = animator;
			m_fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			m_eventMethods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
			_behaviour = _animator.GetBehaviours<AI_Behaviour>();
			InitEvents();
				
			_initialized = true;
		}
	}
    
	private IEnumerator PropertyChange()
	{
		while(m_fields.Length > 0)
		{
			foreach(var sb in _behaviour)
			{
				t_fields = sb.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
				UpdateFields(sb);
			}
			yield return new WaitForSeconds(SyncUpdateSpeed);
		}
	}
	
	private void UpdateFields(AI_Behaviour behavior)
	{
		foreach(FieldInfo f in t_fields)
		{
			if(f.GetCustomAttribute<AI_InjectField>() != null)
			{
				foreach(FieldInfo m_f in m_fields)
				{
					if(m_f.Name == f.Name && m_f.FieldType == f.FieldType)
					{
						f.SetValue(behavior, m_f.GetValue(this));
					}
				}
			}
		}
	}
	
	private void InitEvents()
	{
		foreach(var sb in _behaviour)
		{
			t_events = sb.GetType().GetEvents(BindingFlags.Instance | BindingFlags.Public);
			foreach(EventInfo f in t_events)
			{
				if(f.GetCustomAttribute<AI_InjectEvent>() != null)
				{
					foreach(MethodInfo m_f in m_eventMethods)
					{
						if(m_f.GetCustomAttribute<AI_InjectEventTarget>() != null && m_f.Name == f.Name)
						{
							Delegate d = Delegate.CreateDelegate(f.EventHandlerType, this, m_f);
							f.AddEventHandler(sb, d);
						}
					}
				}
			}
		}
	}
	
	public enum DisableType
	{
		alwaysActive,
		script,
		gameObject,
		both
	}
}
