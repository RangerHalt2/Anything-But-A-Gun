using UnityEngine;

public class P_FridayFunday : MonoBehaviour
{
    private bool isProjectile = false;
    private void Start()
    {
        if (isProjectile)
        {
            this.enabled = false;
        }
    }
}
