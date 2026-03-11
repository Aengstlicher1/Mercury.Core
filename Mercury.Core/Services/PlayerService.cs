using Mercury.Core.Utils;
using Mercury.Core.Utils.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Services
{
    public class PlayerService
    {
        public async Task<string?> GetStreamAsync(string videoId, CancellationToken cToken = default)
        {
            Dictionary<string, object?> payload = new()
            {
                { "videoId", videoId }
            };

            var response = await RequestHandler.PostAsync(Endpoints.Player, payload, ClientType.IOSMusic, cToken);

            using IDisposable _ = response.ParseJson(out var json);

            return null;
        } 
    }
}
