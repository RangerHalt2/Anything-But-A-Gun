using System;
using UnityEngine;

[CreateAssetMenu(menuName = "PackAPunch/Type")]
public class PAP_ScriptableObjects : ScriptableObject
{
    [SerializeField] private string assemblyQualifiedTypeName;
    public string promotionName;
    public string promotionEffect;

    public Type GetTypeSafe()
    {
        return Type.GetType(assemblyQualifiedTypeName);
    }
}
