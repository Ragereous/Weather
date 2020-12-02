using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Weather.Models
{
    public class Geocoding
    {
        public class Benchmark
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("benchmarkName")]
            public string BenchmarkName { get; set; }

            [JsonPropertyName("benchmarkDescription")]
            public string BenchmarkDescription { get; set; }

            [JsonPropertyName("isDefault")]
            public bool IsDefault { get; set; }
        }

        public class Address
        {
            [JsonPropertyName("street")]
            public string Street { get; set; }

            [JsonPropertyName("state")]
            public string State { get; set; }

            [JsonPropertyName("zip")]
            public string Zip { get; set; }
        }

        public class Input
        {
            [JsonPropertyName("benchmark")]
            public Benchmark Benchmark { get; set; }

            [JsonPropertyName("address")]
            public Address Address { get; set; }
        }

        public class Result
        {
            [JsonPropertyName("input")]
            public Input Input { get; set; }

            [JsonPropertyName("addressMatches")]
            public List<object> AddressMatches { get; set; }
        }

        public class Root
        {
            [JsonPropertyName("result")]
            public Result Result { get; set; }
        }
    }
}
