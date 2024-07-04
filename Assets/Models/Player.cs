using Assets.Enums;
using UnityEngine;

namespace Assets.Models
{
    public class Player
    {
        public string Id { get; set; }
        public Vector3 Position { get; set; } = Vector3.zero;
        public float Speed { get; set; }
        public float SomeThreshold { get; set; }
        public PlayerState PlayerState { get; set; } = PlayerState.IDLE;
        public Quaternion Rotation { get; set; }
        public float Angle { get; set; }
    }
}
