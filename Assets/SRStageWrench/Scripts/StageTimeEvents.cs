using UnityEngine;
using UnityEngine.Events;

namespace Synth.Utils { 
    
    public class StageTimeEvents : MonoBehaviour { 

        [SerializeField]
        private float target;

        public float Target { get => target; private set => target = value; }
        public UnityEvent OnTimeTick;
        
    }
}