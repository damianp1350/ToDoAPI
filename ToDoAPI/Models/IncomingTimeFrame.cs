using System.Text.Json.Serialization;

namespace ToDoAPI.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IncomingTimeFrame
{
    Today,
    NextDay,
    CurrentWeek
}
