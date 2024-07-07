using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticle : MonoBehaviour
{
    public event Action<BulletParticle> OnDestroyed;
    
    public void Deactivate() 
    {
        OnDestroyed?.Invoke(this);
    }
}
