using UnityEngine;

public class MissilesExtraVFX : MonoBehaviour
{
    [Header("VFX Settings")]
    public GameObject missileVFXPrefab;
    public Transform vfxSpawnPoint;
    public float lifetime = 1.2f;

    public void Anim_Missiles()
    {
        SpawnVFX();
    }

    void SpawnVFX()
    {
        if (missileVFXPrefab == null || vfxSpawnPoint == null)
            return;

        GameObject vfx = Instantiate(missileVFXPrefab, vfxSpawnPoint.position, vfxSpawnPoint.rotation);

        Destroy(vfx, lifetime);
    }
}