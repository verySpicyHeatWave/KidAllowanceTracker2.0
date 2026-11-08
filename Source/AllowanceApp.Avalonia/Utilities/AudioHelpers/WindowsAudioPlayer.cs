using Avalonia.Platform;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Utilities.AudioHelpers
{
    public class WindowsAudioPlayer : IAudioPlayer
    {
        private Stream? _assetStream;
        private MemoryStream? _memoryStream;
        private WaveFileReader? _waveFileReader;
        private WaveOut? _waveout;

        public WindowsAudioPlayer()
        {
            if (!OperatingSystem.IsWindows()) return;
            _waveout = new();
        }

        public void PlaySound(string uriString)
        {
            if (!OperatingSystem.IsWindows()) return;

            Uri uri = new(uriString);

            _assetStream = AssetLoader.Open(uri);
            _memoryStream = new();
            _assetStream.CopyTo(_memoryStream);

            _memoryStream.Seek(0, 0);

            _waveFileReader = new(_memoryStream);
            _waveout?.Init(_waveFileReader);
            _waveout?.Play();
        }

        public async Task FinishPlayback()
        {
            if (!OperatingSystem.IsWindows()) return;
            if (_waveout == null) return;

            while (_waveout.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(100);
            }
        }

        public void Dispose()
        {
            if (!OperatingSystem.IsWindows()) return;
            _waveout?.Dispose();
            _waveFileReader?.Dispose();
            _memoryStream?.Dispose();
            _assetStream?.Dispose();
        }
    }
}
