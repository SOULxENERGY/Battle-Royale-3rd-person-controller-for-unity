using UnityEngine;


[System.Serializable]
public class Inventory 
{
    public int NoOfTimesLandedOnGround = 0;


    public void Increase()
    {
        NoOfTimesLandedOnGround++;
    }

   public Inventory()
    {

    }
}

