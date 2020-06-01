using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField] int height = 13;
    [SerializeField] int width = 13;
    [SerializeField] float _cellSize = 1.6f;

    public Transform TargetTransform { get; set; }
    public Transform FromTransform { get; set; }

    public Transform CenterSearchingZone { get; set; }
    
    private float maxWidth = 0;
    private float minWidth = 0;
    private float maxHeight = 0;
    private float minHeight = 0;

    private List<CellInfo> VisitedNodes = new List<CellInfo>();
    private List<CellInfo> SearchingNeighbour = new List<CellInfo>();
    

    private Vector3 from;

    [Serializable]
    private class CellInfo
    {
        public CellInfo(Vector3 position, float priority, float startDistance, CellInfo directionToNextCell, int iX, int iY) 
        { Position = position; Priority = priority; StartDistance = startDistance; DirectionToNextCell = directionToNextCell; this.iX = iX; this.iY = iY; }

        public Vector3 Position;
        public float Priority;
        public float StartDistance;
        public CellInfo DirectionToNextCell;
        public int iX;
        public int iY;
    }

    #region 3 Main methods, which pathfinding

    #region StartPathfinding
    public List<Vector3> StartPathFinding(Vector3 from, Vector3 target,bool parallel = false) 
    {
        bool obstacles = ObstaclesRaycast(from, target);
        if(!obstacles) return null;

        
        VisitedNodes.Clear();
        SearchingNeighbour.Clear();

        (int iX, int iY) = IndexesPositionInGrid(from);
        (int endX, int endY) = IndexesPositionInGrid(target);

        Vector3 nextPos = new Vector3(minHeight + iY * _cellSize, from.y, minWidth + iX * _cellSize);
        if (CenterSearchingZone != null) nextPos += CenterSearchingZone.position;

        CellInfo NextSearch = new CellInfo(nextPos, Vector3.Distance(nextPos, from) + Vector3.Distance(nextPos, target), Vector3.Distance(nextPos, from), null, iX, iY);
        SearchingNeighbour.Add(NextSearch);

        if (parallel) { StepsParallelPathFinding(NextSearch, target); return parallelPath; }
        return StepsPathFinding(NextSearch, target);
    }
    #endregion
    [SerializeField] float deltadistanceToTargetForSearchStopping = 0.8f;


    #region Parallel Pathfinding
    public List<Vector3> parallelPath;
    private IEnumerator StepsParallelPathFinding(CellInfo currentCell, Vector3 target, int MaxLoops = 99999)
    {
        bool search = true;
        int iLoop = 0;

        while (search)
        {
            iLoop++;
            if (iLoop > MaxLoops) { throw new Exception("Fuck The Pathfinding"); search = false; }

            yield return new WaitForSeconds(0.01f);

            VisitedNodes.Add(currentCell);
            SearchingNeighbour.Remove(currentCell);

            if (Vector3.Distance(currentCell.Position, target) < (_cellSize + deltadistanceToTargetForSearchStopping))
            {
                if (!Physics.Raycast(currentCell.Position, target - currentCell.Position, Vector3.Distance(currentCell.Position, target)))
                {
                    parallelPath = PathBuild(); //
                    search = false;
                }
            }
            else
            {
                float priority;

                int iX = currentCell.iX;
                int iY = currentCell.iY;

                for (int x = iX - 1; x <= iX + 1; x++)
                {
                    for (int y = iY - 1; y <= iY + 1; y++)
                    {
                        if (y >= 0 && y < height && x >= 0 && x < width /*&& !obstaclesGrid[x, y]*/)
                        {
                            Vector3 nextPos = new Vector3(minHeight + y * _cellSize, currentCell.Position.y, minWidth + x * _cellSize);
                            if (CenterSearchingZone != null) nextPos += CenterSearchingZone.position;
                            float distanceBtwNextAndCurrent = Vector3.Distance(nextPos, currentCell.Position) + currentCell.StartDistance;

                            bool visited = false;
                            foreach (CellInfo node in VisitedNodes) if (node.Position.Equals(nextPos)) visited = true;

                            if (!visited && !Physics.Raycast(currentCell.Position, nextPos - currentCell.Position, Vector3.Distance(currentCell.Position, nextPos)))
                            {
                                priority = distanceBtwNextAndCurrent + Vector3.Distance(nextPos, target);

                                int iSearching = -1000;
                                for (int i = 0; i < SearchingNeighbour.Count; i++) if (SearchingNeighbour[i].Position.Equals(nextPos)) iSearching = i;

                                if (iSearching == -1000) SearchingNeighbour.Add(new CellInfo(nextPos, priority, distanceBtwNextAndCurrent, currentCell, x, y));
                                else if (SearchingNeighbour[iSearching].Priority > priority) SearchingNeighbour[iSearching] = new CellInfo(nextPos, priority, distanceBtwNextAndCurrent, currentCell, x, y);

                            }
                        }
                    }
                }

                CellInfo NextSearch = ChoosingCell();
                if (NextSearch == null) { parallelPath = null; search = false; }

                currentCell = NextSearch;
            }
        }
    }
    #endregion

    #region Pathfinding
    private List<Vector3> StepsPathFinding(CellInfo currentCell, Vector3 target, int MaxLoops = 9999)
    {
        int iLoop = 0;
        //(int endX, int endY) = IndexesPositionInGrid(target);

        while (true)
        {
            iLoop++;
            if (iLoop > MaxLoops) { Debug.LogWarning("Fuck The Pathfinding"); return null; }

            VisitedNodes.Add(currentCell);
            SearchingNeighbour.Remove(currentCell);

            if (Vector3.Distance(currentCell.Position, target) < (_cellSize + deltadistanceToTargetForSearchStopping))
            {
                bool obstacles = ObstaclesRaycast(currentCell.Position, target);
                if (!obstacles) return PathBuild();
            }
            else
            {
                float priority;

                int iX = currentCell.iX;
                int iY = currentCell.iY;

                for (int x = iX - 1; x <= iX + 1; x++)
                {
                    for (int y = iY - 1; y <= iY + 1; y++)
                    {
                        if (y >= 0 && y < height && x >= 0 && x < width /*&& !obstaclesGrid[x, y]*/)
                        {
                            Vector3 nextPos = new Vector3(minHeight + y * _cellSize, currentCell.Position.y, minWidth + x * _cellSize);
                            if (CenterSearchingZone != null) nextPos += CenterSearchingZone.position;
                            float distanceBtwNextAndCurrent = Vector3.Distance(nextPos, currentCell.Position) + currentCell.StartDistance;
;
                            bool visited = false;
                            foreach (CellInfo node in VisitedNodes) if (node.Position.Equals(nextPos)) visited = true;


                            bool obstacles = ObstaclesRaycast(currentCell.Position, nextPos);
                            if (!visited && !obstacles)
                            {
                                priority = distanceBtwNextAndCurrent + Vector3.Distance(nextPos, target);
                                //priority = distanceBtwNextAndCurrent + (Mathf.Abs(x - endX) + Mathf.Abs(y - endY)) * 10;

                                int iSearching = -1000;
                                for (int i = 0; i < SearchingNeighbour.Count; i++) if (SearchingNeighbour[i].Position.Equals(nextPos)) iSearching = i;

                                if (iSearching == -1000) SearchingNeighbour.Add(new CellInfo(nextPos, priority, distanceBtwNextAndCurrent, currentCell, x, y));
                                else if (SearchingNeighbour[iSearching].Priority > priority) SearchingNeighbour[iSearching] = new CellInfo(nextPos, priority, distanceBtwNextAndCurrent, currentCell, x, y);
                            }
                        }
                    }
                }

                CellInfo NextSearch = ChoosingCell();
                if (NextSearch == null) return null;

                currentCell = NextSearch;
            }
        }
    }
    #endregion

    #region Построение пути среди оцененных точек
    private List<Vector3> PathBuild() 
    {
        List<Vector3> path = new List<Vector3>();

        CellInfo lastVisited = VisitedNodes[VisitedNodes.Count - 1];
        CellInfo Next;

        while (lastVisited != null)
        {
            Next = lastVisited.DirectionToNextCell;
            path.Add(lastVisited.Position);
            lastVisited = Next;
        }

        path.Reverse();

        return path;
    }
    #endregion

    #endregion

    public bool ObstaclesRaycast(Vector3 from, Vector3 to, Transform FromTransform, Transform TargetTransform)
    {
        RaycastHit[] hits = Physics.RaycastAll(from, to - from, Vector3.Distance(from, to));

        return ObstaclesRaycast(hits, FromTransform, TargetTransform);
    }
    private bool ObstaclesRaycast(Vector3 from, Vector3 to)
    {
        RaycastHit[] hits = Physics.RaycastAll(from, to - from, Vector3.Distance(from, to));

        return ObstaclesRaycast(hits);
    }
    private bool ObstaclesRaycast(RaycastHit[] hits,Transform FromTransform, Transform TargetTransform)
    {
        bool obstacles = false;
        foreach (RaycastHit hit in hits)
        {
            if (!(hit.transform.tag == "Tanks" && (hit.transform == FromTransform || hit.transform == TargetTransform))) obstacles = true;
        }
        return obstacles;
    }
    private bool ObstaclesRaycast(RaycastHit[] hits)
    {
        bool obstacles = false;
        foreach (RaycastHit hit in hits)
        {
            if (!(hit.transform.tag == "Tanks" && (hit.transform == FromTransform || hit.transform == TargetTransform))) obstacles = true;
        }
        return obstacles;
    }
    


    #region Выбор необработанной клетки, среди всех соседних клеток
    private CellInfo ChoosingCell() 
    {
        CellInfo NextSearch = null;
        float priority = float.MaxValue;
        foreach (CellInfo elem in SearchingNeighbour)
        {
            if (priority > elem.Priority)
            {
                priority = elem.Priority;
                NextSearch = elem;
            }
        }

        if (NextSearch == null) Debug.Log("Все возможные клетки просканированы, но путь не найден. Предположительно цель или агент, окружены стенами и недоступны");
        return NextSearch;
    }
    #endregion
    #region Расчёт индексов ячейки на поле,
    private (int, int) IndexesPositionInGrid(Vector3 position)
    {
        if (CenterSearchingZone != null) position -= CenterSearchingZone.position;

        if (position.y > _cellSize) return (-1, -1);

        if (maxHeight == 0) maxHeight = (height * _cellSize) / 2.0f;
        if (minHeight == 0) minHeight = (_cellSize - (height * _cellSize)) / 2.0f;
        if (maxWidth == 0) maxWidth = (width * _cellSize) / 2.0f;
        if (minWidth == 0) minWidth = (_cellSize - (width * _cellSize)) / 2.0f;

        position = new Vector3(position.x - minWidth, position.y, position.z - minHeight);
        int y = Mathf.RoundToInt(position.x / _cellSize);
        int x = Mathf.RoundToInt(position.z / _cellSize);
        return (x, y);
    }
    #endregion
}