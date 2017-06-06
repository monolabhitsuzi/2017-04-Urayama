using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Hihumin2
{

    class GameStateViewModel : BindingBase
    {
        Random random = new Random();

        public const byte GAMEOVER   = 0;
        public const byte CARRY_OK   = 1;
        public const byte CARRY_PERFECT = 2;
        public const byte CARRY_NG   = 3;
        
        private int hihuminLimit = 9;
        private int freightLimit = 15;

        private int[] myHihumins = new int[10];

        public int[] MyHihumins { get => myHihumins; set => myHihumins = value; }

        private int[] throwedHihumins = new int[10];

        public int[] ThrowedHihumins { get => throwedHihumins; set => myHihumins = value; }
        
        private int nowFreight;
        public int NowFreight { get => nowFreight; set { nowFreight = value; PropertyChange("NowFreight"); } }
        
        private int nextFreight;
        public int NextFreight { get => nextFreight; set { nextFreight = value; PropertyChange("NextFreight"); } }

        private int chainCount;
        public int ChainCount { get => chainCount; set { chainCount = value; PropertyChange("ChainCount"); } }

        private int virtue = 50;
        public int Virtue { get => virtue; set { virtue = value; PropertyChange("Virtue"); } }

        private int score = 0;
        public int Score { get => score; set { score = value; PropertyChange("Score"); } }

        private System.Threading.Timer timer;

        private int timeLimit = 120;
        public int TimeLimit { get => timeLimit; set { timeLimit = value; PropertyChange("TimeLimit"); } }

        private MainPage mainpage;

        public GameStateViewModel(MainPage mainpage)
        {
            this.mainpage = mainpage;

            timer = new System.Threading.Timer((o) => {if (--TimeLimit == 0) this.mainpage.GameOverCallback(); }, null,0,1000);

            NowFreight = random.Next(freightLimit)+1;
            NextFreight = random.Next(freightLimit)+1;

            for (int i = 0; i < MyHihumins.Length; i++)
            {
                MyHihumins[i] = random.Next(hihuminLimit)+1;
            }

            
        }
        
        public byte Carry()
        {
            bool perfectFlag = false;

            int sum = ThrowedHihumins.Select(n => n).Sum();

            if (NowFreight <= sum)
            {
                for (int i = 0;i < ThrowedHihumins.Length;i++)
                {
                    ThrowedHihumins[i] = 0;
                }
                PropertyChange("ThrowedHihumins");

                Score += 50;

                int penalty = sum - NowFreight;
                Virtue -= penalty;

                if (NowFreight == sum)
                {
                    perfectFlag = true;
                    Score += 50;
                    Virtue += 1;
                }

                // ひふみんの補充
                for (int i = 0; i < Virtue;i+=5)
                {
                    PushToMyHihumins(random.Next(hihuminLimit)+1);
                }

                // 次の値の用意
                NowFreight = NextFreight;
                NextFreight = random.Next(freightLimit)+1;

                if (NowFreight > MyHihumins.Select(n => n).Sum())
                {
                    return GAMEOVER;
                }
                else
                {
                    return perfectFlag ? CARRY_PERFECT : CARRY_OK;
                }
                
            }
            else
            {
                return CARRY_NG;
            }
            
        }

        public void SortMyHihumins()
        {
            int[] sorted = new int[10];
            int c = 0;

            for (int i = 0; i < MyHihumins.Length - 1;i++)
            {
                if(MyHihumins[i] != 0)
                {
                    sorted[c++] = MyHihumins[i];
                }
            }
            MyHihumins = sorted;
            OnPropertyChanged("MyHihumins");
        }

        public void ThrowMyHihumin(int index)
        {
            if (MyHihumins[index] != 0)
            {
                for (int i = 0; i < ThrowedHihumins.Length; i++)
                {
                    if (ThrowedHihumins[i] == 0)
                    {
                        ThrowedHihumins[i] = MyHihumins[index];
                        MyHihumins[index] = 0;
                        OnPropertyChanged("MyHihumins");
                        OnPropertyChanged("ThrowedHihumins");
                        break;
                    }
                }
            }
        }

        public void CatchThrowedHihumin(int index)
        {
            if (ThrowedHihumins[index] != 0)
            {
                PushToMyHihumins(ThrowedHihumins[index]);
                ThrowedHihumins[index] = 0;
                OnPropertyChanged("ThrowedHihumins");
            }
        }

        private void PushToMyHihumins(int hihumin)
        {
            for (int i = 0; i < MyHihumins.Length; i++)
            {
                if (MyHihumins[i] == 0)
                {
                    MyHihumins[i] = hihumin;
                    OnPropertyChanged("MyHihumins");
                    break;
                }
            }
        }

    }

    class BindingBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(field, value)) { return false; }

            field = value; this.OnPropertyChanged(propertyName); return true;
        }

        public void PropertyChange(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

    }


}
