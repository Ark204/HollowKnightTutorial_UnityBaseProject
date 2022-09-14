using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldField : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player Enter Field");
            Physics2D.IgnoreLayerCollision(6, 8);//…Ë÷√Œﬁµ–
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player Exit Field");
            Physics2D.IgnoreLayerCollision(6, 8,false);//“∆≥˝Œﬁµ–
        }
    }
}
