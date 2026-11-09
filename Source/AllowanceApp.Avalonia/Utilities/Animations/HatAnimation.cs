using AllowanceApp.Avalonia.Utilities.AudioHelpers;
using AllowanceApp.Avalonia.Views.Accounts;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Utilities.Animations
{
    public class HatAnimation : IAnimationRenderer
    {
        private readonly Random _rng;
        private readonly AllowanceView _view;
        private readonly Canvas _canvas;

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

        public HatAnimation(AllowanceView view, Canvas canvas, Random? rng = null)
        {
            _view = view;
            _canvas = canvas;
            _rng = rng ?? new Random();
        }

        public async Task RenderAsync()
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
            _canvas.Children.Add(hat);

            double angle = 270 * Math.PI / 180;
            double speed = _rng.Next(250, 600);
            double vx = Math.Cos(angle) * speed;
            double vy = Math.Sin(angle) * speed;
            double rotationalSpeed = _rng.Next(-720, 720);

            _ = AnimateHatToss(hat, origin, vx, vy, rotationalSpeed);
        }

        private async Task AnimateHatToss(Control piece, Point origin, double vx, double vy, double rotationalSpeed)
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

            _canvas.Children.Remove(piece);
        }

    }
}
