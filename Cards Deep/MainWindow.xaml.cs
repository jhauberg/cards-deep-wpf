using CardsDeep.Controls;
using CardsDeep.ViewModel;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CardsDeep {
    public partial class MainWindow : Window {
        int cardsPlayedSinceLastSkip = 0;

        public MainWindow() {
            InitializeComponent();

            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            Reset();
        }

        void Window_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Space) {
                Skip();
                /*
                //DrawIntoRiver();

                // it appears there's z-fighting going on..
                //Card sword = new Card();
                Card sword = river.Children[0] as Card;
                CardViewModel swordCardContext = sword.DataContext as CardViewModel;

                swordCardContext.Type = CardType.Sword;
                swordCardContext.Value = 5;

                treasureSlot.Add(sword);
                sword.IsHitTestVisible = false;
                //sword.Flip(animated: false);
                //sword.PlaceInSlot(treasureSlot, TimeSpan.FromSeconds(1));
                */
            } else if (e.Key == Key.Enter) {
                if (GameHasFinished && CanReset) {
                    Reset();
                }
            }
        }

        bool GameHasFinished {
            get;
            set;
        }

        bool CanReset {
            get;
            set;
        }

        ScoreViewModel ScoreContext {
            get {
                return Resources["ScoreContext"] as ScoreViewModel;
            }
        }

        void Card_Activated(object sender, CardActivationEventArgs e) {
            discardSlot.SetValue(Canvas.ZIndexProperty, 0);
            playedCardsLayout.SetValue(Canvas.ZIndexProperty, 0);
            riverLayout.SetValue(Canvas.ZIndexProperty, 1);

            if (GameHasFinished) {
                e.IsCanceled = true;

                return;
            }
            
            river.IsHitTestVisible = false;

            Card card = sender as Card;
            
            if (e.Action == CardActivationAction.Down) {
                CardSlot slot = null;

                switch ((card.DataContext as CardViewModel).Type) {
                    default:
                        break;
                        
                    case CardType.Health: {
                        //slot = discardSlot;

                        HealthViewModel healthContext = health.DataContext as HealthViewModel;

                        int healthBefore = healthContext.Value;
                        int restoredHealth = (card.DataContext as CardViewModel).Value;

                        healthContext.Value += restoredHealth;

                        int healthAfter = healthContext.Value;

                        int consumedHealth = healthAfter - healthBefore;

                        ScoreContext.AmountOfHealthConsumed += consumedHealth;
                    } break;
                        
                    case CardType.Treasure: {
                        slot = treasureSlot;
                    } break;

                    case CardType.Part: {
                        slot = manaSlot;

                        if (manaSlot.HoldsCards && manaSlot.StackCount >= 10) {
                            // limit weapon parts stack to 10
                            e.IsCanceled = true;
                        }
                    } break;

                    case CardType.Monster: {
                        slot = monsterSlot;

                        if (monsterSlot.HoldsCards) {
                            CardViewModel topCardContext = monsterSlot.TopCard.DataContext as CardViewModel;
                            CardViewModel cardContext = card.DataContext as CardViewModel;

                            if (topCardContext.Value <= cardContext.Value) {
                                e.IsCanceled = true;
                            }
                        }

                        if (!e.IsCanceled) {
                            HealthViewModel healthContext = health.DataContext as HealthViewModel;

                            int damage = (card.DataContext as CardViewModel).Value;

                            if (!weaponSlot.IsEmpty) {
                                Card weapon = weaponSlot.TopCard;
                                CardViewModel weaponCardContext = weapon.DataContext as CardViewModel;

                                damage -= weaponCardContext.Value;
                            }

                            if (damage > 0) {
                                healthContext.Value -= damage;
                            }

                            if (ScoreContext.HighestMonsterStack < 1) {
                                ScoreContext.HighestMonsterStack = 1;
                            }
                        }
                    } break;

                    case CardType.Sword: {
                        slot = weaponSlot;
                    } break;
                }

                if (slot != null) {
                    e.Slot = slot;
                }
            } else if (e.Action == CardActivationAction.Up) {
                if ((card.DataContext as CardViewModel).Type != CardType.Monster) {
                    e.Slot = discardSlot;

                    card.Flip();

                    river.IsHitTestVisible = true;
                } else {
                    e.IsCanceled = true;
                }
            }

            if (e.IsCanceled) {
                river.IsHitTestVisible = true;
            }
        }

        void Card_ActivationFinished(object sender, CardActivationFinishedEventArgs e) {
            river.IsHitTestVisible = !e.IsAwaitingAnimation;

            if ((health.DataContext as HealthViewModel).Value <= 0) {
                CanReset = true;
                GameHasFinished = true;
            }
        }

        bool IsRiverFull {
            get {
                return river.Children.Count == 5;
            }
        }

        void DrawIntoRiver() {
            if (!IsRiverFull) {
                Card card = deck.Draw();

                card.Activated += Card_Activated;
                card.ActivationFinished += Card_ActivationFinished;
                card.Discarded += Card_Discarded;
                card.Placed += Card_Placed;

                card.Margin = new Thickness(0,0,4,0);

                river.Children.Add(card);

                ThreadStart t = new ThreadStart(() => {
                    Thread.Sleep(200);

                    Dispatcher.BeginInvoke((Action)delegate {
                        card.Flip();
                    });
                });

                new Thread(t).Start();
            }
        }

        void OnCardMovedFromRiver(Card card) {
            if (!deck.CanSkip) {
                const int AmountOfCardsThatMustBePlayedBeforeAllowingSkipping = 5;

                if (++cardsPlayedSinceLastSkip >= AmountOfCardsThatMustBePlayedBeforeAllowingSkipping) {
                    cardsPlayedSinceLastSkip = 0;

                    ScoreContext.AmountOfRoomsClearedOrSkipped += 1;

                    deck.CanSkip = true;
                }

                if (cardsPlayedSinceLastSkip == AmountOfCardsThatMustBePlayedBeforeAllowingSkipping - 1) {
                    skipHintLabel.Visibility = Visibility.Visible;
                    skipHintLabel.Text = "You can skip on the following turn";
                } /*else if (cardsPlayedSinceLastSkip == AmountOfCardsThatMustBePlayedBeforeAllowingSkipping - 2) {
                    skipHintLabel.Visibility = Visibility.Visible;
                    skipHintLabel.Text = "You can skip on the next-next turn";
                }*/ else {
                    skipHintLabel.Visibility = Visibility.Collapsed;
                }
            }

            river.IsHitTestVisible = true;
            river.Children.Remove(card);

            DrawIntoRiver();
        }

        void Card_Placed(object sender, CardPlacedEventArgs e) {
            Card card = sender as Card;

            if (e.FromSlot == null) {
                OnCardMovedFromRiver(card);

                if ((card.DataContext as CardViewModel).Type == CardType.Monster) {
                    if (weaponSlot.IsEmpty) {
                        ThreadStart t = new ThreadStart(() => {
                            Thread.Sleep(500);

                            Dispatcher.BeginInvoke((Action)delegate {
                                ScoreContext.AmountOfMonstersSlain += 1;
                                ScoreContext.Score += MonsterSlainScoreValue * (card.DataContext as CardViewModel).Value;

                                DiscardAllCardsInSlot(monsterSlot);
                            });
                        });

                        new Thread(t).Start();
                        //DiscardAllCardsInSlot(monsterSlot, TimeSpan.FromSeconds(1));
                    }
                }
            } else {
                //e.FromSlot.Remove(sender as Card);
            }
        }

        void CardStack_Activated(object sender, EventArgs e) {
            Skip();
        }

        void Card_Discarded(object sender, EventArgs e) {
            OnCardMovedFromRiver(sender as Card);
        }

        void DiscardAllCardsInSlot(CardSlot slot) {
            DiscardAllCardsInSlot(slot, TimeSpan.Zero);
        }

        void DiscardAllCardsInSlot(CardSlot slot, TimeSpan initialDelay) {
            TimeSpan delayInterval = TimeSpan.FromMilliseconds(100);
            TimeSpan delay = initialDelay;

            foreach (Card card in slot.Cards) {
                card.PlaceInSlot(discardSlot, delay);
                card.Flip();

                delay += delayInterval;
            }
        }
  
        void Skip() {
            if (GameHasFinished) {
                return;
            }

            if (deck.CanSkip) {
                deck.CanSkip = false;

                river.Children.Clear();

                FillRiver();

                ScoreContext.AmountOfRoomsClearedOrSkipped += 1;
            }
        }

        void FillRiver() {
            ThreadStart t2 = new ThreadStart(() => {
                Thread.Sleep(200);

                for (int i = 0; i < 5; i++) {
                    Dispatcher.BeginInvoke((Action)delegate {
                        DrawIntoRiver();
                    });

                    Thread.Sleep(100);
                }
            });

            new Thread(t2).Start();
        }

        void Reset() {
            CanReset = false;
            GameHasFinished = false;

            cardsPlayedSinceLastSkip = 0;

            deck.CanSkip = true;

            ThreadStart t = new ThreadStart(() => {
                Dispatcher.BeginInvoke((Action)delegate {
                    DiscardAllCardsInSlot(weaponSlot);
                    DiscardAllCardsInSlot(monsterSlot);
                });

                Thread.Sleep(250);

                Dispatcher.BeginInvoke((Action)delegate {
                    DiscardAllCardsInSlot(manaSlot);
                    DiscardAllCardsInSlot(treasureSlot);
                });

                Thread.Sleep(750);

                Dispatcher.BeginInvoke((Action)delegate {
                    discardSlot.Clear();

                    HealthViewModel healthContext = health.DataContext as HealthViewModel;

                    healthContext.Value = healthContext.MaxValue;

                    ScoreContext.Reset();

                    river.Children.Clear();

                    FillRiver();
                });
            });

            new Thread(t).Start();
        }

        void ConvertStackToSword(CardSlot slot) {
            Card sword = new Card();

            CardViewModel swordCardContext = sword.DataContext as CardViewModel;

            swordCardContext.Type = CardType.Sword;
            swordCardContext.Value = slot.StackCount;

            slot.Add(sword);

            sword.Flip();
            sword.PlaceInSlot(weaponSlot, TimeSpan.FromSeconds(1));
        }

        void manaSlot_Activated(object sender, EventArgs e) {
            if (weaponSlot.HoldsCards) {
                return;
            }

            CardSlot slot = sender as CardSlot;

            if (slot.StackCount < 2) {
                return;
            }
            
            discardSlot.SetValue(Canvas.ZIndexProperty, 1);
            riverLayout.SetValue(Canvas.ZIndexProperty, 0);
            playedCardsLayout.SetValue(Canvas.ZIndexProperty, 2);

            ScoreContext.AmountOfWeaponsCrafted += 1;
            ScoreContext.Score += WeaponCraftScoreValue * slot.StackCount;//(WeaponCraftScoreValue * slot.StackCount) * slot.StackCount;

            DiscardAllCardsInSlot(slot);

            ConvertStackToSword(slot);
        }

        void weaponSlot_Activated(object sender, EventArgs e) {
            CardSlot slot = sender as CardSlot;
            
            discardSlot.SetValue(Canvas.ZIndexProperty, 1);
            riverLayout.SetValue(Canvas.ZIndexProperty, 0);
            playedCardsLayout.SetValue(Canvas.ZIndexProperty, 2);

            ScoreContext.Score += (MonstersSlainScoreValue * monsterSlot.StackCount) * monsterSlot.StackCount;

            if (ScoreContext.HighestMonsterStack < monsterSlot.StackCount) {
                ScoreContext.HighestMonsterStack = monsterSlot.StackCount;
            }

            DiscardAllCardsInSlot(slot);

            if (monsterSlot.HoldsCards) {
                ScoreContext.AmountOfMonstersSlain += monsterSlot.StackCount;

                DiscardAllCardsInSlot(monsterSlot);
            }
        }

        const int TreasureScoreValue = 10;
        const int WeaponCraftScoreValue = 1;
        const int MonsterSlainScoreValue = 1; // monster pulled with no sword equipped
        const int MonstersSlainScoreValue = 2; // monster stack removed

        void treasureSlot_Activated(object sender, EventArgs e) {
            CardSlot slot = sender as CardSlot;

            discardSlot.SetValue(Canvas.ZIndexProperty, 1);
            riverLayout.SetValue(Canvas.ZIndexProperty, 0);
            playedCardsLayout.SetValue(Canvas.ZIndexProperty, 2);

            int scoreValue = (TreasureScoreValue * slot.StackCount) * slot.StackCount;

            ScoreContext.Score += scoreValue;
            ScoreContext.AmountOfTreasureHoarded += scoreValue;

            DiscardAllCardsInSlot(slot);
        }
    }
}
