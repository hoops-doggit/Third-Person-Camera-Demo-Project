using UnityEngine;

namespace DefaultNamespace {
    public class ComponentHub : MonoBehaviour{
        [SerializeField] private OrbitCamera orbitCamera;
        public OrbitCamera OrbitCamera => orbitCamera;
    }
}