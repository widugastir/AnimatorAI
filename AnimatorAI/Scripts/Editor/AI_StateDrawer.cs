using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[CustomEditor(typeof(AI_State), true)]
public class AI_StateDrawer : Editor
{
	[HideInInspector] public FieldInfo[] testFields;
	[HideInInspector] public List<string> testFieldNames;
	
	private bool b;
	private AI_State _target;
	private int[] _selected;
	private string[] _options;
	private AnimatorControllerParameter[] _parameters;
	private AnimatorController _runtimeAnimatorController;
	
	void OnEnable()
	{
		b = true; 
		_target = (AI_State)target;
		if(_target._animator != null)
			_runtimeAnimatorController = (_target._animator.runtimeAnimatorController as AnimatorController);
	}
	
	private void UpdateStats()
	{
		if(_target._animator == null) return;
		
		_runtimeAnimatorController = (_target._animator.runtimeAnimatorController as AnimatorController);
		testFields = _target.GetType()
			.GetFields(BindingFlags.Instance | BindingFlags.Public)
			.Where(field => field.GetCustomAttribute<AI_BindParameter>() != null)
			.ToArray();
			
		_selected = new int[testFields.Length];
		_parameters = _runtimeAnimatorController.parameters;
		
		testFieldNames = new List<string>();
		testFieldNames.Add("None");
		foreach(var f in _parameters)
		{
			testFieldNames.Add(f.name);
		}
		_options = testFieldNames.ToArray();
	}
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		b = GUILayout.Toggle(b, "Hide params");
		
		if(!b)
		{
			UpdateStats();
			if(_runtimeAnimatorController != null)
			{
				EditorGUILayout.BeginVertical();
				for(int i = 0; i < testFields.Length; i++)
				{
					FieldInfo f = testFields[i];
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.BeginHorizontal("box");
					
					IBindableParameter v = (IBindableParameter) testFields[i].GetValue(_target);
					
					if(f.FieldType.IsGenericType)
						EditorGUILayout.LabelField(f.FieldType.GetGenericArguments()[0].ToString().Replace("System.", "") + ":", GUILayout.Width(80));
					else
						EditorGUILayout.LabelField(f.FieldType.ToString().Replace("AI_", "") + ":", GUILayout.Width(80));
					EditorGUILayout.LabelField("" + f.Name, GUILayout.Width(100));
					int select = EditorGUILayout.Popup("", GetIndexByString(_options, v.GetParameterName()), _options, GUILayout.Width(150));
					
					EditorGUILayout.EndHorizontal();
					if (EditorGUI.EndChangeCheck())
					{
						IBindableParameter b = (IBindableParameter)f.GetValue(_target);
						if(select == 0)
						{
							b.Unbind();
						}
						else
						{
							b.Bind(_options[select], _target._animator, _parameters[select - 1].type);
						}
						EditorUtility.SetDirty(_target.gameObject);
					}
				}
				EditorGUILayout.EndVertical();
			}
			else
			{
				EditorGUILayout.HelpBox("Animator is null", MessageType.None);
			}
		}
	}
	
	private int GetIndexByString(string[] array, string field)
	{
		for(int i = 0; i < array.Length; i++)
		{
			if(array[i] == field)
				return i;
		}
		return 0;
	}
}
