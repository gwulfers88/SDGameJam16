using UnityEngine;
using System.Collections;

public class BulletNetwork : Photon.MonoBehaviour
{
    Vector3 pos;
    Quaternion rot;

	// Use this for initialization
	void Start ()
    {
        if(photonView.isMine)
        {
            pos = transform.position;
            rot = transform.rotation;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo messageInfo)
    {
        if (stream.isWriting)
        {
            Debug.Log("My bullet");
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.isReading)
        {
            Debug.Log("Their bullet");
            pos = (Vector3)stream.ReceiveNext();
            rot = (Quaternion)stream.ReceiveNext();
        }
    }
}
