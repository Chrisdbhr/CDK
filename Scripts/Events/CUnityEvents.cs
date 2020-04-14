using System;
using UnityEngine;
using UnityEngine.Events;

namespace CDK
{
    [System.Serializable]
    public class CUnityEvent : UnityEvent {}
    
    [System.Serializable]
    public class CUnityEventBool : UnityEvent<bool> {}
    
    [System.Serializable]
    public class CUnityEventFloat : UnityEvent<float> {}
    
    [System.Serializable]
    public class CUnityEventInt : UnityEvent<int> {}

    [System.Serializable]
    public class CUnityEventString : UnityEvent<string> {}
    
    [System.Serializable]
    public class CUnityEventColor : UnityEvent<Color> {}

    
}