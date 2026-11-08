using AllowanceApp.Avalonia.Messages;
using AllowanceApp.Avalonia.Utilities.AudioHelpers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.Messaging;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Views.Accounts
{
    public partial class AllowanceView : UserControl
    {
        private static readonly Random _rng = new();

        private readonly List<Uri> _hats = new()
        {
            new Uri("avares://AllowanceApp.Avalonia/Assets/Images/Hats/beanie.png"),
            new Uri("avares://AllowanceApp.Avalonia/Assets/Images/Hats/beret.png"),
            new Uri("avares://AllowanceApp.Avalonia/Assets/Images/Hats/bowler.png"),
            new Uri("avares://AllowanceApp.Avalonia/Assets/Images/Hats/bucket-hat.png"),
            new Uri("avares://AllowanceApp.Avalonia/Assets/Images/Hats/hat.png"),
            new Uri("avares://AllowanceApp.Avalonia/Assets/Images/Hats/party-hat-1.png"),
            new Uri("avares://AllowanceApp.Avalonia/Assets/Images/Hats/party-hat-2.png"),
            new Uri("avares://AllowanceApp.Avalonia/Assets/Images/Hats/pirate.png"),
            new Uri("avares://AllowanceApp.Avalonia/Assets/Images/Hats/straw-hat.png"),
            new Uri("avares://AllowanceApp.Avalonia/Assets/Images/Hats/wizard-hat.png")
        };

        private static readonly IBrush[] _confettiColors =
        [
            Brushes.Red,
            Brushes.Green,
            Brushes.Yellow,
            Brushes.LimeGreen,
            Brushes.DeepSkyBlue,
            Brushes.MediumPurple,
            Brushes.HotPink
        ];

        public AllowanceView()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<DataRefreshSucessfulMessage>(this, async (r, m) => await RandomAnimation((AllowanceView)r));
        }

        private async Task RandomAnimation(AllowanceView view)
        {
            int dice = _rng.Next(0, 2);
            switch (dice)
            {
                case 0:
                    await HatAnimation(view);
                    break;
                case 1:
                    await ConfettiAnimation(view);
                    break;
            }
        }

        private async Task ConfettiAnimation(AllowanceView view)
        {
            var origin = view.AllowanceGrid.TranslatePoint(
                new Point(view.AllowanceGrid.Bounds.Width / 2, view.AllowanceGrid.Bounds.Height / 2), view.ConfettiCanvas) ?? new Point(200, 200);
            using IAudioPlayer player = AudioPlayerFactory.GetAudioPlayerForOS();
            player.PlaySound("avares://AllowanceApp.Avalonia/Assets/Sounds/oh-my-god.wav");
            for (int j = 0; j < 25; j++)
            {
                for (int i = 0; i < 50; i++)
                {
                    SpawnConfettiPiece(origin);
                }
                await Task.Delay(100);
            }
            await player.FinishPlayback();
        }

        private void SpawnConfettiPiece(Point origin)
        {
            var size = _rng.Next(6, 14);
            var piece = new Rectangle
            {
                Width = size,
                Height = size * 0.6,
                Fill = _confettiColors[_rng.Next(_confettiColors.Length)],
                RenderTransform = new RotateTransform(),
                RenderTransformOrigin = RelativePoint.Center
            };

            Canvas.SetLeft(piece, origin.X);
            Canvas.SetTop(piece, origin.Y);
            ConfettiCanvas.Children.Add(piece);

            double angle = (_rng.NextDouble() * Math.PI) - Math.PI;
            double speed = _rng.Next(150, 350);
            double vx = Math.Cos(angle) * speed;
            double vy = Math.Sin(angle) * speed;
            double rotationalSpeed = _rng.Next(-720, 720);

            _ = AnimateThing(piece, origin, vx, vy, rotationalSpeed);
        }

        private async Task HatAnimation(AllowanceView view)
        {
            using IAudioPlayer player = AudioPlayerFactory.GetAudioPlayerForOS();
            player.PlaySound("avares://AllowanceApp.Avalonia/Assets/Sounds/yeah-woah.wav");
            for (int j = 0; j < 30; j++)
            {
                SpawnHat();
                await Task.Delay(_rng.Next(200));
            }
            await player.FinishPlayback();
        }

        private void SpawnHat()
        {
            var nextX = _rng.Next(20, 400);
            var origin = new Point(nextX, 550);
            var next = _rng.Next(_hats.Count);
            var hat = new Image
            {
                Width = 50,
                Height = 50,
                Source = new Bitmap(AssetLoader.Open(_hats[next])),
                RenderTransform = new RotateTransform(),
                RenderTransformOrigin = RelativePoint.Center
            };

            Canvas.SetLeft(hat, origin.X);
            Canvas.SetTop(hat, origin.Y);
            ConfettiCanvas.Children.Add(hat);

            double angle = 270 * Math.PI / 180;
            double speed = _rng.Next(250, 600);
            double vx = Math.Cos(angle) * speed;
            double vy = Math.Sin(angle) * speed;
            double rotationalSpeed = _rng.Next(-720, 720);

            _ = AnimateThing(hat, origin, vx, vy, rotationalSpeed);
        }

        private async Task AnimateThing(Control piece, Point origin, double vx, double vy, double rotationalSpeed)
        {
            const double gravity = 600;
            const double dt = 1.0 / 60.0;
            double x = origin.X;
            double y = origin.Y;
            double rotation = 0;
            double elapsed = 0;
            double lifetime = 2.0 + _rng.NextDouble();

            while (elapsed < lifetime)
            {
                vy += gravity * dt;
                x += vx * dt;
                y += vy * dt;
                rotation += rotationalSpeed * dt;
                elapsed += dt;

                Canvas.SetLeft(piece, x);
                Canvas.SetTop(piece, y);
                ((RotateTransform)piece.RenderTransform!).Angle = rotation;

                if (elapsed > lifetime - 0.5)
                {
                    piece.Opacity = Math.Max(0, lifetime - elapsed) / 0.5;
                }

                await Task.Delay(TimeSpan.FromSeconds(dt));
            }

            ConfettiCanvas.Children.Remove(piece);
        }
    }
}