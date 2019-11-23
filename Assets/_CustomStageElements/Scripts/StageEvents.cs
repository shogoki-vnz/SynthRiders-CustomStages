using UnityEngine;
using UnityEngine.Events;

namespace Synth.Utils { 
    
    public class StageEvents : MonoBehaviour { 
        
        [SerializeField]
        private int scoreTarget;
        [SerializeField]
        private int timeTarget;

        public int ScoreTarget { get => scoreTarget; private set => scoreTarget = value; }
        public int TimeTarget { get => timeTarget; private set => timeTarget = value; }

        public UnityEvent OnSongStart;
        public UnityEvent OnSongEnd;
        public UnityEvent OnNoteHit;
        public UnityEvent OnNoteFail;
        public UnityEvent OnTimeTick;
        public UnityEvent OnScore;
        public UnityEvent OnEnterSpecial;
        public UnityEvent OnCompleteSpecial;
        public UnityEvent OnFailSpecial;
        
    }
}