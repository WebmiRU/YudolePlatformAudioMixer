using System.Text.Json.Serialization;
using CSCore.CoreAudioAPI;

namespace YudolePlatformAudioMixer;

public class Audio
{
    public class Item
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("pid")] public int Pid { get; set; }
        [JsonPropertyName("volume")] public int Volume { get; set; }
    }

    public static void SetApplicationVolume(int pid, int volume)
    {
        using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
        {
            using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
            {
                foreach (var session in sessionEnumerator)
                {
                    using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                    using (var sessionControl = session.QueryInterface<AudioSessionControl2>())
                    {
                        if (sessionControl.ProcessID == pid)
                        {
                            simpleVolume.MasterVolume = (float)(volume * 0.01);
                        }
                    }
                }
            }
        }
    }
    
    public static List<Item> GetApplicationsVolumeList()
    {
        var result = new List<Item>();
        
        using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
        {
            using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
            {
                foreach (var session in sessionEnumerator)
                {
                    using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                    using (var sessionControl = session.QueryInterface<AudioSessionControl2>())
                    {
                        result.Add(new Item
                        {
                            Name = sessionControl.Process.ProcessName,
                            Pid = sessionControl.ProcessID,
                            Volume = (int)(simpleVolume.MasterVolume * 100),
                        });

                        
                        // Console.Write(sessionControl.Process.ProcessName);
                        // Console.Write(" / ");
                        // Console.Write(simpleVolume.MasterVolume);
                        // Console.Write(" / ");
                        // Console.WriteLine(sessionControl.ProcessID);
                        
                        

                        if (sessionControl.ProcessID == 13936)
                        {
                            simpleVolume.MasterVolume = 0.90f;
                        }
                    }
                }
            }
        }

        return result;
    }
        
    private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
    {
        using (var enumerator = new MMDeviceEnumerator())
        {
            using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
            {
                //Debug.WriteLine("DefaultDevice: " + device.FriendlyName);
                var sessionManager = AudioSessionManager2.FromMMDevice(device);
                return sessionManager;
            }
        }
    }
}