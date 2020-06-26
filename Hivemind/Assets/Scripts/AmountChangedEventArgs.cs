using System;

namespace Assets.Scripts
{
    public class AmountChangedEventArgs : EventArgs
    {
        public int Amount;

        public AmountChangedEventArgs(int amount)
        {
            Amount = amount;
        }
    }
}
