using System;

namespace CardsDeep.Controls {
    public class CardPlacedEventArgs : EventArgs {
        public CardPlacedEventArgs(CardSlot intoSlot, CardSlot fromSlot) {
            ToSlot = intoSlot;
            FromSlot = fromSlot;
        }

        public CardSlot FromSlot {
            get;
            private set;
        }

        public CardSlot ToSlot {
            get;
            private set;
        }
    }
}
