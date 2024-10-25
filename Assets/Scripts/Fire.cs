using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fire : MonoBehaviour
{
    public Transform firePos;
    public GameObject bulletPrefab;
    private ParticleSystem muzzleFlash;
    private PhotonView pv;
    private bool isMouseClick => Input.GetMouseButtonDown(0);

    void Start()
    {
        pv = GetComponent<PhotonView>();
        muzzleFlash = firePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();    
    }

    void Update()
    {
        if (pv.IsMine && isMouseClick)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                FireBullet(pv.Owner.ActorNumber);
                pv.RPC("FireBullet", RpcTarget.Others, pv.Owner.ActorNumber);
            }
        }
    }

    [PunRPC]
    public void FireBullet(int actorNo)
    {
        if (!muzzleFlash.isPlaying) muzzleFlash.Play(true);
        GameObject bullet = Instantiate(bulletPrefab,
                                    firePos.position,
                                    firePos.rotation);
        bullet.GetComponent<Bullet>().actorNumber = actorNo;
    }
}
