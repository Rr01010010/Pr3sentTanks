using UnityEngine;

public class GridPlaceholder : MonoBehaviour
{
    #region Grid
    public float CellSize
    {
        get 
        {
            if (_cellSize < 0.03f) _cellSize = 0.03f;
            return _cellSize;
        }
        set => _cellSize = value;
    }

    [Header("Grid Settings")]
    [SerializeField] float _cellSize;
    [SerializeField] int height;
    [SerializeField] int width;
    [SerializeField] string numberCells;
    private void GizmoDrawGrid()
    {
        Gizmos.color = Color.white;
        numberCells = (height * width).ToString();

        int y = 0;
        float maxHeight = (height * CellSize) / 2.0f;
        float minHeight = -(height * CellSize) / 2.0f;
        float maxWidth = (width * CellSize) / 2.0f;
        float minWidth = -(width * CellSize) / 2.0f;

        for (int h = 0; h < height / 2; h++)
        {
            Gizmos.DrawLine(new Vector3(minWidth, y, CellSize / 2 + h * CellSize), new Vector3(maxWidth, y, CellSize / 2 + h * CellSize));
            Gizmos.DrawLine(new Vector3(minWidth, y, -CellSize / 2 - h * CellSize), new Vector3(maxWidth, y, -CellSize / 2 - h * CellSize));
        }
        for (int w = 0; w < width / 2; w++)
        {
            Gizmos.DrawLine(new Vector3(CellSize / 2 + w * CellSize, y, minHeight), new Vector3(CellSize / 2 + w * CellSize, y, maxHeight));
            Gizmos.DrawLine(new Vector3(-CellSize / 2 - w * CellSize, y, minHeight), new Vector3(-CellSize / 2 - w * CellSize, y, maxHeight));
        }
    }
    #endregion
    public bool DrawGrid = true;
    public bool boolDrawSight = true;
    private void OnDrawGizmos()
    {
        if (DrawGrid) { GizmoDrawGrid(); }

        if (boolDrawSight) { DrawSight(); }
    }

    private void Update()
    {
        CreateNodeOnClick();
        PickNextPrefab();
        ChangeLevelInstatinating();
    }
    [Header("Links")]
    [SerializeField] TopMouseControl MouseContol;
    [SerializeField] PrefabsSO TanksPrefabsSO;
    [SerializeField] Transform Container;
    [Header("Pick prefab")]
    [SerializeField] int _numbPrefabInSO;
    [SerializeField] Transform ShowPrefabPosition;

    public int NumbPrefabInSO
    {
        get
        {
            if (_numbPrefabInSO < 0) _numbPrefabInSO = TanksPrefabsSO.Prefabs.Count - 1;
            if (_numbPrefabInSO > TanksPrefabsSO.Prefabs.Count - 1) _numbPrefabInSO = 0;
            return _numbPrefabInSO;
        }
        set => _numbPrefabInSO = value;
    }
    private void DrawSight()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(MouseContol.Sight, new Vector3(CellSize * 0.25f, CellSize * 0.25f, CellSize * 0.25f));
    }
    private void CreateNodeOnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 Sight = MouseContol.Sight;

            Sight.x = ((int)((MouseContol.Sight.x + Mathf.Sign(MouseContol.Sight.x) * CellSize / 2) / CellSize)) * CellSize;
            Sight.y = ((int)((MouseContol.Sight.y + Mathf.Sign(MouseContol.Sight.y) * CellSize / 2) / CellSize)) * CellSize;
            Sight.z = ((int)((MouseContol.Sight.z + Mathf.Sign(MouseContol.Sight.z) * CellSize / 2) / CellSize)) * CellSize;

            bool notDetect = true;
            for (int i = 0; notDetect && i < Container.childCount; i++) 
            {
                if (Container.GetChild(i).position.Equals(Sight)) 
                {
                    Destroy(Container.GetChild(i).gameObject);
                    notDetect = false;
                }
            }
            if (notDetect) Instantiate(TanksPrefabsSO.Prefabs[NumbPrefabInSO], Sight, Quaternion.identity, Container);
        }
    }
    private void PickNextPrefab()
    {
        float dScroll = Input.mouseScrollDelta.y;
        if (ShowPrefabPosition.childCount == 0) Instantiate(TanksPrefabsSO.Prefabs[NumbPrefabInSO], ShowPrefabPosition);

        if (dScroll != 0) 
        {
            if (dScroll > 0) NumbPrefabInSO -= 1;
            else NumbPrefabInSO += 1;

            if(ShowPrefabPosition.childCount == 1) Destroy(ShowPrefabPosition.GetChild(0).gameObject);
            
            Instantiate(TanksPrefabsSO.Prefabs[NumbPrefabInSO], ShowPrefabPosition);
        }
    }

    private void ChangeLevelInstatinating()
    {
        if (Input.GetKeyDown(KeyCode.Q)) MouseContol.HeightOfNewPoint += CellSize;
        
        if(Input.GetKeyDown(KeyCode.E)) MouseContol.HeightOfNewPoint -= CellSize;

    }
}