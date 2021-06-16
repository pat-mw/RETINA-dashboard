using UnityEngine;
using System;


/// <summary>
/// The Data SO was able to be serialized using Unity's native JSON utilities
/// However, nested data did not function as expected, so we will be trying plain c# classes
/// With plain c# classes we will use Sirenix Serialisation
/// </summary>

namespace RetinaNetworking
{
    [CreateAssetMenu(menuName = "Networking/Data Item")]
    [Serializable]
    public class DataSO : ScriptableObject
    {
        [SerializeField] int coomer;
        [SerializeField] float boomer;
        [SerializeField] string zoomer;
        [SerializeField] enumSample loomer;
    }
}

