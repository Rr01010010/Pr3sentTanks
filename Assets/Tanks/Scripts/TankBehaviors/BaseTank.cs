using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BaseTank : MonoDestructionObjectsTanks
{

    public Transform TowerAxis { get => towerAxis; set => towerAxis = value; }
    [SerializeField] private Transform towerAxis = null;

    [SerializeField] protected Rocket _rocketPrefab = null;
    protected List<float> lastDistanses = null;
    protected List<bool> pluses = null;

    public float MovementSpeed;

    #region
    protected virtual float AngleBetweenTarget(Transform Axis, Vector3 target)
    {
        return AngleBetweenTarget(Axis, target, Vector3.forward);
    }
    protected virtual float AngleBetweenTarget(Transform Axis, Vector3 target, Vector3 rotFilter) 
    {
        if (Axis != null)
        {
            //проверка на нулрефы
            if (lastDistanses == null) lastDistanses = new List<float>();
            if (pluses == null) pluses = new List<bool>();

            //высчитываем позицию перед башней в равном удалении как и удаленность цели от башни
            Vector3 towerUp = Axis.position + Axis.forward * Vector3.Distance(Axis.position, target);

            return Vector3.Angle(towerUp - Axis.position, target - Axis.position);
        }
        return 0;
    }

    protected virtual void AxisRotate(Transform Axis, Vector3 target, int layer, float RotationSpeed = 30)
    {
        AxisRotate(Axis, target, Vector3.up, layer, RotationSpeed);
    }
    protected virtual void AxisRotate(Transform Axis, Vector3 target, Vector3 rotFilter, int layer,float RotationSpeed = 30)
    {
        if (Axis != null)
        {
            //проверка на нулрефы
            if (lastDistanses == null) lastDistanses = new List<float>();
            if (pluses == null) pluses = new List<bool>();
            while (lastDistanses.Count <= layer) { lastDistanses.Add(1000); pluses.Add(true); }

            //высчитываем угл между башней и целью, и корректируем под скорость поворота 
            //PS тут можно потенциально добавить анимации или инерцию поворота
            float angle = AngleBetweenTarget(Axis, target);

            //Самый хардкор здесь, я определяю сторону в которую нужно крутить башню
            if (angle > lastDistanses[layer]) pluses[layer] = !pluses[layer];
            lastDistanses[layer] = angle;

            if (angle > RotationSpeed * Time.deltaTime) angle = RotationSpeed * Time.deltaTime;            
            
            //Крутим башню в нужную сторону в зависимости от знака (plus)
            if (pluses[layer]) Axis.Rotate(rotFilter * -angle);
            else Axis.Rotate(rotFilter * angle);
        }
    }
    #endregion
    public Vector3 PositionForMovingToPosition(Vector3 pos, float timeIntervals = float.MinValue)
    {
        if (timeIntervals < 0) timeIntervals = Time.deltaTime;
        return Vector3.MoveTowards(transform.position, pos, MovementSpeed * timeIntervals);
    }

    protected float lastShootTime;
    protected float hitDamage;
    protected float shootCooldawn;

    [SerializeField] private Transform rocketSpawn;
    protected void Shoot(float hitDamage)
    {
        lastShootTime = Time.realtimeSinceStartup;
        Rocket rocket = Instantiate(_rocketPrefab, rocketSpawn.position, Quaternion.identity);
        rocket.Damage = hitDamage;
        rocket.transform.LookAt(rocketSpawn.position + TowerAxis.forward);// .rotation = Quaternion.LookRotation(TowerAxis.forward);
        rocket.Direction = TowerAxis.forward;        
    }

    protected bool ShouldShoot(bool noRaycast = false)
    {
        if (Time.realtimeSinceStartup - lastShootTime > shootCooldawn)
        {
            if (noRaycast) return true;
            Vector3 playerPos = TanksManager.SingleManager.Player.transform.position;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, playerPos - transform.position, Vector3.Distance(playerPos, transform.position));

            if (hits == null || hits.Length == 0 || hits.Length > 2)
            {
                return false;
            }
            else return true;
        }
        else return false;

    }
    public static bool PositionInViewField(Vector3 ViewPosition, Vector3 edgeVision1, Vector3 edgeVision2, Vector3 CheckingPos)
    {
        // узнали направления крайних точек зрения
        Vector3 direction1 = edgeVision1 - ViewPosition;
        Vector3 direction2 = edgeVision2 - ViewPosition;

        // узнали угол поля зрения  (половину)
        float viewAngle = Vector3.Angle(direction1, direction2) / 2;

        //узнали направление центральной точки взгляда и направление проверяемой точки
        direction1 = (direction1.normalized + direction2.normalized);
        direction2 = CheckingPos - ViewPosition;

        return Vector3.Angle(direction1, direction2) < viewAngle;
    }
}