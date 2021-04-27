using UnityEngine;

// Trigger-container for animator parameters
[System.Serializable]
public class AI_Trigger : IBindableParameter
{
	[SerializeField, HideInInspector] public string _parameter;
	[SerializeField, HideInInspector] private Animator _animator;
	
	public string GetParameterName() => _parameter;
	public void SetParameterName(string name) => _parameter = name;
	public AnimatorControllerParameterType GetParameterType() => AnimatorControllerParameterType.Trigger;
	
	public void Bind(string parameter, Animator animator, AnimatorControllerParameterType type)
	{
		if(type == AnimatorControllerParameterType.Trigger)
		{
			_animator = animator;
			_parameter = parameter;
		}
	}
	
	public void Unbind()
	{
		_animator = null;
		_parameter = null;
	}
	
	public void Set()
	{
		if(_animator)
			_animator.SetTrigger(_parameter);
	}
}