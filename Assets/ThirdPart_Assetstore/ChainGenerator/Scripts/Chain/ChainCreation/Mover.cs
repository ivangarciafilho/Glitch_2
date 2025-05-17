namespace Chain
{
    public interface Mover
    {
        public float MachinerySpeed { get; set; }
        public int MachineryId { get; set; }

        public ChainEnums.ChainDirection MachineryDirection { get; set; }

        public void StartMotion();

        public void StopMotion();

        public void MachinerySetup(float machinerySpeed, int machineryId, IMachinePartData data,
            ChainEnums.ChainDirection direction)
        {
        }
        
        public float PrepareSpeedForChain();
    }
}