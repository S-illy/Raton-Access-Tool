using NAudio.Wave;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server.Classes
{
    public class SFX
    {
        private static UserConfig userConfig;

        static SFX()
        {
            LoadConfig();
        }

        private static void LoadConfig()
        {
            string configFilePath = "Data/userConfig.json";
            if (File.Exists(configFilePath))
            {
                string json = File.ReadAllText(configFilePath);
                userConfig = JsonConvert.DeserializeObject<UserConfig>(json);
            }
            else
            {
                userConfig = new UserConfig { audioMute = false };
            }
        }

        private static void play(byte[] soundData)
        {
            Task.Run(() =>
            {
                using (var ms = new MemoryStream(soundData))
                using (var reader = new Mp3FileReader(ms))
                using (var output = new WaveOutEvent())
                {
                    output.Init(reader);
                    output.Play();
                    while (output.PlaybackState == PlaybackState.Playing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            });
        }

        public static void click()
        {
            if (!userConfig.audioMute)
            {
                play(RatonRAT.Properties.Resources.click);
            }
        }

        public static void notification()
        {
            if (!userConfig.audioMute)
            {
                play(RatonRAT.Properties.Resources.notification);
            }
        }

        public static void enable()
        {
            play(RatonRAT.Properties.Resources.enable);
        }

        public static void disable()
        {
            play(RatonRAT.Properties.Resources.disable);
        }

        private class UserConfig
        {
            public bool darkMode { get; set; }
            public bool audioMute { get; set; }
            public bool notificationMute { get; set; }
        }
    }
}
