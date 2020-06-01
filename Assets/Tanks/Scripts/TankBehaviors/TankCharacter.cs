using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCharacter : BaseTank
{
    [SerializeField] Rigidbody myRigid;
    [SerializeField] float baseRotationSpeed;
    [SerializeField] float towerRotationSpeed;
    public float HitDamage;
    public float ShootCooldawn;
    void Start()
    {
        hitDamage = HitDamage;
        shootCooldawn = ShootCooldawn;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    Vector3 lastTarget = Vector3.zero;
    void MoveForward() 
    {
        //Debug.Log($"Default = {LayerMask.GetMask("Default")}");
        //Debug.Log($"Transparent = {LayerMask.GetMask("TransparentFX")}");
        //Debug.Log($"Ignore Raycast = {LayerMask.GetMask("Ignore Raycast")}");
        //Debug.Log($"Water = {LayerMask.GetMask("Water")}");
        //Debug.Log($"UI = {LayerMask.GetMask("UI")}");
        //Debug.Log($"Tanks = {LayerMask.GetMask("Tanks")}");

        myRigid.AddForce(transform.forward * MovementSpeed);
        //transform.position += transform.up * MovementSpeed * Time.deltaTime;
    }
    void MoveBack() 
    {
        myRigid.AddForce(-transform.forward * MovementSpeed);
        //transform.position -= transform.up * MovementSpeed * Time.deltaTime;
    }

    void Rotate(bool left) 
    {
        float angle = baseRotationSpeed * Time.deltaTime;

        if (left) transform.Rotate(0, angle, 0);
        else transform.Rotate(0, -angle, 0);
    }

    void TransformRotate(Transform transform,float rotationSpeed, bool left)
    {
        float angle = rotationSpeed * Time.deltaTime;

        if (left) transform.Rotate(0, angle, 0);
        else transform.Rotate(0, -angle, 0);
    }

    protected void FixedUpdate()
    {
        if (myRigid.velocity.magnitude > 0.1f) myRigid.velocity -= myRigid.velocity * 0.25f;


        if (Input.GetKey(KeyCode.W)) MoveForward();

        else if (Input.GetKey(KeyCode.S)) MoveBack();

        if (Input.GetKey(KeyCode.A)) TransformRotate(transform, baseRotationSpeed, false);

        else if (Input.GetKey(KeyCode.D)) TransformRotate(transform, baseRotationSpeed, true);


        if (Input.GetKey(KeyCode.Q)) TransformRotate(TowerAxis, towerRotationSpeed, false);

        else if (Input.GetKey(KeyCode.E)) TransformRotate(TowerAxis, towerRotationSpeed, true);

        
        
        if (Input.GetMouseButton(0) && ShouldShoot(true)) Shoot(hitDamage);
    }
    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Rocket"))
        {
            Destroy(other.gameObject);
            Health--;
        }
    }
}
