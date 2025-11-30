using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlacementSpot : MonoBehaviour, IPointerClickHandler
{
    public static readonly List<PlacementSpot> AllSpots = new();

    [Header("Build")]
    public bool occupied;
    public Transform mountPoint;
    public BuildManager buildManager;

    [Header("Tile Visual")]
    public MeshRenderer tileRenderer;
    public Material tileBaseMaterial;
    public Color freeColor = new Color(0.2f, 0.8f, 0.2f, 0.7f);    
    public Color occupiedColor = new Color(0.4f, 0.4f, 0.4f, 0.7f);

    private Material _runtimeMat;

    GameObject _placedUnitInstance;
    public GameObject placedUnitPrefab;

    void Awake()
    {
        if (tileRenderer == null)
        {
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
            tile.name = "TileVisual";
            tile.transform.SetParent(transform, false);

            tile.transform.localPosition = new Vector3(0f, 0.01f, 0f);
            tile.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            tile.transform.localScale = new Vector3(1f, 1f, 1f);

            Collider col = tile.GetComponent<Collider>();
            if (col != null) Destroy(col);

            tileRenderer = tile.GetComponent<MeshRenderer>();
        }

        if (tileRenderer != null)
        {
            if (tileBaseMaterial != null)
            {
                _runtimeMat = new Material(tileBaseMaterial);
                tileRenderer.material = _runtimeMat;
            }
            else
            {
                _runtimeMat = tileRenderer.material;
            }
        }
    }


    void OnEnable()
    {
        AllSpots.Add(this);
        UpdateTileColor();
    }

    void OnDisable()
    {
        AllSpots.Remove(this);
    }

    void UpdateTileColor()
    {
        if (_runtimeMat == null) return;

        _runtimeMat.color = occupied ? occupiedColor : freeColor;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (occupied)
        {
            Debug.Log("[PlacementSpot] Spot already occupied.");
            return;
        }

        if (buildManager == null)
        {
            Debug.LogError("[PlacementSpot] buildManager is NOT assigned!");
            return;
        }

        buildManager.TryBuildAt(this);
    }

    public void PlaceUnit(GameObject unitPrefab)
    {
        Transform mp = mountPoint != null ? mountPoint : transform;

        _placedUnitInstance = Instantiate(unitPrefab, mp.position, mp.rotation);
        placedUnitPrefab = unitPrefab;
        occupied = true;

        UpdateTileColor();
        Debug.Log($"[PlacementSpot] Placed unit {unitPrefab.name} on {name}");
    }

    public void ResetUnitInstance()
    {
        if (placedUnitPrefab == null)
        {
            occupied = false;
            UpdateTileColor();
            return;
        }

        if (_placedUnitInstance != null)
        {
            Destroy(_placedUnitInstance);
        }

        Transform mp = mountPoint != null ? mountPoint : transform;
        _placedUnitInstance = Instantiate(placedUnitPrefab, mp.position, mp.rotation);
        occupied = true;

        UpdateTileColor();
        Debug.Log($"[PlacementSpot] Reset unit {placedUnitPrefab.name} on {name}");
    }
}

