using UnityEngine;
using System;


namespace RetinaNetworking
{
    [CreateAssetMenu(menuName = "Networking/Data Item")]
    [Serializable]
    public class Data : ScriptableObject
    {
        [SerializeField] int coomer;
        [SerializeField] float boomer;
        [SerializeField] string zoomer;
        [SerializeField] enumSample loomer;
    }

    public enum enumSample
    {
        bob,
        charlie,
        zed
    }
}

