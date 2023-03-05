using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class FloorCalculator : MonoBehaviour
{
    public enum BuildingTypes
    {
        ASK = 0,
        Adm,
        Garage
    }

    [SerializeField] private List<BuildingZone> _buildingZones;
    [SerializeField] private float _defaultFirstFloorOffset;
    [SerializeField] private List<BuildingOffset> _buildingOffsets;
    [SerializeField] private float _defaultFloorHeight;
    [SerializeField] private List<BuildingHeights> _buildingHeights;

    public int CalculateFloor(Vector3 position)
    {
        var offset = _defaultFirstFloorOffset;
        BuildingTypes? building = null;

        foreach (var zone in _buildingZones)
        {
            if (position.AllDimensionsBiggerThan(zone.LowerPoint.position) &&
                position.AllDimensionsLowerThan(zone.UpperPoint.position))
            {
                int buildingOffsetIndex;
                if ((buildingOffsetIndex = _buildingOffsets.FindIndex(buildingOffset => buildingOffset.Building == zone.Building)) == -1)
                    continue;

                var buildingOffset = _buildingOffsets[buildingOffsetIndex];
                offset = buildingOffset.Offset;
                building = buildingOffset.Building;

                break;
            }
        }

        var floorHeight = _defaultFloorHeight;
        int buildingHeightIndex;
        if (building != null && (buildingHeightIndex = _buildingHeights.FindIndex(buildingHeight => buildingHeight.Building == building)) != -1)
            floorHeight = _buildingHeights[buildingHeightIndex].Height;

        return Mathf.CeilToInt((position.y - offset) / floorHeight);
    }
}

[Serializable]
public struct BuildingZone
{
    public FloorCalculator.BuildingTypes Building;
    public Transform LowerPoint;
    public Transform UpperPoint;
}

[Serializable]
public struct BuildingOffset
{
    public FloorCalculator.BuildingTypes Building;
    public float Offset;
}

[Serializable]
public struct BuildingHeights
{
    public FloorCalculator.BuildingTypes Building;
    public float Height;
}