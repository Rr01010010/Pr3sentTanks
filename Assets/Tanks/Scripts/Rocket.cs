using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoDestructionObjectsTanks
{
    public float Damage;
    [SerializeField] float speedRocket;
    public Vector3 Direction {get;set;}
    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + Direction.normalized * speedRocket * Time.deltaTime, Color.cyan);

        transform.position += Direction.normalized * speedRocket * Time.deltaTime;
    }
    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
        if (other.gameObject.tag.Equals("Obstacles")) Destroy(gameObject);
    }
}
