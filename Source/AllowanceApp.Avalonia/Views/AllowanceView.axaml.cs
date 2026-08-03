using AllowanceApp.Avalonia.Messages;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Views
{
    public partial class AllowanceView : UserControl
    {
        private static readonly Random _rng = new();
        private static readonly IBrush[] _confettiColors = new IBrush[]
        {
            Brushes.Red,
            Brushes.Green,
            Brushes.Yellow,
            Brushes.LimeGreen,
            Brushes.DeepSkyBlue,
            Brushes.MediumPurple,
            Brushes.HotPink
        };

        public AllowanceView()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<DataRefreshSucessfulMessage>(this, async (r, m) =>
            {
                var view = (AllowanceView)r;

                // TODO: Get the button that was clicked here, feed it to the message as an arg
                var origin = view.AllowanceGrid.TranslatePoint(
                    new Point(view.AllowanceGrid.Bounds.Width / 2, view.AllowanceGrid.Bounds.Height / 2), view.ConfettiCanvas) ?? new Point(200, 200);
                for (int j = 0; j < 10; j++)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        SpawnConfettiPiece(origin);
                    }
                    await Task.Delay(50);
                }
            });
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

            double angle = (_rng.NextDouble() * Math.PI) - (Math.PI / 2);
            double speed = _rng.Next(150, 350);
            double vx = Math.Cos(angle) * speed;
            double vy = Math.Sin(angle) * speed;
            double rotationalSpeed = _rng.Next(-720, 720);

            _ = AnimateParticle(piece, origin, vx, vy, rotationalSpeed);
        }

        private async Task AnimateParticle(Rectangle piece, Point origin, double vx, double vy, double rotationalSpeed)
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