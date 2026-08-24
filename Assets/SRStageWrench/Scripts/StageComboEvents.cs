using UnityEngine;
using UnityEngine.Events;

namespace Synth.Utils { 
    
    public class StageComboEvents : MonoBehaviour { 
        
        [SerializeField]
        private int target;

        public int Target { get => target; private set => target = value; }
        public UnityEvent OnNthCombo;
        
    }
}