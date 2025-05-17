using System;
using System.Collections;
using System.Collections.Generic;
using Chain;
using UnityEngine;

namespace ChainInGame
{
    public class InteractableGear : MonoBehaviour
    {
        public Cogwheel _gear;
        private BoxCollider _collider;
        public void Setup(Cogwheel gear)
        {
            _gear = gear;
            AddCollider();
            SetSize();
        }
    
        void AddCollider()
        {
            _collider = gameObject.AddComponent<BoxCollider>();
        }

        void SetSize()
        {
            float size = _gear.Data.Radius * 1.5f;
            _collider.size = new Vector3(size, 1, size);
        }
    }

}
