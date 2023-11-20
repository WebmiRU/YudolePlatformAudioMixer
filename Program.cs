using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace YudolePlatformAudioMixer
{
    public class Message
    {
        [JsonPropertyName("type")]public string? Type { get; set; }
    }

    public class GetApplicationsVolumeListResponse
    {
        [JsonPropertyName("type")] public string? Type { get; set; } = "application_volume_list";
        [JsonPropertyName("payload")] public List<Audio.Item>? Payload { get; set; } = new();
    }

    public class SetApplicationVolumeRequest: Message
    {
        [JsonPropertyName("pid")] public int? Pid { get; set; }
        [JsonPropertyName("volume")] public int? Volume { get; set; }
    }
    
    public class WSS: WebSocketBehavior
    {
        // public WSS()
        // {
        //     Console.WriteLine("Construct");
        // }
        
        protected override void OnOpen()
        {
            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
        }

        protected override void OnMessage (MessageEventArgs e)
        {
            try
            {
                var message = JsonSerializer.Deserialize<Message>(e.Data);
                switch (message.Type)
                {
                    case "get/application_volume_list":
                        var AVLPayload = Audio.GetApplicationsVolumeList();
                        Send(JsonSerializer.Serialize(new GetApplicationsVolumeListResponse {Payload = AVLPayload}));
                        break;
                    case "set/application_volume":
                        var SAVRData = JsonSerializer.Deserialize<SetApplicationVolumeRequest>(e.Data);
                        if (SAVRData.Pid >= 0 && SAVRData.Volume >= 0 && SAVRData.Volume <= 100)
                        {
                            Audio.SetApplicationVolume((int)SAVRData.Pid, (int)SAVRData.Volume);
                        }
                        else
                        {
                            // ERROR
                        }
                        break;
                    
                    default:
                        
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            
            
            // Send (msg);
        }
    }

    public class Program
    {
        public static void Main (string[] args)
        {
            var wss = new WebSocketServer ("ws://0.0.0.0:8123");

            wss.AddWebSocketService<WSS> ("/mixer");
            wss.Start ();
            Console.ReadKey (true);
            wss.Stop ();
        }
    }
}