using System.Runtime.Serialization;

namespace SpotiConnector.Application.Enums
{
    public enum AuthorizationScopesEnum
    {
        [EnumMember(Value = "user-read-playback-state")]
        UserReadPlaybackState,
        [EnumMember(Value = "user-modify-playback-state")]
        UserModifyPlaybackState,
        [EnumMember(Value = "user-read-currently-playing")]
        UserReadCurrentlyPlaying,
        [EnumMember(Value = "user-top-read")]
        UserTopRead
    }
}
