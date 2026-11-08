using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Utilities.AudioHelpers
{
    internal class UnhandledAudioPlayer : IAudioPlayer
    {
        public void PlaySound(string uri) { }
        public Task FinishPlayback() => Task.CompletedTask;
        public void Dispose() { }
    }
}