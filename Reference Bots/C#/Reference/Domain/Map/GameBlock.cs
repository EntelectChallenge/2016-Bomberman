using Newtonsoft.Json;
using Reference.Domain.Map.Entities;
using Reference.Domain.Map.Entities.PowerUps;

namespace Reference.Domain.Map
{
    public class GameBlock
    {
        public Location Location { get; set; }

        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        public IEntity Entity { get; set; }
        public BombEntity Bomb { get; set; }
        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        public IPowerUp PowerUp { get; set; }
        public bool Exploding { get; set; }
    }
}
