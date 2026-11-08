
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Utilities.AudioHelpers
{
    public interface IAudioPlayer : IDisposable
    {
        public void PlaySound(string uri);

        public Task FinishPlayback();
    }

    public static class AudioPlayerFactory
    {
        public static IAudioPlayer GetAudioPlayerForOS()
        {
            if (OperatingSystem.IsWindows())
            {
                return new WindowsAudioPlayer();
            }
            else if (OperatingSystem.IsLinux())
            {
                return new LinuxAudioPlayer();
            }
            else
            {
                return new UnhandledAudioPlayer();
            }
        }
    }
}
