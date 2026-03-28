using UnityEngine;

public class P_Prolonged : MonoBehaviour
{

    public float modifierLength { get; private set; } = 1.5f;

    private BlackholeScript blackHole;
    private LavaPuddleScript lavaPuddle;
    private PootCloudScript fartCloud;

    
    void Start()
    {
        WeaponClass wc = GetComponent<WeaponClass>();
        if(wc != null && wc.GetType() != typeof(CameraWeaponScript))
        {
            this.enabled = false;
            return;
        }

        if ((blackHole = GetComponent<BlackholeScript>()) != null) BlackHoleCat();
        if ((lavaPuddle = GetComponent<LavaPuddleScript>()) != null) LavaLamp();
        if ((fartCloud = GetComponent<PootCloudScript>()) != null) WindBreaker();
    }


    private void LavaLamp() {
        lavaPuddle.puddleTimer *= modifierLength;
    }

    private void WindBreaker()
    {
        fartCloud.cloudTimer *= modifierLength;
    }

    private void BlackHoleCat()
    {
        blackHole.timer *= modifierLength;
    }

}
