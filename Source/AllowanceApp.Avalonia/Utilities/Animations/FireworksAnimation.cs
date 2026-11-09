using AllowanceApp.Avalonia.Views.Accounts;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Utilities.Animations
{
    public class FireworksAnimation : IAnimationRenderer
    {
        private readonly Random _rng;
        private readonly AllowanceView _view;
        private readonly Canvas _canvas;

        private static readonly IBrush[] _fireworkColors =
        [
            Brushes.Red,
            Brushes.LimeGreen,
            Brushes.DeepSkyBlue,
            Brushes.MediumPurple,
            Brushes.HotPink
        ];

        public FireworksAnimation(AllowanceView view, Canvas canvas, Random? rng = null)
        {
            _view = view;
            _canvas = canvas;
            _rng = rng ?? new Random();
        }

        public async Task RenderAsync()
        {
            for (int j = 0; j < 20; j++)
            {
                SpawnFirework();
                await Task.Delay(_rng.Next(400));
            }
        }

        public void SpawnFirework()
        {
            var nextX = _rng.Next(20, 400);
            var origin = new Point(nextX, 550);

            double angle = 270 * Math.PI / 180;
            double speed = _rng.Next(400, 600);
            double vy = Math.Sin(angle) * speed;

            _ = AnimateFirework(origin, vy);
        }

        private async Task AnimateFirework(Point origin, double vy)
        {
            const double gravity = 600;
            const double dt = 1.0 / 60.0;
            Queue<Control> trails = new Queue<Control>();

            double x = origin.X;
            double y = origin.Y;

            while (vy < 0)
            {
                var trail = new Rectangle
                {
                    Width = 3,
                    Height = 3,
                    Fill = Brushes.DarkGray
                };

                vy += gravity * dt;
                x = origin.X + _rng.Next(-2, 3);
                y += vy * dt;

                trails.Enqueue(trail);
                Canvas.SetLeft(trail, x);
                Canvas.SetTop(trail, y);
                _canvas.Children.Add(trail);

                if (trails.Count >= 40)
                {
                    var oldSpark = trails.Dequeue();
                    _canvas.Children.Remove(oldSpark);
                }

                await Task.Delay(TimeSpan.FromSeconds(dt));
            }

            // EXPLODE HERE, AT THE EXACT X AND Y
            var explosion = RenderSparkExplosion(new Point(origin.X, y));

            while (trails.Count != 0)
            {
                var oldSpark = trails.Dequeue();
                _canvas.Children.Remove(oldSpark);

                await Task.Delay(TimeSpan.FromSeconds(dt));
            }

            await explosion;
        }

        private async Task RenderSparkExplosion(Point origin)
        {
            var color = _fireworkColors[_rng.Next(_fireworkColors.Length)];
            for (int i = 0; i < 250; i++)
            {
                SpawnSpark(origin, color);
            }
        }

        private void SpawnSpark(Point origin, IBrush color)
        {
            var piece = new Rectangle
            {
                Width = 2,
                Height = 2,
                Fill = color,
                Opacity = _rng.NextDouble() * 0.5 + 0.5
            };

            Canvas.SetLeft(piece, origin.X);
            Canvas.SetTop(piece, origin.Y);
            _canvas.Children.Add(piece);
            double angle = (_rng.NextDouble() * Math.PI * 2);
            double speed = _rng.Next(20, 50);
            double vx = Math.Cos(angle) * speed;
            double vy = Math.Sin(angle) * speed;

            _ = AnimateSpark(piece, origin, vx, vy);
        }

        private async Task AnimateSpark(Control piece, Point origin, double vx, double vy)
        {
            const double gravity = 50;
            const double dt = 1.0 / 60.0;
            double x = origin.X;
            double y = origin.Y;
            double elapsed = 0;
            double lifetime = 0.5 + _rng.NextDouble();

            while (elapsed < lifetime)
            {
                vy += gravity * dt;
                x += vx * dt;
                y += vy * dt;
                elapsed += dt;

                Canvas.SetLeft(piece, x);
                Canvas.SetTop(piece, y);

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
