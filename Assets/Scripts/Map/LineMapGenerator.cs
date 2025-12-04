using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class LineMapGenerator : MonoBehaviour
{
    [Header("Room Prefabs")]
    [SerializeField] private GameObject[] spawnRoomPrefabs;
    [SerializeField] private GameObject[] bossRoomPrefabs;
    [SerializeField] private GameObject[] roomSections;
    [SerializeField] private GameObject[] specialRooms;

    [Header("Room Counts")]
    [SerializeField] private int minRooms = 9;
    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int specialRoomCount = 2;

    [Header("Special Room Timing")]
    [Tooltip("Earliest possible index for the first special room.")]
    [SerializeField] private int firstSpecialMin = 2;

    [Tooltip("Latest possible index for the first special room (inclusive).")]
    [SerializeField] private int firstSpecialMax = 4;

    [Tooltip("How many rooms cannot contain a special room after placing one.")]
    [SerializeField] private int delayBetweenSpecials = 4;

    [Tooltip("After the delay, how many rooms long the window is where the next special MUST spawn.")]
    [SerializeField] private int windowAfterDelay = 2;

    
    [Header("NavMesh")]
    [SerializeField] private NavMeshSurface navMeshSurface;


    private void Start()
    {
        GenerateRooms();
    }

    private void GenerateRooms()
    {
        int totalRooms = Random.Range(minRooms, maxRooms + 1);

        GameObject spawnRoomPrefab = spawnRoomPrefabs[Random.Range(0, spawnRoomPrefabs.Length)];
        GameObject spawnRoom = Instantiate(spawnRoomPrefab, Vector3.zero, spawnRoomPrefab.transform.rotation);

        Transform currentEndPoint = spawnRoom.transform.Find("EndPoint");
        if (currentEndPoint == null)
        {
            Debug.LogError("SpawnRoom is missing EndPoint!");
            return;
        }

        List<GameObject> middleRooms = new List<GameObject>();
        List<GameObject> availableRooms = new List<GameObject>(roomSections);

        for (int i = 0; i < totalRooms - 2; i++)
        {
            if (availableRooms.Count == 0)
                availableRooms = new List<GameObject>(roomSections);

            int randomIndex = Random.Range(0, availableRooms.Count);
            GameObject selected = availableRooms[randomIndex];
            availableRooms.RemoveAt(randomIndex);

            middleRooms.Add(selected);
        }

        int numSpecialRooms = Mathf.Min(specialRoomCount, specialRooms.Length);

        List<GameObject> chosenSpecials = new List<GameObject>(specialRooms);
        ShuffleList(chosenSpecials);
        chosenSpecials = chosenSpecials.GetRange(0, numSpecialRooms);

        List<int> insertIndices = new List<int>();

        int minIndex = firstSpecialMin;
        int maxIndex = firstSpecialMax;

        for (int i = 0; i < numSpecialRooms; i++)
        {
            maxIndex = Mathf.Min(maxIndex, middleRooms.Count - 1);

            if (minIndex > maxIndex)
                minIndex = maxIndex;

            int chosenIndex = Random.Range(minIndex, maxIndex + 1);
            insertIndices.Add(chosenIndex);

            minIndex = chosenIndex + delayBetweenSpecials;
            maxIndex = minIndex + windowAfterDelay;
        }

        insertIndices.Sort();

        int offset = 0;
        for (int i = 0; i < insertIndices.Count; i++)
        {
            int insertAt = insertIndices[i] + offset;
            middleRooms.Insert(insertAt, chosenSpecials[i]);
            offset++;
        }

        foreach (GameObject roomPrefab in middleRooms)
        {
            currentEndPoint = SpawnMiddleRoom(roomPrefab, currentEndPoint);
        }

        GameObject bossRoomPrefab = bossRoomPrefabs[Random.Range(0, bossRoomPrefabs.Length)];
        GameObject bossRoom = Instantiate(bossRoomPrefab);

        Transform bossStart = bossRoom.transform.Find("StartPoint");
        if (bossStart == null)
        {
            Debug.LogError("BossRoom is missing StartPoint!");
            return;
        }

        Vector3 offsetPos = currentEndPoint.position - bossStart.position;
        bossRoom.transform.position += offsetPos;

        Debug.Log($"Generated {middleRooms.Count} rooms total with {numSpecialRooms} special rooms.");

        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogWarning("NavMeshSurface reference is missing!");
        }
    }

    private Transform SpawnMiddleRoom(GameObject sectionPrefab, Transform currentEnd)
    {
        GameObject newSection = Instantiate(sectionPrefab);

        Transform newStart = newSection.transform.Find("StartPoint");
        Transform newEnd = newSection.transform.Find("EndPoint");

        if (newStart == null || newEnd == null)
        {
            Debug.LogError($"StartPoint or EndPoint missing on {sectionPrefab.name}");
            return currentEnd;
        }

        Quaternion rotationOffset = Quaternion.FromToRotation(newStart.forward, currentEnd.forward);
        newSection.transform.rotation = rotationOffset * newSection.transform.rotation;

        Vector3 positionOffset = currentEnd.position - newStart.position;
        newSection.transform.position += positionOffset;

        return newEnd;
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}