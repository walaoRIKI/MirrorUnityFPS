using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapBaseGeneration;

public static class MapGenData
{
    public enum MapBaseType { city, forest, future }

    public static List<GameObject> bases = new List<GameObject>();

    public static int seed;
    public static int baseCount;

    public static List<int> mapBasesType = new List<int>();
    public static SortedDictionary<int, List<List<StructureData>>> baseContents = new SortedDictionary<int, List<List<StructureData>>>();

    public static Dictionary<int, List<BaseMapBuildingPathGen.PathPoint>> buildingPath = new Dictionary<int, List<BaseMapBuildingPathGen.PathPoint>>();

    #region Pack

    public static void Pack(out byte[] data) {

        //Prepare binary profile
        NetworkWriter writer = new NetworkWriter();
        writer.Write(seed);

        //Base count
        var baseCount = bases.Count;
        writer.Write(baseCount);

        writer.WriteList(mapBasesType);

        //Base Data
        for (int i = 0; i < baseCount; i++) {   //Base count

            //How many spaned building/tree/etc in this base?
            var spawnedObjectInBaseCount = baseContents[i].Count;
            writer.Write(spawnedObjectInBaseCount);

            //Loop through the spawned object List and store it
            for(int j = 0; j < spawnedObjectInBaseCount; j++) {

                WriteSpawnedObjectInBase(writer, baseContents[i][j]);

            }

        }

        //Path
        writer.WriteInt(buildingPath.Count);

        foreach(var pathData in buildingPath) {

            writer.WriteInt(pathData.Key);
            writer.WriteInt(pathData.Value.Count);

            for(int i = 0; i < pathData.Value.Count; i++) {

                writer.WriteVector3(pathData.Value[i].startPos);
                writer.WriteVector3(pathData.Value[i].endPos);

            }
        }

        data = writer.ToArray();

    }

    private static void WriteSpawnedObjectInBase(NetworkWriter networkWritter, List<StructureData> objectList) {

        var buildingCount = objectList.Count;
        networkWritter.Write(buildingCount);

        //Store each data
        for (int j = 0; j < buildingCount; j++) {

            StructureData structureData = objectList[j];

            networkWritter.Write(structureData.prefabIndex);
            networkWritter.Write(structureData.position);
            networkWritter.Write(structureData.rotation);

        }

    }

    #endregion

    #region Unpack

    public static void Unpack(byte[] data) {

        NetworkReader reader = new NetworkReader(data);

        seed = reader.ReadInt();

        //Bases
        baseCount = reader.ReadInt();

        mapBasesType = reader.ReadList<int>();

        for(int i = 0; i < baseCount; i++) {

            //How many list in this base
            var baseContentListCount = reader.ReadInt();

            baseContents.Add(i, new List<List<StructureData>>());

            //Load the spawned contest list
            for(int j = 0; j < baseContentListCount; j++) {

                baseContents[i].Add(new List<StructureData>());
                ReadSpawnedObjectInBase(reader, baseContents[i][j]);

            }

        }

        //Path
        var pathDicCount = reader.ReadInt();

        for(int i = 0; i < pathDicCount; i++) {

            var dicKey = reader.ReadInt();
            buildingPath.Add(dicKey, new List<BaseMapBuildingPathGen.PathPoint>());

            var pathCount = reader.ReadInt();

            for(int i2 = 0; i2 < pathCount; i2++) {

                BaseMapBuildingPathGen.PathPoint newPathPoint;
                newPathPoint.startPos = reader.ReadVector3();
                newPathPoint.endPos = reader.ReadVector3();

                buildingPath[dicKey].Add(newPathPoint);

            }

        }
    }

    private static void ReadSpawnedObjectInBase(NetworkReader networkReader, List<StructureData> targetList) {

        var buildingCount = networkReader.ReadInt();

        for (int i = 0; i < buildingCount; i++) {

            StructureData structureData;

            structureData.prefabIndex = networkReader.ReadInt();
            structureData.position = networkReader.ReadVector3();
            structureData.rotation = networkReader.ReadQuaternion();

            targetList.Add(structureData);
        }

    }

    #endregion
}
