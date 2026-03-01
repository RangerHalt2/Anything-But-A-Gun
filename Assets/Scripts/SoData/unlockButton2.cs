using UnityEngine;

[CreateAssetMenu]
public class unlockButton2 : ScriptableObject
{
	
	public bool _value;

	public bool Value
	{
		get { return _value; }
		set { _value = value; }
	}

}
