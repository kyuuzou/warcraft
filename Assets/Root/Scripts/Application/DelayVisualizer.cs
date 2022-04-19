using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DelayVisualizer : MonoBehaviour {

    [SerializeField]
    private float delay = 0.5f;

    private int number = 0;
    private Text textField;

    private void Start() {
        this.textField = this.GetComponent<Text>();

        this.StartCoroutine(this.Tick());
    }

    private IEnumerator Tick() {
        do {
            this.number++;
            this.textField.text = this.number.ToString();

            yield return new WaitForSeconds(this.delay);
        } while (true);
    }
}
