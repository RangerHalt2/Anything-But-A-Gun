using System;
using UnityEngine;

[CreateAssetMenu(menuName = "PackAPunch/Type")]
public class PAP_ScriptableObjects : ScriptableObject
{
    [SerializeField] private string assemblyQualifiedTypeName;

    public Type GetTypeSafe()
    {
        return Type.GetType(assemblyQualifiedTypeName);
    }
}
