using System;

namespace CardsDeep.ViewModel {
    internal class ScoreViewModel : BaseViewModel {
        public void Reset() {
            Score = 0;

            AmountOfWeaponsCrafted = 0;
            AmountOfMonstersSlain = 0;
            AmountOfHealthConsumed = 0;
            AmountOfTreasureHoarded = 0;
            AmountOfRoomsClearedOrSkipped = 0;
            HighestMonsterStack = 0;
        }

        int _highestMonsterStack;

        public int HighestMonsterStack {
            get {
                return _highestMonsterStack;
            }
            set {
                if (_highestMonsterStack != value) {
                    _highestMonsterStack = value;

                    OnPropertyChanged("HighestMonsterStack");
                    OnPropertyChanged("HighestMonsterStackDisplay");
                }
            }
        }

        public string HighestMonsterStackDisplay {
            get {
                return String.Format("{0}", HighestMonsterStack);
            }
        }

        int _roomsCleared;

        public int AmountOfRoomsClearedOrSkipped {
            get {
                return _roomsCleared;
            }
            set {
                if (_roomsCleared != value) {
                    _roomsCleared = value;

                    OnPropertyChanged("AmountOfRoomsClearedOrSkipped");
                    OnPropertyChanged("AmountOfRoomsClearedOrSkippedDisplay");
                }
            }
        }

        public string AmountOfRoomsClearedOrSkippedDisplay {
            get {
                return String.Format("{0}", AmountOfRoomsClearedOrSkipped);
            }
        }

        int _treasuresHoarded;

        public int AmountOfTreasureHoarded {
            get {
                return _treasuresHoarded;
            }
            set {
                if (_treasuresHoarded != value) {
                    _treasuresHoarded = value;

                    OnPropertyChanged("AmountOfTreasureHoarded");
                    OnPropertyChanged("AmountOfTreasureHoardedDisplay");
                }
            }
        }

        public string AmountOfTreasureHoardedDisplay {
            get {
                return String.Format("{0}", AmountOfTreasureHoarded);
            }
        }

        int _healthConsumed;

        public int AmountOfHealthConsumed {
            get {
                return _healthConsumed;
            }
            set {
                if (_healthConsumed != value) {
                    _healthConsumed = value;

                    OnPropertyChanged("AmountOfHealthConsumed");
                    OnPropertyChanged("AmountOfHealthConsumedDisplay");
                }
            }
        }

        public string AmountOfHealthConsumedDisplay {
            get {
                return String.Format("{0}", AmountOfHealthConsumed);
            }
        }

        int _monstersSlain;

        public int AmountOfMonstersSlain {
            get {
                return _monstersSlain;
            }
            set {
                if (_monstersSlain != value) {
                    _monstersSlain = value;

                    OnPropertyChanged("AmountOfMonstersSlain");
                    OnPropertyChanged("AmountOfMonstersSlainDisplay");
                }
            }
        }

        public string AmountOfMonstersSlainDisplay {
            get {
                return String.Format("{0}", AmountOfMonstersSlain);
            }
        }

        int _weaponsCrafted;

        public int AmountOfWeaponsCrafted {
            get {
                return _weaponsCrafted;
            }
            set {
                if (_weaponsCrafted != value) {
                    _weaponsCrafted = value;

                    OnPropertyChanged("AmountOfWeaponsCrafted");
                    OnPropertyChanged("AmountOfWeaponsCraftedDisplay");
                }
            }
        }

        public string AmountOfWeaponsCraftedDisplay {
            get {
                return String.Format("{0}", AmountOfWeaponsCrafted);
            }
        }

        int _score;

        public int Score {
            get {
                return _score;
            }
            set {
                if (_score != value) {
                    _score = value;

                    OnPropertyChanged("Score");
                    OnPropertyChanged("ScoreDisplay");
                }
            }
        }

        public string ScoreDisplay {
            get {
                return String.Format("{0}", Score);
            }
        }
    }
}
