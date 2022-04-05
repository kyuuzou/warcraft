using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class SpawnFactory : MonoBehaviour {

    [SerializeField]
    private Building buildingPrefab;

    [SerializeField]
    private BuildingData[] buildingDataPrefabs;

    [SerializeField]
    private Item[] itemPrefabs;

    [SerializeField]
    private Decoration meshDecorationPrefab;

    [SerializeField]
    private Decoration spriteDecorationPrefab;
    
    [SerializeField]
    private DecorationData[] decorationDataPrefabs;

    [SerializeField]
    private Unit unitPrefab;

    [SerializeField]
    private UnitData[] unitDataPrefabs;

    [SerializeField]
    private Transform buildingParent;

    [SerializeField]
    private Transform decorationParent;

    [SerializeField]
    private Transform projectileParent;
    public Transform ProjectileParent {
        get { return this.projectileParent; }
    }

    [SerializeField]
    private Transform unitParent;

    [SerializeField]
    private TraitData[] nonTraitData;

    private Map map;

    private Dictionary<BuildingType, BuildingData> buildingData;
    private Dictionary<DecorationType, DecorationData> decorationData;
    private Dictionary<UnitType, UnitData> unitData;

    private Dictionary<string, BuildingType> nameToBuildingType;
    private Dictionary<string, UnitType> nameToUnitType;

    private Dictionary<TileType, DecorationType> decorationByTileType;
    private Dictionary<ItemIdentifier, Item> itemByIdentifier;
    private Dictionary<Type, TraitData> traitByType;

    private int spawnCount = 0;

    private void Awake () {
        this.map = ServiceLocator.Instance.Map;

        this.nameToBuildingType = new Dictionary<string, BuildingType> ();
        this.nameToUnitType = new Dictionary<string, UnitType> ();

        this.InitializeSpawnableDictionaries ();
        this.InitializeTraitDictionary ();
    }

    private BuildingType GetBuildingType (string nameOnFile) {
        if (this.nameToBuildingType.ContainsKey (nameOnFile)) {
            return this.nameToBuildingType[nameOnFile];
        }

        return BuildingType.None;
    }

    public BuildingData GetData (BuildingType type) {
        if (this.buildingData.ContainsKey (type)) {
            return BuildingData.Instantiate (this.buildingData[type]);
        }

        Debug.Log ("BuildingType not found: " + type);
        return null;
    }

    public DecorationData GetData (DecorationType type) {
        if (this.decorationData.ContainsKey (type)) {
            return DecorationData.Instantiate (this.decorationData[type]);
        }

        return null;
    }

    public UnitData GetData (UnitType type) {
        if (this.unitData.ContainsKey (type)) {
            return UnitData.Instantiate (this.unitData[type]);
        }

        return null;
    }

    public T GetData<T> () where T : TraitData {
        return (T) this.traitByType[typeof (T)];
    }

    public DecorationType GetDecorationByTileType (TileType tileType) {
        if (! this.decorationByTileType.ContainsKey (tileType)) {
            return DecorationType.None;
        }

        return this.decorationByTileType[tileType];
    }

    public ItemIdentifier GetRandomItemIdentifier (params ItemIdentifier[] identifiers) {
        float total = 0;

        foreach (ItemIdentifier identifier in identifiers) {
            total += this.itemByIdentifier[identifier].Probability;
        }

        float random = Random.Range (0, total);
        total = 0;

        foreach (ItemIdentifier identifier in identifiers) {
            float probability = this.itemByIdentifier[identifier].Probability;

            if (probability == 0.0f) {
                continue;
            }

            total += probability;

            if (total >= random) {
                return identifier;
            }
        }

        return ItemIdentifier.None;
    }

    private UnitType GetUnitType (string nameOnFile) {
        if (this.nameToUnitType.ContainsKey (nameOnFile)) {
            return this.nameToUnitType[nameOnFile];
        }

        return UnitType.None;
    }

    private void InitializeSpawnableDictionaries () {
        this.buildingData = new Dictionary<BuildingType, BuildingData> ();
        this.decorationData = new Dictionary<DecorationType, DecorationData> ();
        this.decorationByTileType = new Dictionary<TileType, DecorationType> ();
        this.itemByIdentifier = new Dictionary<ItemIdentifier, Item> ();
        this.unitData = new Dictionary<UnitType, UnitData> ();

        foreach (BuildingData building in this.buildingDataPrefabs) {
            if (building == null) {
                continue;
            }

            this.buildingData[building.Type] = building;

            if (building.NameOnFile == string.Empty) {
#if CUSTOM_DEBUG
                Debug.LogWarning ("NameOnFile not defined for " + building.Type);
#endif
            } else {
                this.nameToBuildingType[building.NameOnFile] = building.Type;
            }
        }

        foreach (DecorationData decoration in this.decorationDataPrefabs) {
            this.decorationData[decoration.Type] = decoration;
            this.decorationByTileType[decoration.TileType] = decoration.Type;
        }

        foreach (UnitData unit in this.unitDataPrefabs) {
            if (unit == null) {
                continue;
            }

            this.unitData[unit.Type] = unit;

            if (unit.NameOnFile == string.Empty) {
#if CUSTOM_DEBUG
                Debug.LogWarning ("NameOnFile not defined for " + unit.Type);
#endif
            } else {
                this.nameToUnitType[unit.NameOnFile] = unit.Type;
            }
        }

        foreach (Item item in this.itemPrefabs) {
            this.itemByIdentifier[item.Identifier] = item;
        }
    }

    private void InitializeTraitDictionary () {
        this.traitByType = new Dictionary<Type, TraitData> ();

        foreach (TraitData data in this.nonTraitData) {
            if (data == null) {
                continue;
            }

            this.traitByType[data.GetType ()] = data;
        }
    }

    public SpawnableSprite Spawn (string type, Faction faction, Vector2Int position) {
        UnitType unitType = this.GetUnitType (type);

        if (unitType != UnitType.None) {
            return this.SpawnUnit (unitType, faction, position);
        }

        if (! Settings.Instance.Isometric) {
            BuildingType buildingType = this.GetBuildingType (type);
                
            if (buildingType != BuildingType.None) {
                return this.SpawnBuilding (buildingType, faction, position);
            }
        }

        return null;
    }

    public Building SpawnBuilding (BuildingType buildingType, Faction faction) {
        if (! this.buildingData.ContainsKey (buildingType)) {
            return null;
        }

        Building building = Transform.Instantiate (
            this.buildingPrefab,
            new Vector3 (0.0f, 0.0f, 5.0f),
            Quaternion.identity
        );

        this.spawnCount ++;
        building.transform.parent = this.buildingParent;
        building.name = string.Format ("{0} {1}", buildingType.ToString (), this.spawnCount);

        building.SetBuildingType (buildingType);
        building.Faction = faction;
        faction.AddBuilding (building);

        return building;
    }
    
    public Building SpawnBuilding (BuildingType type, Faction faction, Vector2Int position) {
        Building building = this.SpawnBuilding (type, faction);

        if (building == null) {
            return building;
        }

        //Exception for castles, which seem to be one tile to the right, for some reason
        if (type == BuildingType.HumanCastle || type == BuildingType.OrcCastle) {
            position.x --;
        }

        MapTile tile = this.map.GetTile (position.x, position.y);
        building.Initialize (tile);

        return building;
    }
    
    public Decoration SpawnDecoration (DecorationType type, Faction faction) {
        if (! this.GetData (type).Spawnable) {
            return null;
        }

        Decoration decoration = GameObject.Instantiate (
            type == DecorationType.Unknown ? this.spriteDecorationPrefab : this.meshDecorationPrefab
        );
        
        this.spawnCount ++;
        decoration.transform.parent = this.decorationParent;
        decoration.name = string.Format ("{0} {1}", type.ToString (), this.spawnCount);
        
        decoration.SetDecorationType (type);
        decoration.Faction = faction;

        return decoration;
    }
    
    public Decoration SpawnDecoration (DecorationType type, Faction faction, Vector2Int position) {
        if (this.GetData (type) == null) {
            Debug.LogError ("No data found for: " + type);
            return null;
        }

        Decoration decoration = this.SpawnDecoration (type, faction);

        if (decoration == null) {
            return null;
        }
        
        MapTile tile = this.map.GetTile (position.x, position.y);
        decoration.Initialize (tile);

        return decoration;
    }
    
    public Decoration SpawnDecoration (TileType type, Faction faction, Vector2Int position) {
        if (this.decorationByTileType.ContainsKey (type)) {
            return this.SpawnDecoration (this.decorationByTileType[type], faction, position);
        }

        return null;
    }

    /// <param name="relativePosition">Relative position from the center of the tile.</param>
    public Item SpawnItem (ItemIdentifier identifier, Vector2Int position, Vector3 relativePosition) {
        Item item = Item.Instantiate<Item> (this.itemByIdentifier[identifier]);

        item.InitializeExternals ();
        item.SetTile (this.map.GetTile (position));
        //item.RelativePosition = relativePosition;

        return item;
    }

    public Unit SpawnUnit (UnitType unitType, Faction faction) {
        Unit unit = GameObject.Instantiate (this.unitPrefab);

        this.spawnCount ++;
        unit.transform.parent = this.unitParent;
        unit.name = string.Format ("{0} {1}", unitType.ToString (), this.spawnCount);

        unit.Faction = faction;
        unit.SetUnitType (unitType);

        faction.AddUnit (unit);

        return unit;
    }
    
    public Unit SpawnUnit (UnitType type, Faction faction, Vector2Int position) {
        if (this.GetData (type) == null) {
            Debug.LogError ("No data found for: " + type);
            return null;
        }

        Unit unit = this.SpawnUnit (type, faction);

        if (unit == null) {
            return null;
        }

        MapTile tile = this.map.GetTile (position.x, position.y);
        unit.Initialize (tile);
        this.map.AddUnit (unit);

        return unit;
    }
}
