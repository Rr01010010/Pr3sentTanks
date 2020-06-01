using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTankAI : BaseTank
{
    #region Поля с характеристиками танка и свойства для установки характеристик
    [Serializable]
    public struct Characteristics 
    {
        public int Health;
        public float Speed;
        public float HitDamage;
        public float ShootCooldawn;
        public float TowerRotationSpeed;
        public float BaseRotationSpeed;
        public float AnotherTankBuildingTime;
    }
    public Characteristics SetCharacteristics 
    {
        set 
        {
            Health = value.Health;
            MovementSpeed = value.Speed;
            hitDamage = value.HitDamage;
            shootCooldawn = value.ShootCooldawn;
            baseRotationSpeed = value.BaseRotationSpeed;
            towerRotationSpeed = value.TowerRotationSpeed;
        }
    }

    protected Vector3 lastObservedPlayerPosition = Vector3.zero;
    protected float towerRotationSpeed;
    protected float baseRotationSpeed;

    [SerializeField] protected float timeToUpdatePath = 2;
    #endregion

    #region Вызов поиска пути
    private List<Vector3> PathToPlayer = new List<Vector3>();
    protected void SearchPath(Vector3 target) 
    {
        SearchPathToTarget = target;
    }
    protected Vector3 SearchPathToTarget
    {
        set 
        {
            TanksManager.SingleManager.PathFindingSystem.FromTransform = transform;
            TanksManager.SingleManager.PathFindingSystem.TargetTransform = TanksManager.SingleManager.Player.transform;

            PathToPlayer = TanksManager.SingleManager.PathFindingSystem.StartPathFinding(transform.position, value);
        }
    }
    protected virtual IEnumerator UpdatePathToPlayer()
    {
        while (_health > 0 && TanksManager.SingleManager.Player!= null)
        {
            if(TanksManager.SingleManager.Player!=null) SearchPath(TanksManager.SingleManager.Player.transform.position);
            yield return new WaitForSeconds(timeToUpdatePath);
        }
    }
    #endregion

    #region Передвижение танка
    public Vector3 MoveToPosition { set => transform.Translate(value - transform.position); }
    public Vector3 PositionForMovingToIndex(int index,float timeIntervals = float.MinValue) 
    {
        if (timeIntervals < 0) timeIntervals = Time.deltaTime;
        return Vector3.MoveTowards(transform.position, PathToPlayer[index], MovementSpeed * timeIntervals);
    }
    //public Vector3 PositionForMovingToPosition(Vector3 pos, float timeIntervals = float.MinValue)
    //{
    //    if (timeIntervals < 0) timeIntervals = Time.deltaTime;
    //    return Vector3.MoveTowards(transform.position, pos, speed * timeIntervals);
    //}
    #endregion

    #region Выбор позиции для последующего перемещения
    protected Vector3 GetNextStep() 
    {
        if (ClearingPassedStepThereisStep()) return PathToPlayer[0];

        bool obstacles = TanksManager.SingleManager.PathFindingSystem.ObstaclesRaycast(transform.position,
            Target, transform, TanksManager.SingleManager.Player.transform);

        if (!obstacles) return Target + Vector3.Normalize(transform.position - Target) * 1.6f;
        else return transform.position;
    }

    public TankCharacter TargetPlayer { get; set; }
    //private BaseTank _targetPlayer = null;
    public Vector3 Target
    {
        get
        {
            if (TargetPlayer != null && TargetPlayer.Equals(TanksManager.SingleManager.Player)) return TargetPlayer.transform.position;
            else return _target;
        }
        set => _target = value;
    }
    private Vector3 _target;

    protected bool ClearingPassedStepThereisStep() 
    {
        if (PathToPlayer == null || PathToPlayer.Count == 0) return false;
        else 
        {
            Vector3 nextNode = PathToPlayer[0];
            if (PathToPlayer.Count > 1)
            {
                Vector3 next2Node = PathToPlayer[1];
                if (Vector3.Angle(next2Node - nextNode, transform.position - nextNode) < 70) 
                {
                    PathToPlayer.RemoveAt(0);
                    return ClearingPassedStepThereisStep();
                }
            }
            
            if (Vector3.Distance(nextNode, transform.position) < 0.005f)
            {
                PathToPlayer.RemoveAt(0);
                return ClearingPassedStepThereisStep();
            }
            else return true;
        }
    }

    //public Vector3 SearchingPositionAroundObject(Transform _object, float range) 
    //{
    //    return SearchingPositionAroundObject(_object.position,range); 
    //}
    //
    //public Vector3 SearchingPositionAroundObject(Vector3 _object, float range) 
    //{
    //    if (TanksManager.SingleManager.takenWayPoints == null) TanksManager.SingleManager.takenWayPoints = new List<int>();
    //
    //    if (takenIdx != -1)
    //    {
    //        TanksManager.SingleManager.takenWayPoints[takenIdx] = -1;
    //        takenIdx = -1;
    //    }
    //
    //    Vector3 SearchedPos = Vector3.zero;
    //    while (!(Bound1.x > SearchedPos.x && SearchedPos.x > Bound2.x) || !(Bound1.y > SearchedPos.y && SearchedPos.y > Bound2.y)) 
    //    {
    //        float x = UnityEngine.Random.Range(-range, range);
    //        float y = Mathf.Sqrt(range * range - x * x);
    //        float znak = UnityEngine.Random.Range(-1.1f, 1.1f);
    //        if (znak < 0) y = -y;
    //        SearchedPos = new Vector3(x, y, 0);
    //    }
    //    takenIdx = TanksManager.SingleManager.SearchingNearestWayPoint(SearchedPos);
    //    TanksManager.SingleManager.takenWayPoints.Add(takenIdx);
    //
    //    return Vector3.zero; 
    //}
    #endregion

    #region Метод которые может обозначить, видит ли вражеский танк игрока или нет
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
    #endregion

    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("plRocket"))
        {
            Destroy(other.gameObject);
            Health--;
        }
    }

    private bool _stopTankBehavior;
    public bool StopTankBehaviors 
    {
        get => _stopTankBehavior;
        set 
        {
            if (value) { if(gameObject!=null) Destroy(gameObject); }
            _stopTankBehavior = value;
        }
    }
}