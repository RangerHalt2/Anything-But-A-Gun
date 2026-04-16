using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Quaternion ShakeOffset { get; private set; } = Quaternion.identity;


    [System.Serializable]
    public struct ShakeConfig
    {
        public float duration;       // How long the shake lasts
        public float impactX;        // Rotation strength on X axis (pitch)
        public float impactY;        // Rotation strength on Y axis (yaw)
        public float impactZ;        // Roll strength on Z axis
        public float frequencyZ;     // Back-and-forth speed on Z
    }

    private ShakeConfig _config;
    private float _elapsed;
    private bool _shaking;
    private Quaternion _originalRotation;

    void Update()
    {
        if (!_shaking)
        {
            ShakeOffset = Quaternion.identity;
            return;
        }

        _elapsed += Time.deltaTime;

        if (_elapsed >= _config.duration)
        {
            _shaking = false;
            return;
        }

        // Normalized time 0→1
        float t = _elapsed / _config.duration;

        // Smooth decay: starts strong, eases out
        float decay = 1f - Mathf.SmoothStep(0f, 1f, t);

        // X and Y: random noise scaled by impact and decay
        float rotX = (Random.value * 2f - 1f) * _config.impactX * decay;
        float rotY = (Random.value * 2f - 1f) * _config.impactY * decay;

        // Z: oscillates back and forth like a pendulum
        float rotZ = Mathf.Sin(_elapsed * _config.frequencyZ * Mathf.PI * 2f)
                     * _config.impactZ * decay;

        ShakeOffset = Quaternion.Euler(rotX, rotY, rotZ);
    }

    /// <summary>
    /// Trigger a camera shake. Safe to call while already shaking —
    /// the new shake replaces the current one.
    /// </summary>
    public void Shake(ShakeConfig config)
    {
        _config = config;
        _elapsed = 0f;
        _shaking = true;
    }

}
