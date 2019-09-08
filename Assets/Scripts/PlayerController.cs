using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Combat
{
    public class PlayerController : MonoBehaviour
    {
        private RaycastHit hit;
        public float weaponRange = 2.0f;
        private Mover mover;
        //IEnumerable<int> layers = Enumerable.Range(0, 31);
        private string[] Interactable = { "Player", "Enemy", "Terrain"};
        Transform target;
        private Fighter fighter;
        private Ray MouseRay
        {
            get => Camera.main.ScreenPointToRay(Input.mousePosition);
        }


        // Start is called before the first frame update
        void Start()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
        }

        // Update is called once per frame
        void Update()
        {
            //MoveToCursor();
            //InteractWithCombat();
            InteractWith();
            Debug.DrawRay(MouseRay.origin, MouseRay.direction * 50);
        }

        public void MoveToCursor()
        {
            Mover mover = GetComponent<Mover>();
            if (Input.GetMouseButtonDown(0))
            {
                mover.MoveTo(hit.point);
                //target = null;
            }
        }


        private void RayCastCheckLayer(int layer)
        {
            //return Physics.Raycast(MouseRay, out hit, layer);
            switch (layer)
            {
                case 0:
                    Debug.Log("this is the player");
                    break;
                case 1:
                    Debug.Log("this is an enemy");
                    InteractWithCombat();
                    break;
                case 2:
                    Debug.Log("this is a terrain");
                    MoveToCursor();
                    break;
                default:
                    Debug.Log("We hit nothing");
                    break;
                        
            }
        }

        private void InteractWithCombat()
        {
            fighter.Attack(hit, weaponRange, mover);
        }

        private void InteractWith()
        {
            for (int i = 0; i < Interactable.Length; i++)
            {
                if (Physics.Raycast(MouseRay, out hit) && HitCompareTag(Interactable[i]) &&
                    Input.GetMouseButtonDown(0))
                {
                    //Debug.Log("We hit a terrain");
                    RayCastCheckLayer(i);
                }
            }
        }

        private bool HitCompareTag(string tag)
        {
            return hit.collider.CompareTag(tag);
        }
    }
}