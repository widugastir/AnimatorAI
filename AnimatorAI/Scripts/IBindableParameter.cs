using UnityEngine;

// Interface for link AI_DataParameter with animator parameters
public interface IBindableParameter
{
	public void Bind(string parameter, Animator animator, AnimatorControllerParameterType type);
	public void Unbind();
	public string GetParameterName();
	public void SetParameterName(string name);
	public AnimatorControllerParameterType GetParameterType();
}
