using Chain;
using UnityEngine;

namespace ChainInGame
{
    public class MachineryInGame
    {
        private Machinery _machinery;
        private CogHolder _cogHolder;

        public MachineryInGame(Machinery machinery)
        {
            _machinery = machinery;
            _cogHolder = machinery.cogHolder;

            if (_machinery.chainGenerator.ChainData == null)
            {
                Debug.Log(machinery.name + " is not Chain Related");
            }
        }

        public void SetSpeedAtRuntime(float speed)
        {
            _machinery.ChangeSpeedInRuntime(speed);
        }

        public void StopMotion()
        {
            _machinery.StopMovers();
        }

        public void StartMotion()
        {
            _machinery.Move();
        }

        public void ResetChain()
        {
            if (_machinery.chainGenerator.ChainData != null)
                _machinery.chainGenerator.ResetLinks();
        }

        public void AddToMachinery(InteractableGear interactableGear)
        {
            var gear = interactableGear._gear;
            if (_cogHolder.cogs.Contains(gear)) return;

            gear.transform.SetParent(_cogHolder.transform);
            _cogHolder.cogs.Add(gear);

            Regenerate();
        }

        public void RemoveFromMachinery(InteractableGear interactableGear)
        {
            var gear = interactableGear._gear;
            if (!_cogHolder.cogs.Contains(gear)) return;
            gear.transform.SetParent(null);
            _cogHolder.cogs.Remove(gear);

            Regenerate();
        }

        void GenerateChain()
        {
            _machinery.chainGenerator.GenerateChain(null, _machinery.cogHolder.GetChainRelatedCogs());
        }

        public void GenerateAndMove()
        {
            GenerateChain();
            _machinery.SetMovers();
            _machinery.Move();
        }

        public void Regenerate()
        {
            StopMotion();
            ResetChain();
            GenerateAndMove();
        }
    }
}