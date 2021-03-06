using UnityEngine;

public class AI_Behaviour : StateMachineBehaviour
{
	public string LinkedStateName;

	public static event System.Action<Animator, AI_Behaviour> OnStateChange;
	protected virtual void Begin(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}
	protected virtual void Exit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}
	protected virtual void Tick(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){}
	
	public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		OnStateChange?.Invoke(animator, this);
		Begin(animator, stateInfo, layerIndex);
	}
	
	public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Exit(animator, stateInfo, layerIndex);
	}
	
	public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Tick(animator, stateInfo, layerIndex);
	}
}
