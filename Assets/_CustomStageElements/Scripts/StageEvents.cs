using UnityEngine;
using UnityEngine.Events;

namespace Synth.Utils { 
    
    public class StageEvents : MonoBehaviour { 

        public UnityEvent OnSongStart;
        public UnityEvent OnSongEnd;
        public UnityEvent OnNoteHit;
        public UnityEvent OnNoteFail;
        public UnityEvent OnEnterSpecial;
        public UnityEvent OnCompleteSpecial;
        public UnityEvent OnFailSpecial;
        
    }
}