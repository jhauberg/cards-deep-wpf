using System;

namespace CardsDeep.Controls {
    public class CardActivationFinishedEventArgs : EventArgs {
        public CardActivationFinishedEventArgs(bool isAwaitingAnimation) {
            IsAwaitingAnimation = isAwaitingAnimation;
        }

        public bool IsAwaitingAnimation {
            get;
            private set;
        }
    }

    public class CardActivationEventArgs : EventArgs {
        public CardActivationEventArgs(CardActivationAction action) {
            Action = action;
        }

        public bool IsCanceled {
            get;
            set;
        }

        public CardActivationAction Action {
            get;
            private set;
        }

        public CardSlot Slot {
            get;
            set;
        }
    }
}
