using AllowanceApp.Avalonia.Messages;
using AllowanceApp.Avalonia.Utilities.Animations;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Views.Accounts
{
    public partial class AllowanceView : UserControl
    {
        private static readonly Random _rng = new();

        private readonly List<IAnimationRenderer> _animations;


        public AllowanceView()
        {
            InitializeComponent();

            _animations =
            [
                new ConfettiAnimation(this, ConfettiCanvas, _rng),
                new HatAnimation(this, ConfettiCanvas, _rng),
                new FireworksAnimation(this, ConfettiCanvas, _rng),
                new LickingDogAnimation(LickingDogGif)
            ];

            WeakReferenceMessenger.Default.Register<DataRefreshSucessfulMessage>(this, async (r, m) => await RandomAnimation((AllowanceView)r));
        }

        private async Task RandomAnimation(AllowanceView view)
        {
            int dice = _rng.Next(0, _animations.Count);
            dice = _rng.Next(3, 4);
            var animation = _animations[dice];
            await animation.RenderAsync();
        }
    }
}