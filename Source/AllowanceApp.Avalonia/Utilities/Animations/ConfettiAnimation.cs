using AllowanceApp.Avalonia.Utilities.AudioHelpers;
using AllowanceApp.Avalonia.Views.Accounts;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Utilities.Animations
{
    public class ConfettiAnimation : IAnimationRenderer
    {
        private readonly Random _rng;
        private readonly AllowanceView _view;
        private readonly Canvas _canvas;

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

        public ConfettiAnimation(AllowanceView view, Canvas canvas, Random? rng = null)
        {
            _view = view;
            _canvas = view.ConfettiCanvas;
            _rng = rng ?? new Random();
        }

        public async Task RenderAsync()
        {
            var origin = new Point(225, 100);
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
            _canvas.Children.Add(piece);

            double angle = (_rng.NextDouble() * Math.PI) - Math.PI;
            double speed = _rng.Next(150, 350);
            double vx = Math.Cos(angle) * speed;
            double vy = Math.Sin(angle) * speed;
            double rotationalSpeed = _rng.Next(-720, 720);

            _ = AnimateConfettiPiece(piece, origin, vx, vy, rotationalSpeed);
        }

        private async Task AnimateConfettiPiece(Control piece, Point origin, double vx, double vy, double rotationalSpeed)
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
