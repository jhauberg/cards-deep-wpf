using CardsDeep.ViewModel;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CardsDeep.Controls {
    public partial class CardStack : UserControl {
        public event EventHandler Activated;

        public CardStack() {
            InitializeComponent();
        }

        void UserControl_MouseUp(object sender, MouseButtonEventArgs e) {
            OnActivated();

            Storyboard slideStoryboard = Resources["SlideToSnapStoryboard"] as Storyboard;

            slideStoryboard.Begin();
        }

        void OnActivated() {
            if (Activated != null) {
                Activated(this, EventArgs.Empty);
            }
        }

        public bool CanSkip {
            get;
            set;
        }

        public Card Draw() {
            Card card = new Card();
            CardViewModel context = card.DataContext as CardViewModel;

            bool cardShouldBeBeneficial = Roll.Next(0, 100) >= 60;

            if (cardShouldBeBeneficial) {
                int r = Roll.Next(0, 100);

                if (r >= 0 && r < 10) {
                    context.Type = CardType.Treasure;
                } else if (r >= 10 && r < 40) {
                    context.Type = CardType.Part;
                } else if (r >= 40 && r < 55) {
                    context.Type = CardType.Health;
                } else {
                    context.Type = CardType.Sword;
                }
            } else {
                context.Type = CardType.Monster;
            }

            switch (context.Type) {
                default:
                    break;

                case CardType.Health: {
                    context.Value = Roll.Next(2, 11);
                } break;

                case CardType.Monster: {
                    context.Value = Roll.Next(2, 15);
                } break;

                case CardType.Sword: {
                    context.Value = Roll.Next(2, 11);
                }
                break;
            }
            
            return card;
        }

        void Grid_MouseEnter(object sender, MouseEventArgs e) {
            if (!CanSkip) {
                return;
            }

            Storyboard slideStoryboard = Resources["SlideDownwardsStoryboard"] as Storyboard;

            slideStoryboard.Begin();
        }

        void Grid_MouseLeave(object sender, MouseEventArgs e) {
            Storyboard slideStoryboard = Resources["SlideToSnapStoryboard"] as Storyboard;

            slideStoryboard.Begin();
        }
    }
}
