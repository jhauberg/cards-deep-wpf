using CardsDeep.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CardsDeep.Controls {
    public partial class CardSlot : UserControl {
        public event EventHandler Activated;

        public CardSlot() {
            InitializeComponent();

            AllowStacking = true;
        }

        public void Remove(Card card) {
            stack.Children.Remove(card);
        }

        public void Clear() {
            stack.Children.Clear();
        }

        public void Add(Card card) {
            if (!CanHoldAdditionalCards) {
                return;
            }

            if (stack.Children.Contains(card)) {
                return;
            }

            double offset = 0;

            bool shouldOffsetCard = false;

            if (IsLimitingShownCards) {
                if (HoldsMoreCardsThanShown || StackCount == MaxShownCards) {
                    card.Visibility = Visibility.Collapsed;
                } else {
                    shouldOffsetCard = true;
                }
            } else {
                shouldOffsetCard = true;
            }

            if (shouldOffsetCard) {
                offset = StackVerticalOffset;
            }
         
            ((card.RenderTransform as TransformGroup).Children[0] as TranslateTransform).Y = offset;
            ((card.RenderTransform as TransformGroup).Children[1] as RotateTransform).Angle = 0;
            
            if (card.Parent is Panel) {
                (card.Parent as Panel).Children.Remove(card);
            }
            
            card.IsHitTestVisible = false;

            if (FlipsCardsWhenAdded) {
                card.Flip();
            }

            stack.Children.Add(card);
        }

        public IEnumerable<Card> Cards {
            get {
                IList<Card> cards = new List<Card>();

                if (HoldsCards) {
                    foreach (Card card in stack.Children) {
                        cards.Add(card);
                    }
                }

                return cards;
            }
        }

        public Card TopCard {
            get {
                return StackCount > 0 ? stack.Children[StackCount - 1] as Card : null;
            }
        }

        public bool IsLimitingShownCards {
            get {
                return MaxShownCards > 0;
            }
        }
        
        public bool HoldsMoreCardsThanShown {
            get {
                return IsLimitingShownCards && StackCount >= MaxShownCards;
            }
        }

        public bool FlipsCardsWhenAdded {
            get;
            set;
        }

        public bool HoldsCards {
            get {
                return StackCount > 0;
            }
        }

        public bool IsEmpty {
            get {
                return StackCount == 0;
            }
        }

        public bool CanHoldAdditionalCards {
            get {
                return 
                    StackCount == 0 || 
                    (StackCount > 0 && AllowStacking);
            }
        }

        public bool AllowStacking {
            get;
            set;
        }

        public int StackCount {
            get {
                return stack.Children.Count;
            }
        }
        
        public double StackVerticalOffset {
            get {
                return 6.0 * (HoldsMoreCardsThanShown ? MaxShownCards : StackCount);
            }
        }

        public int MaxShownCards {
            get;
            set;
        }

        CardSlotViewModel CardSlotContext {
            get {
                return DataContext != null ? DataContext as CardSlotViewModel : null;
            }
        }

        public string Title {
            get {
                return CardSlotContext.Title;
            }
            set {
                CardSlotContext.Title = value;
            }
        }

        void Grid_MouseUp(object sender, MouseButtonEventArgs e) {
            if (HoldsCards) {
                OnActivated();
            }
        }

        void OnActivated() {
            System.Diagnostics.Debug.WriteLine("Activated Slot: " + this);
            if (Activated != null) {
                Activated(this, EventArgs.Empty);
            }
        }
    }
}
