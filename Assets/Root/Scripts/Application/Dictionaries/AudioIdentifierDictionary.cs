
public class AudioIdentifierDictionary : InspectorDictionary<AudioIdentifierDictionary, AudioIdentifier, AudioSample> {

    protected override void InitializeEntries () {
        base.InitializeEntries ();

        foreach (AudioSample sample in this.Entries) {
            if (sample == null) {
                continue;
            }

            sample.Initialize ();
        }
    }
}
