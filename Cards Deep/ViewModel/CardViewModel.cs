using System.Windows.Media;

namespace CardsDeep.ViewModel {
    internal class CardViewModel : BaseViewModel {
        CardType _type = CardType.Nothing;

        public CardType Type {
            get {
                return _type;
            }
            set {
                if (_type != value) {
                    _type = value;

                    OnPropertyChanged("Type");
                    OnPropertyChanged("Suit");
                }
            }
        }

        public bool HasValue {
            get {
                return _value > 0;
            }
        }

        int _value;

        public int Value {
            get {
                return _value;
            }
            set {
                if (_value != value) {
                    _value = value;

                    OnPropertyChanged("Value");
                    OnPropertyChanged("HasValue");
                }
            }
        }

        public SolidColorBrush Suit {
            get {
                Color suit = Colors.White;

                if (Type == CardType.Monster) {
                    suit = Colors.PaleVioletRed;
                }

                return new SolidColorBrush(suit);
            }
        }
    }
}
