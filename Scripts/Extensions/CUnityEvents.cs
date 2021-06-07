using System;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
    
    [Serializable]
    public class CUnityEvent : UnityEvent {}
    
    [Serializable]
    public class CUnityEventBool : UnityEvent<bool> {}
    
    [Serializable]
    public class CUnityEventFloat : UnityEvent<float> {}
    
    [Serializable]
    public class CUnityEventInt : UnityEvent<int> {}

    [Serializable]
    public class CUnityEventString : UnityEvent<string> {}

    [Serializable]
    public class CUnityEventGameObject : UnityEvent<GameObject> {}

    [Serializable]
    public class CUnityEventColor : UnityEvent<Color> {}

    [Serializable]
    public class CUnityEventTransform : UnityEvent<Transform> {}
    
    [Serializable]
    public class CUnityEventCollision : UnityEvent<Collision> {}

    [Serializable]
    public class CUnityEventCollision2D : UnityEvent<Collision2D> {}

}