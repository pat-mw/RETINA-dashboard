using UnityEngine;
using System;

/// <summary>
/// THE Nested Data as a scriptable object -> containing nested scriptable objects, Did NOT function as expected
/// Instead of serializing the data contained within the nested SO - a reference to the SO was serialized.
/// This could not be circumvented by using Sirenix Serialiser because it refuses to work with UnityObjects.
/// Next attempt is to avoid the use of SO's and just use plain classes for data
/// </summary>

namespace RetinaNetworking
{
    [CreateAssetMenu(menuName = "Networking/Nested Data Item")]
    [Serializable]
    public class NestedDataSO : ScriptableObject
    {
        [SerializeField] int coomer;
        [SerializeField] float boomer;
        [SerializeField] Data zoomer;
    }
}

