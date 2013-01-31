namespace CardsDeep.ViewModel {
    internal class HealthViewModel : BaseViewModel {
        int _value;
        int _valueMax;

        public int MaxValue {
            get {
                return _valueMax;
            }
            set {
                if (_valueMax != value) {
                    _valueMax = value;

                    OnPropertyChanged("MaxValue");
                }
            }
        }

        public int Value {
            get {
                return _value;
            }
            set {
                if (_value != value) {
                    _value = value;

                    if (_value > MaxValue) {
                        _value = MaxValue;
                    } else if (_value < 0) {
                        _value = 0;
                    }

                    OnPropertyChanged("Value");
                    OnPropertyChanged("Ratio");
                }
            }
        }

        public double Ratio {
            get {
                return (double)Value / MaxValue;
            }
        }
    }
}
