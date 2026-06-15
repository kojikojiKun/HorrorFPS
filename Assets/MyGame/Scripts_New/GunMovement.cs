using UnityEngine;

public class GunMovement
{
    private Transform camPos;
    private Transform gunPivot;

    public GunMovement(Camera cam, Transform pivot)
    {
        camPos = cam.transform;
        gunPivot = pivot;
    } 

    public void FollowCamera()
    {
        //ƒJƒƒ‰‚ÌŒü‚«‚Ée‚ğ’Ç]‚³‚¹‚é.
        gunPivot.forward = camPos.forward;
    }
}
