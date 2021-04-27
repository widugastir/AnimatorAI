using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class AI_State : MonoBehaviour
{
	[Header("Sync")]
	public float SyncUpdateSpeed = 1f;
	[SerializeField] protected AI_State[] SyncStatesActivity;
	
	[Header("Settings")]
	public DisableType Disable = DisableType.script;
	public Animator _animator;
	private AI_Brain _brain;
	private FieldInfo[] m_fields;
	private FieldInfo[] t_fields;
	private EventInfo[] t_events;
	private MethodInfo[] m_eventMethods;
	
	[HideInInspector] public AI_Behaviour _behaviour;
	[HideInInspector] public bool _initialized = false;
	
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
	
	public virtual void Init(Animator animator, AI_Brain brain, AI_Behaviour behaviour)
	{
		if(_initialized == false)
		{
			DisableState();
			_brain = brain;
			_animator = animator;
			_behaviour = behaviour;
			
			if(_behaviour != null)
			{
				m_fields = this.GetType()
					.GetFields(BindingFlags.Instance | BindingFlags.Public)
					.Where(field => field.GetCustomAttribute<AI_LinkData>() != null && field.FieldType.IsGenericType)
					.ToArray();
			
				t_fields = _behaviour.GetType()
					.GetFields(BindingFlags.Instance | BindingFlags.Public)
					.Where(field => field.GetCustomAttribute<AI_LinkData>() != null && field.FieldType.IsGenericType)
					.ToArray();
					
				m_eventMethods = this.GetType()
					.GetMethods(BindingFlags.Instance | BindingFlags.Public);
					
				LinkData();
				InitEvents();
			}
				
			_initialized = true;
		}
	}
	
	private void LinkData()
	{
		foreach(FieldInfo f in m_fields)
		{
			foreach(FieldInfo t in t_fields)
			{
				if(f.Name == t.Name
					&& f.FieldType.GetGenericTypeDefinition() == t.FieldType.GetGenericTypeDefinition()
					&& f.FieldType.GetGenericArguments()[0] == t.FieldType.GetGenericArguments()[0])
				{
					AI_Data<System.Int32> f_d = (AI_Data<System.Int32>)f.GetValue(this);
					AI_Data<System.Int32> t_d = (AI_Data<System.Int32>)t.GetValue(_behaviour);
					f_d.Link(t_d);
					t_d.Link(f_d);
				}
			}
		}
	}
    
	private void InitEvents()
	{
		t_events = _behaviour.GetType().GetEvents(BindingFlags.Instance | BindingFlags.Public);
		foreach(EventInfo f in t_events)
		{
			if(f.GetCustomAttribute<AI_InjectEvent>() != null)
			{
				foreach(MethodInfo m_f in m_eventMethods)
				{
					if(m_f.GetCustomAttribute<AI_InjectEventTarget>() != null && m_f.Name == f.Name)
					{
						Delegate d = Delegate.CreateDelegate(f.EventHandlerType, this, m_f);
						f.AddEventHandler(_behaviour, d);
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
