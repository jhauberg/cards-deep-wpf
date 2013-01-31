using CardsDeep.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CardsDeep.Controls {
    public partial class Card : UserControl {
        public event EventHandler<CardActivationEventArgs> Activated;
        public event EventHandler<CardActivationFinishedEventArgs> ActivationFinished;

        public event EventHandler<CardPlacedEventArgs> Placed;
        public event EventHandler Discarded;

        CardViewModel CardContext {
            get {
                return DataContext as CardViewModel;
            }
        }

        public Card() {
            InitializeComponent();
        }

        // hack to know which slot a card moves from
        internal CardSlot ContainingSlot {
            get;
            set;
        }

        void OnUpperPartClicked(object sender, MouseButtonEventArgs e) {
            OnActivated(upwards: true);
        }

        void OnLowerPartClicked(object sender, MouseButtonEventArgs e) {
            OnActivated(upwards: false);
        }

        public void Flip() {
            Flip(animated: true);
        }

        public bool HasFrontSideUp {
            get;
            private set;
        }

        public void Flip(bool animated) {
            if (back.Visibility == Visibility.Visible) {
                FlipToFront(animated);
            } else {
                FlipToBack(animated);
            }
        }

        void FlipToFront(bool animated) {
            front.Visibility = System.Windows.Visibility.Visible;

            if (animated) {
                Storyboard flipAnimation = Resources["FlipToFrontStoryboard"] as Storyboard;

                flipAnimation.Completed += delegate {
                    back.Visibility = System.Windows.Visibility.Collapsed;
                };

                flipAnimation.Begin();
            } else {
                back.Visibility = System.Windows.Visibility.Collapsed;
            }

            HasFrontSideUp = true;
        }

        void FlipToBack(bool animated) {
            back.Visibility = System.Windows.Visibility.Visible;

            if (animated) {
                Storyboard flipAnimation = Resources["FlipToBackStoryboard"] as Storyboard;

                flipAnimation.Completed += delegate {
                    front.Visibility = System.Windows.Visibility.Collapsed;
                };

                flipAnimation.Begin();
            } else {
                front.Visibility = System.Windows.Visibility.Collapsed;
            }

            HasFrontSideUp = false;
        }
        /*
        public void Discard() {
            Discard(upwards: true);
        }
        */
        void Vanish(bool upwards) {
            Storyboard discardAnimation = 
                upwards ?
                    Resources["DiscardUpwardsStoryboard"] as Storyboard : 
                    Resources["DiscardDownwardsStoryboard"] as Storyboard;

            discardAnimation.Completed += delegate {
                discardAnimation.Stop();

                IsActivating = false;

                OnDiscarded();
            };

            IsHitTestVisible = false;

            discardAnimation.Begin();
        }
        
        public void PlaceInSlot(CardSlot slot, TimeSpan delay) {
            Storyboard placeAnimation = Resources["PlaceStoryboard"] as Storyboard;
            
            GeneralTransform transform = slot.TransformToVisual(this);

            Point point = transform.Transform(new Point(0, 0));

            (placeAnimation.Children[0] as DoubleAnimation).To = point.X;
            (placeAnimation.Children[1] as DoubleAnimation).To = point.Y + slot.StackVerticalOffset;
            
            placeAnimation.BeginTime = delay;

            double rotateToAngle = 0;
            
            if (slot.RenderTransform is RotateTransform) {
                rotateToAngle = (slot.RenderTransform as RotateTransform).Angle;

                //(placeAnimation.Children[1] as DoubleAnimation).To -= slot.StackVerticalOffset;
            }
            
            (placeAnimation.Children[2] as DoubleAnimation).To = rotateToAngle;

            placeAnimation.Completed += delegate {
                placeAnimation.Stop();

                IsActivating = false;

                /*
                Card newCard = new Card() { 
                    ContainingSlot = slot,
                    DataContext = this.DataContext 
                };

                //newCard.Activated += Activated;
                //newCard.Placed
                if (HasFrontSideUp) {
                    newCard.Flip(animated: false);
                }
                
                slot.Add(newCard);
                */
                
                slot.Add(this);

                OnPlaced(slot, ContainingSlot);

                ContainingSlot = slot;
            };

            IsHitTestVisible = false;
            
            placeAnimation.Begin();
        }

        void OnActivated(bool upwards) {
            IsActivating = true;

            CardActivationEventArgs eventArgs = 
                new CardActivationEventArgs(upwards ? 
                    CardActivationAction.Up : 
                    CardActivationAction.Down);

            if (Activated != null) {
                Activated(this, eventArgs);
            }

            if (eventArgs.IsCanceled) {
                IsActivating = false;

                return;
            }

            if (eventArgs.Slot == null) {
                Vanish(upwards);
            } else {
                if (eventArgs.Slot.CanHoldAdditionalCards) {
                    PlaceInSlot(eventArgs.Slot, TimeSpan.Zero);
                } else {
                    IsActivating = false;
                }
            }

            OnActivationFinished(IsActivating);
        }

        void OnActivationFinished(bool isAwaitingAnimation) {
            if (ActivationFinished != null) {
                ActivationFinished(this, new CardActivationFinishedEventArgs(isAwaitingAnimation));
            }
        }

        void OnDiscarded() {
            if (Discarded != null) {
                Discarded(this, EventArgs.Empty);
            }
        }

        void OnPlaced(CardSlot toSlot, CardSlot fromSlot) {
            if (Placed != null) {
                Placed(this, new CardPlacedEventArgs(toSlot, fromSlot));
            }
        }

        bool IsActivating {
            get;
            set;
        }

        void Grid_MouseLeave(object sender, MouseEventArgs e) {
            if (IsActivating) {
                return;
            }

            // Because `MouseLeave` does not occur immediately after IsHitTestVisible = false, we have to avoid running the snap animation
            // while the card is being animated into another position.
            Storyboard snapAnimation = Resources["SlideToSnapStoryboard"] as Storyboard;

            snapAnimation.Completed += delegate {
                snapAnimation.Stop();
            };

            snapAnimation.Begin();
        }
    }
}
