using Avalonia.Controls;
using Avalonia.Labs.Gif;
using System;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Utilities.Animations
{
    public class LickingDogAnimation : IAnimationRenderer
    {
        private GifImage _gif;
        public LickingDogAnimation(GifImage gif)
        {
            _gif = gif;
            _gif.IsVisible = false;
            _gif.Opacity = 0;
            _gif.Source = GifStreamSource.FromUri(new Uri("avares://AllowanceApp.Avalonia/Assets/Images/Gifs/licking-dog.gif"));
        }

        public async Task RenderAsync()
        {
            _gif.IsVisible = true;

            while (_gif.Opacity < 1)
            {
                _gif.Opacity = Math.Min(1, _gif.Opacity + 0.1);
                await Task.Delay(50);
            }
            _gif.Opacity = 1;
            await Task.Delay(5500);

            while (_gif.Opacity > 0)
            {
                _gif.Opacity = Math.Max(0, _gif.Opacity - 0.1);
                await Task.Delay(50);
            }

            _gif.Opacity = 0;
            _gif.IsVisible = false;
        }
    }
}
