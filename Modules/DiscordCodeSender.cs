using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InnerNet;
using TOHE;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // Make sure to install Newtonsoft.Json via NuGet

class CodeSender
{
    public static Dictionary<string, string> lobbyMessageDictionary = new Dictionary<string, string>();
    public static readonly HttpClient client = new HttpClient();
    public static string webhookUrl = "https://discord.com/api/webhooks/1358974496849133569/dZaplXOAsEdVUMQNBUBP8-CELpG-rhMLL4M---P3ChG13ev6oNY9BXsUJZVW7BK3rzxL";
    public static async void SendCode()
    {
        var lobbyCode = GameCode.IntToGameName(AmongUsClient.Instance.GameId);
        var player2 = Utils.GetPlayerById(1);
        if (player2 == null)
        {
            if (lobbyMessageDictionary.ContainsKey(lobbyCode))
            {
                var messageId = lobbyMessageDictionary[lobbyCode];
                var editUrl = $"{webhookUrl}/messages/{messageId}";
                await client.DeleteAsync(editUrl);
            }
        }

        if (!Options.SendLobbyCodeToDiscord.GetBool()) return;

        var content = $"{GameCode.IntToGameName(AmongUsClient.Instance.GameId)} - {Utils.GetRegionName} (Version: {Main.PluginDisplayVersion}{Main.PluginDisplaySuffix})\n";
        if (TOHE.GameStates.IsLobby)
        {
            content += "In Lobby";
        }
        else
        {
            content += "In Game";
        }

        if (!lobbyMessageDictionary.ContainsKey(lobbyCode))
        {
            // Send a new message if this lobby code hasn't been sent yet
            var payload = new { content = content };

            var response = await client.PostAsync(webhookUrl, new StringContent(
                JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);
                string messageId = json["id"]?.ToString();

                // Store the message ID in the dictionary
                if (!string.IsNullOrEmpty(messageId))
                {
                    lobbyMessageDictionary[lobbyCode] = messageId;
                }
            }
        }
        else
        {
            // If the lobby code exists, update the corresponding message
            string messageId = lobbyMessageDictionary[lobbyCode];
            var payload = new { content = content };

            var editUrl = $"{webhookUrl}/messages/{messageId}";
            var response = await client.PatchAsync(editUrl, new StringContent(
                JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
        }
    }
}
