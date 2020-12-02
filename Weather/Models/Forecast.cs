namespace QuickType
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Forecast
    {
        [JsonProperty("@context")]
        public ContextElement[] Context { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }

    public partial class ContextClass
    {
        [JsonProperty("@version")]
        public string Version { get; set; }

        [JsonProperty("wx")]
        public Uri Wx { get; set; }

        [JsonProperty("geo")]
        public Uri Geo { get; set; }

        [JsonProperty("unit")]
        public Uri Unit { get; set; }

        [JsonProperty("@vocab")]
        public Uri Vocab { get; set; }
    }

    public partial class Geometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public double[][][] Coordinates { get; set; }
    }

    public partial class Properties
    {
        [JsonProperty("updated")]
        public DateTimeOffset Updated { get; set; }

        [JsonProperty("units")]
        public string Units { get; set; }

        [JsonProperty("forecastGenerator")]
        public string ForecastGenerator { get; set; }

        [JsonProperty("generatedAt")]
        public DateTimeOffset GeneratedAt { get; set; }

        [JsonProperty("updateTime")]
        public DateTimeOffset UpdateTime { get; set; }

        [JsonProperty("validTimes")]
        public string ValidTimes { get; set; }

        [JsonProperty("elevation")]
        public Elevation Elevation { get; set; }

        [JsonProperty("periods")]
        public Period[] Periods { get; set; }
    }

    public partial class Elevation
    {
        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("unitCode")]
        public string UnitCode { get; set; }
    }

    public partial class Period
    {
        [JsonProperty("number")]
        public long Number { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTimeOffset EndTime { get; set; }

        [JsonProperty("isDaytime")]
        public bool IsDaytime { get; set; }

        [JsonProperty("temperature")]
        public long Temperature { get; set; }

        [JsonProperty("temperatureUnit")]
        public TemperatureUnit TemperatureUnit { get; set; }

        [JsonProperty("temperatureTrend")]
        public string TemperatureTrend { get; set; }

        [JsonProperty("windSpeed")]
        public string WindSpeed { get; set; }

        [JsonProperty("windDirection")]
        public string WindDirection { get; set; }

        [JsonProperty("icon")]
        public Uri Icon { get; set; }

        [JsonProperty("shortForecast")]
        public string ShortForecast { get; set; }

        [JsonProperty("detailedForecast")]
        public string DetailedForecast { get; set; }
    }

    public enum TemperatureUnit { F };

    public partial struct ContextElement
    {
        public ContextClass ContextClass;
        public Uri PurpleUri;

        public static implicit operator ContextElement(ContextClass ContextClass) => new ContextElement { ContextClass = ContextClass };
        public static implicit operator ContextElement(Uri PurpleUri) => new ContextElement { PurpleUri = PurpleUri };
    }

    public partial class Forecast
    {
        public static Forecast FromJson(string json) => JsonConvert.DeserializeObject<Forecast>(json, QuickType.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Forecast self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ContextElementConverter.Singleton,
                TemperatureUnitConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ContextElementConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ContextElement) || t == typeof(ContextElement?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    try
                    {
                        var uri = new Uri(stringValue);
                        return new ContextElement { PurpleUri = uri };
                    }
                    catch (UriFormatException) { }
                    break;
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<ContextClass>(reader);
                    return new ContextElement { ContextClass = objectValue };
            }
            throw new Exception("Cannot unmarshal type ContextElement");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ContextElement)untypedValue;
            if (value.PurpleUri != null)
            {
                serializer.Serialize(writer, value.PurpleUri.ToString());
                return;
            }
            if (value.ContextClass != null)
            {
                serializer.Serialize(writer, value.ContextClass);
                return;
            }
            throw new Exception("Cannot marshal type ContextElement");
        }

        public static readonly ContextElementConverter Singleton = new ContextElementConverter();
    }

    internal class TemperatureUnitConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TemperatureUnit) || t == typeof(TemperatureUnit?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "F")
            {
                return TemperatureUnit.F;
            }
            throw new Exception("Cannot unmarshal type TemperatureUnit");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TemperatureUnit)untypedValue;
            if (value == TemperatureUnit.F)
            {
                serializer.Serialize(writer, "F");
                return;
            }
            throw new Exception("Cannot marshal type TemperatureUnit");
        }

        public static readonly TemperatureUnitConverter Singleton = new TemperatureUnitConverter();
    }
}
