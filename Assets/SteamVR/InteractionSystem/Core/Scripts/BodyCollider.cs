using UnityEngine;
using System.Collections;


[RequireComponent( typeof( CapsuleCollider ) )]
public class BodyCollider : MonoBehaviour
{
	[SerializeField] Transform head;
	//[SerializeField] Rigidbody rb;

	//private CapsuleCollider capsuleCollider;

	void Awake()
	{
		//capsuleCollider = GetComponent<CapsuleCollider>();
		//rb = GetComponent<Rigidbody>();
	}


	void FixedUpdate()
	{
		/*float distanceFromFloor = Vector3.Dot( head.localPosition, Vector3.up );
		capsuleCollider.height = Mathf.Max( capsuleCollider.radius, distanceFromFloor );
		*//*Vector3 headpos = head.localPosition - 0.5f * distanceFromFloor * Vector3.up;
		headpos.y = rb.position.y;*//*
		capsuleCollider.center = Vector3.up * (capsuleCollider.height / 2f);*/
		//rb.MovePosition(headpos);
	}
}
