using UnityEngine;

// Data container
[System.Serializable]
public class AI_Data<T> :ILinkable
{
	[System.NonSerialized]
	private AI_Data<T> _linked = null;
	
	[field: SerializeField]
	private T _value;
	public T Value
    {
	    get
		{
			
			return _value;
		}
	    
	    set
		{
			_value = value;
			if(_linked != null)
			{
				_linked.SetValueLinked(_value);
			}
		}
    }
    
	public void SetValueLinked(T value)
	{
		_value = value;
	}
    
	public void Link(System.Object link)
	{
		if(link != this && link is AI_Data<T>)
			_linked = (AI_Data<T>)link;	
	}
}
