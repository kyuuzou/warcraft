using UnityEngine;
using UnityEngine.Video;

public class MissionOutcome : MonoBehaviour {

    [SerializeField]
    private GUIText scoreText;
    private float visibleScore = 0.0f;

    [SerializeField]
    private GUIText rankText;

    [SerializeField]
    private GUIText unitsYouDestroyedText;
    private float visibleUnitsYouDestroyed = 0.0f;

    [SerializeField]
    private GUIText unitsEnemyDestroyedText;
    private float visibleUnitsEnemyDestroyed = 0.0f;

    [SerializeField]
    private GUIText structuresYouDestroyedText;
    private float visibleStructuresYouDestroyed = 0.0f;

    [SerializeField]
    private GUIText structuresEnemyDestroyedText;
    private float visibleStructuresEnemyDestroyed = 0.0f;

    [SerializeField]
    private GUIText goldYouMinedText;
    private float visibleGoldYouMined = 0.0f;

    [SerializeField]
    private GUIText goldEnemyMinedText;
    private float visibleGoldEnemyMined = 0.0f;

    [SerializeField]
    private GUIText unitsYouTrainedText;
    private float visibleUnitsYouTrained = 0.0f;

    [SerializeField]
    private GUIText unitsEnemyTrainedText;
    private float visibleUnitsEnemyTrained = 0.0f;

    [SerializeField]
    private GUIText structuresYouBuiltText;
    private float visibleStructuresYouBuilt = 0.0f;

    [SerializeField]
    private GUIText structuresEnemyBuiltText;
    private float visibleStructuresEnemyBuilt = 0.0f;

    [SerializeField]
    private GUIText lumberYouHarvestedText;
    private float visibleLumberYouHarvested = 0.0f;

    [SerializeField]
    private GUIText lumberEnemyHarvestedText;
    private float visibleLumberEnemyHarvested = 0.0f;

    [SerializeField]
    private VideoClip videoClip;

    [SerializeField]
    private VideoPlayer videoPlayer;

    [SerializeField]
    private AudioSource audioSource;

    private MissionStatistics statistics;

    private void Start () {
        this.statistics = ServiceLocator.Instance.MissionStatistics;

        this.videoPlayer.playOnAwake = false;
        this.videoPlayer.clip = this.videoClip;
        this.videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        this.videoPlayer.targetMaterialRenderer = this.GetComponent<Renderer>();
        this.videoPlayer.targetMaterialProperty = "_MainTex";
        this.videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        this.videoPlayer.SetTargetAudioSource(0, this.audioSource);
        this.videoPlayer.Play();

        int score = this.statistics.CalculateScore ();
        this.rankText.text = Rank.GetHumanRank (score).ToString ();
    }

    private void Update () {
        Utils.ScrollResourceNumber (
            ref this.visibleScore,
            this.statistics.Score,
            this.scoreText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleUnitsYouDestroyed,
            this.statistics.UnitsYouDestroyed,
            this.unitsYouDestroyedText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleUnitsEnemyDestroyed,
            this.statistics.UnitsEnemyDestroyed,
            this.unitsEnemyDestroyedText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleStructuresYouDestroyed,
            this.statistics.StructuresYouDestroyed,
            this.structuresYouDestroyedText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleStructuresEnemyDestroyed,
            this.statistics.StructuresEnemyDestroyed,
            this.structuresEnemyDestroyedText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleGoldYouMined,
            this.statistics.GoldYouMined,
            this.goldYouMinedText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleGoldEnemyMined, 
            this.statistics.GoldEnemyMined, 
            this.goldEnemyMinedText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleUnitsYouTrained, 
            this.statistics.UnitsYouTrained, 
            this.unitsYouTrainedText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleUnitsEnemyTrained, 
            this.statistics.UnitsEnemyTrained, 
            this.unitsEnemyTrainedText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleStructuresYouBuilt, 
            this.statistics.StructuresYouBuilt, 
            this.structuresYouBuiltText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleStructuresEnemyBuilt,
            this.statistics.StructuresEnemyBuilt, 
            this.structuresEnemyBuiltText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleLumberYouHarvested, 
            this.statistics.LumberYouHarvested, 
            this.lumberYouHarvestedText
        );

        Utils.ScrollResourceNumber (
            ref this.visibleLumberEnemyHarvested, 
            this.statistics.LumberEnemyHarvested, 
            this.lumberEnemyHarvestedText
        );
    }
}
