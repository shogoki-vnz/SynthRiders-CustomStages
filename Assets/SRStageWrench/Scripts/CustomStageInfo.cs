using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synth.Utils {
    public class CustomStageInfo : MonoBehaviour
    {
        [HideInInspector] public string StageRenderPipeline = "SRP";
        [HideInInspector] public string ReleaseChannel = "Release";
        [HideInInspector] public string StageTemplateVersion = "0.41";

        //Normal Stage Items
        public bool isNormStage;
        public Texture2D normStageIcon;
        public string normStageName;
        public string normStageAuthor;
        public GameObject normTile01;
        public GameObject normTile02;
        public GameObject normTile03;
        public GameObject normplatform;
        public GameObject normSpecialFX;
        public Material normSkybox;
        // Spin Stage Items
        public bool isSpinStage;
        public Texture2D spinStageIcon;
        public string spinStageName;
        public string spinStageAuthor;
        public GameObject spinPlatform;
        public GameObject spinEmitter;
        public GameObject spinSpecialFX;
        public Material spinSkybox;
        //Experience Items
        public bool isExperience;
        public string expTag;
        public Texture2D expCover;
        public string expName;
        public string expAuthor;
        public Texture2D expPromptBanner;
        public string expPromptText;
        public GameObject expPrefab;
        public GameObject expSpecialsFX;
        public Material expSkybox;
    }
}