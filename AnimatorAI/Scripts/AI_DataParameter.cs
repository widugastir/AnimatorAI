using UnityEngine;

// Data-container for animator parameters
[System.Serializable]
public class AI_DataParameter<T> : IBindableParameter
{
	[SerializeField, HideInInspector] private string _parameter;
	[SerializeField, HideInInspector] private Animator _animator;
	[SerializeField, HideInInspector] private AnimatorControllerParameterType _parType;
	
	public string GetParameterName() => _parameter;
	public void SetParameterName(string name) => _parameter = name;
	public AnimatorControllerParameterType GetParameterType() => _parType;
	
	private T _value;
	public T Value
	{
		get { return _value; }
		set
		{
			_value = value;
			if(_parameter != null && _animator != null)
			{
				SetAnimatorPar();
			}
			else
			{
				Debug.LogWarning($"{this} is not binded!");
			}
		}
	}
	
	public void Bind(string parameter, Animator animator, AnimatorControllerParameterType type)
	{
		if(type == AnimatorControllerParameterType.Bool && typeof(T) == typeof(bool)
			|| type == AnimatorControllerParameterType.Float && typeof(T) == typeof(float)
			|| type == AnimatorControllerParameterType.Int && typeof(T) == typeof(int))
		{
			_parType = type;
			_animator = animator;
			_parameter = parameter;
		}
	}
	
	public void Unbind()
	{
		_animator = null;
		_parameter = null;
	}
	
	private void SetAnimatorPar()
	{
		switch(_parType)
		{
		case AnimatorControllerParameterType.Bool:
			_animator.SetBool(_parameter, (bool)(object)_value);
			break;
		case AnimatorControllerParameterType.Float:
			_animator.SetFloat(_parameter, (float)(object)_value);
			break;
		case AnimatorControllerParameterType.Int:
			_animator.SetInteger(_parameter, (int)(object)_value);
			break;
		default:
			Debug.LogError($"Unsupported animator parameter type: {_parType}");
			break;
		}
	}
}
