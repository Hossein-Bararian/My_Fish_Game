using System.Collections.Generic;
using Nakama.TinyJson;
using UnityEngine;

public static class MatchDataJson
{   

    public static string VelocityPosition(Vector2 velocity, Vector3 position)
    {
        var data = new Dictionary<string, string>
        {
            { "velocity.x", velocity.x.ToString() },
            { "velocity.y", velocity.y.ToString() },
            { "position.x", position.x.ToString() },
            { "position.y", position.y.ToString() }
        };
        return data.ToJson();
    }
}
