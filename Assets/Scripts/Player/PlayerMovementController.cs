using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
   public float speed;
   public float jumpForce;
   private Rigidbody2D _rigidbody2D;

   private void Start()
   {
       _rigidbody2D=GetComponent<Rigidbody2D>();
   }

   private void Update()
   {
     transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal"),0,0)* speed * Time.deltaTime);
    
   }

   private void LateUpdate()
   {
       if (Input.GetKeyDown(KeyCode.Space))
       {
           Jump();
       }
   }

   private void Jump()
   {
      _rigidbody2D.AddForce(Vector2.up*jumpForce,ForceMode2D.Impulse);
   }
}
