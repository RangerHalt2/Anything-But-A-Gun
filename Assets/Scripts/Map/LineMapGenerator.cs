using UnityEngine;

public class SectionGenerator : MonoBehaviour
{
    [SerializeField] private GameObject spawnRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private GameObject[] roomSections;

    [SerializeField] private int minRooms = 9;
    [SerializeField] private int maxRooms = 15;

    private void Start()
    {
        GenerateRooms();
    }

    private void GenerateRooms()
    {
        int totalRooms = Random.Range(minRooms, maxRooms + 1);

        GameObject spawnRoom = Instantiate(spawnRoomPrefab, Vector3.zero, spawnRoomPrefab.transform.rotation);
        Transform currentEndPoint = spawnRoom.transform.Find("EndPoint");
        if (currentEndPoint == null) 
        {
            Debug.LogError("SpawnRoom is missing EndPoint!");
        }

        for (int i = 0; i < totalRooms - 2; i++)
        {
            int randomIndex = Random.Range(0, roomSections.Length);
            GameObject middleRoom = roomSections[randomIndex];
            currentEndPoint = SpawnMiddleRoom(middleRoom, currentEndPoint);
        }

        GameObject bossRoom = Instantiate(bossRoomPrefab);
        Transform bossStart = bossRoom.transform.Find("StartPoint");
        if (bossStart == null)
        {
            Debug.LogError("BossRoom is missing StartPoint!");
        }

        Vector3 offset = currentEndPoint.position - bossStart.position;
        bossRoom.transform.position += offset;
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
}