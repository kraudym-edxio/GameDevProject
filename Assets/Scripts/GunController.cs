using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // Player Gun and Ammo Variables
    public int gun = 1;
    public int ammoPistol = 6; // gun 1 = peas
    public int ammoSMG = 10; // gun 2 = corn
    public int ammoShotgun = 4; // gun 3 = grains/wheat
    public int ammoSniper = 6; // gun 4 = sunflower seeds
    
    // ********** Player Shooting **********
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gun = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gun = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gun = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            gun = 4;
        }

        // Shooting
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch (gun)
            {
                case 1:
                {
                    if (ammoPistol >= 1)
                    {
                        shootGun();
                    }

                    break;
                }
                case 2:
                {
                    if (ammoSMG >= 1)
                    {
                        shootGun();
                    }

                    break;
                }
                case 3:
                {
                    if (ammoShotgun >= 1)
                    {
                        shootGun();
                    }

                    break;
                }
                case 4:
                {
                    if (ammoSniper >= 1)
                    {
                        shootGun();
                    }

                    break;
                }
            }
        }
    }

    private void shootGun()
    {
        
    }
}
