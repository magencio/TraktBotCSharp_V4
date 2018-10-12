using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class People
    {
        public class CastMember
        {
            [JsonProperty("character")]
            public string Character { get; set; }

            [JsonProperty("person")]
            public Person Person { get; set; }
        }

        public class CrewMember
        {
            [JsonProperty("job")]
            public string Job { get; set; }
            
            [JsonProperty("person")]
            public Person Person { get; set; }
        }

        public class CrewMembers
        {
            [JsonProperty("production")]
            public List<CrewMember> Production { get; set; }

            [JsonProperty("art")]
            public List<CrewMember> Art { get; set; }

            [JsonProperty("crew")]
            public List<CrewMember> Crew { get; set; }

            [JsonProperty("costume & make-up")]
            public List<CrewMember> CostumeAndMakeUp { get; set; }

            [JsonProperty("directing")]
            public List<CrewMember> Directing { get; set; }

            [JsonProperty("writing")]
            public List<CrewMember> Writing { get; set; }

            [JsonProperty("sound")]
            public List<CrewMember> Sound { get; set; }

            [JsonProperty("camera")]
            public List<CrewMember> Camera { get; set; }
        }

        [JsonProperty("cast")]
        public List<CastMember> Cast { get; set; }

        [JsonProperty("crew")]
        public CrewMembers Crew { get; set; }
    }
}        