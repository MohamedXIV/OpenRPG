using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Attack(RaycastHit hit, float weaponRange, Mover mover)
        {
            Debug.Log("Take this you crepy sphere");
            if (Input.GetMouseButtonDown(0) &&
                Vector3.Distance(transform.position, hit.point) < weaponRange && hit.transform != null)
            {
                mover.MoveTo(hit.transform.position);
            }
            else
            {
                mover.Stop();
            }
        }
    }
}