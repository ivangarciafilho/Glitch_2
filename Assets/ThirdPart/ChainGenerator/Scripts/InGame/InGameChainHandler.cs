using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chain;
using UnityEngine;

namespace ChainInGame
{
    public class InGameChainHandler : MonoBehaviour
    {
        public Machinery[] machineries;
        public Cogwheel[] gears;
        
        private List<MachineryInGame> _machineriesInGame = new();

        private MachineryInGame _currentMachineryInGame;

        private void OnEnable()
        {
            ChainEvents.InGameEvents.OnOptionSet += SelectMachinery;
        }

        private void Start()
        {
            if(machineries.Length == 0)
                machineries = FindObjectsOfType<Machinery>();
            
            if(gears.Length == 0)
                gears = FindObjectsOfType<Cogwheel>();
            
            ChainEvents.InGameEvents.OnMachineriesSet?.Invoke(machineries);
            
            SetInGameMachineries();
            CreateInteractables();

            _currentMachineryInGame = _machineriesInGame.First();
        }
        
        Ray RayFromCamera() => Camera.main.ScreenPointToRay(Input.mousePosition);
  
        private void Update()
        {
            ControlInputs();
        }

        void SetInGameMachineries()
        {
            foreach (var machinery in machineries)
            {
                _machineriesInGame.Add(new MachineryInGame(machinery));
            }
        }

        void CreateInteractables()
        {
            foreach (var gear in gears)
            {
                var interactable = gear.gameObject.AddComponent<InteractableGear>();
                
                interactable.Setup(gear);
                interactable.gameObject.layer = LayerMask.NameToLayer("InteractableGear");
            }
        }

        void ControlInputs()
        {
            if (Input.GetMouseButtonDown(0))
            {
            
                if (Physics.Raycast(RayFromCamera(), out RaycastHit hit, LayerMask.GetMask("InteractableGear")))
                {
                    var interactable = hit.transform.gameObject.GetComponent<InteractableGear>();
                    if(!interactable) return;
                    _currentMachineryInGame.AddToMachinery(interactable);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(RayFromCamera(), out RaycastHit hit, LayerMask.GetMask("InteractableGear")))
                {
                    var interactable = hit.transform.gameObject.GetComponent<InteractableGear>();
                    if(!interactable) return;
                    _currentMachineryInGame.RemoveFromMachinery(interactable);
                }
            }
        }

        void SelectMachinery(int i)
        {
            _currentMachineryInGame = _machineriesInGame[i];
        }

        public void StopMotion()
        {
            _currentMachineryInGame.StopMotion();
        }

        public void StartMotion()
        {
            _currentMachineryInGame.StartMotion();
        }

        private void OnDisable()
        {
            ChainEvents.InGameEvents.OnOptionSet -= SelectMachinery;
        }
    }
}

