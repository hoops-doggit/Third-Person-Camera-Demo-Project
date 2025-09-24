
using UnityEngine;

namespace ThisNamespace {
    public class PlayerHub : MonoBehaviour {
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform artRoot;
        [SerializeField] private OrbitCamera orbitCamera;
        
        public Rigidbody Rigidbody => rb;
        public PlayerMovement Movement => movement;
        public Transform ArtRoot => artRoot;
        public OrbitCamera OrbitCamera => orbitCamera;
    }
}