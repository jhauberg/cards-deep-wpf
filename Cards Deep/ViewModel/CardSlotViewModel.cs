using System;

namespace CardsDeep.ViewModel {
    internal class CardSlotViewModel : BaseViewModel {
        string _title;

        public string Title {
            get {
                return _title;
            }
            set {
                if (_title != value) {
                    _title = value;

                    OnPropertyChanged("Title");
                }
            }
        }
    }
}
