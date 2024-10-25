using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using Player = Photon.Realtime.Player;
using UnityEngine.SceneManagement;

public class Damage : MonoBehaviourPunCallbacks
{
    private Renderer[] renderers;
    private int initHp = 100;
    public int currHp = 100;
    private Animator anim;
    private CharacterController cc;
    private readonly int hasdDie = Animator.StringToHash("Die");
    private readonly int hashRespawn = Animator.StringToHash("Respawn");
    private GameManager gameManager;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        currHp = initHp;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (currHp > 0 && coll.collider.CompareTag("BULLET"))
        {
            currHp -= 20;
            if (currHp <= 0)
            {
                if (photonView.IsMine)
                {
                    var actorNo = coll.collider.GetComponent<Bullet>().actorNumber;
                    Player lastShootPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNo);
                    string msg = string.Format("\n<color=#00ff00>{0}</color> is killed by <color=#ff0000>{1}</color>",
                        photonView.Owner.NickName, lastShootPlayer.NickName);
                    photonView.RPC("KillMessage", RpcTarget.All, msg);
                }
                
                StartCoroutine(PlayerDie());
            }
        }
    }

    [PunRPC]
    public void KillMessage(string msg)
    {
        gameManager.msgList.text += msg;
    }

    private IEnumerator PlayerDie()
    {
        cc.enabled = false;
        anim.SetBool(hashRespawn, false);
        anim.SetTrigger(hasdDie);
        yield return new WaitForSeconds(3.0f);

        anim.SetBool(hashRespawn, true);
        SetPlayerVisible(false);
        yield return new WaitForSeconds(1.5f);

        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        transform.position = points[idx].position;

        currHp = initHp;
        SetPlayerVisible(true);
        cc.enabled = true;
    }

    private void SetPlayerVisible(bool isVisible)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = isVisible;
        }
    }
}
