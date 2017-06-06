using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hihumin2
{
	public partial class MainPage : ContentPage
	{
        GameStateViewModel gameState;

		public MainPage()
		{
			InitializeComponent();
            gameState = new GameStateViewModel(this);
            this.Content.BindingContext = gameState;
            
        }

        private void Go_Button_Clicked(object sender, EventArgs e)
        {
            switch (gameState.Carry())
            {
                case GameStateViewModel.CARRY_NG:
                    DisplayAlert("", "ひふみんたりないよ", "了解");
                    break;
                case GameStateViewModel.CARRY_PERFECT:
                    break;
                case GameStateViewModel.GAMEOVER:
                    DisplayAlert("","残念ながらゲームオーバーです","了解");
                    //Navigation.PushModalAsync(new ResultPage(new int[] { 100,50}));
                    Application.Current.MainPage = new ResultPage(new int[] { gameState.Score });
                    break;
            }

        }

        public void GameOverCallback()
        {
            Device.BeginInvokeOnMainThread(() => {
                //Navigation.PushModalAsync(new ResultPage(new int[] { gameState.Score }));
                Application.Current.MainPage = new ResultPage(new int[] { gameState.Score });
            });
        }

        private void Throwed_Hihumin_Clicked(object sender, EventArgs e)
        {
            gameState.CatchThrowedHihumin(int.Parse(((Button)sender).CommandParameter.ToString()));
        }

        private void My_Hihumin_Clicked(object sender, EventArgs e)
        {
            gameState.ThrowMyHihumin(int.Parse(((Button)sender).CommandParameter.ToString()));
        }

    }

}
