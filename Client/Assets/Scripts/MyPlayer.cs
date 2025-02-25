using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
	NetworkManager _network;

	void Start()
    {
		StartCoroutine(nameof(CoSendPacket));
		_network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
	}

    void Update()
    {
		if(Input.GetKey(KeyCode.W))
		{
			transform.position += new Vector3(0.0f, 0.0f, 0.1f);
		}

		if(Input.GetKey(KeyCode.S))
		{
			transform.position -= new Vector3(0.0f, 0.0f, 0.1f);
		}

		if(Input.GetKey(KeyCode.A))
		{
			transform.position -= new Vector3(0.1f, 0.0f, 0.0f);
		}

		if(Input.GetKey(KeyCode.D))
		{
			transform.position += new Vector3(0.1f, 0.0f, 0.0f);
		}
	}

	IEnumerator CoSendPacket()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.017f); // fps60 기준 1프레임

			ClientMove movePacket = new ClientMove();
			movePacket.posX = transform.position.x;
			movePacket.posY = transform.position.y;
			movePacket.posZ = transform.position.z;
			_network.Send(movePacket.Write());
		}
	}
}
