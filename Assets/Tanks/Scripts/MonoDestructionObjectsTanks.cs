using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MonoDestructionObjectsTanks : MonoBehaviour
{
    [SerializeField] protected int _health = 1;
    public virtual int Health 
    {
        get => _health;
        set 
        {
            _health = value;
            if (_health <= 0) Destroy(this.gameObject);
        } 
    }
    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Rocket") || other.gameObject.tag.Equals("plRocket")) 
        {
            Destroy(other.gameObject);
            Health--;
        }
    }
}
