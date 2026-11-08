using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Utilities.AudioHelpers
{
    public class LinuxAudioPlayer : IAudioPlayer
    {
        public void PlaySound(string uri)
        {

        }

        public Task FinishPlayback()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
