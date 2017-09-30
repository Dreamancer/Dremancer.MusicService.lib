using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWebApi.ApiWrappers.Enums
{
    public enum ResponseType
    {
        Artist, Album, Song, Event, Error
    }

    public enum ApiName
    {
        Spotify, MusicBrainz, LastFM, UNDEFINED = -1
    }

    public static class EnumExtensions
    {
        public static string StringName(this ApiName name)
        {
            switch (name)
            {
                case ApiName.Spotify:
                    return "Spotify";
                case ApiName.LastFM:
                    return "LastFM";
                case ApiName.MusicBrainz:
                    return "MusicBrainz";
                default:
                    return "Udefined";
            }
        }

        public static ApiName ApiNameFromString(string s)
        {
            switch (s)
            {
                case "Spotify":
                    return ApiName.Spotify;
                case "MusicBrainz":
                    return ApiName.MusicBrainz;
                case "LastFM":
                    return ApiName.LastFM;
                default:
                    return ApiName.UNDEFINED;
            }
        }
    }
}
