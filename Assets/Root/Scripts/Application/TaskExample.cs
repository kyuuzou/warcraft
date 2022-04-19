using System.Collections;
using UnityEngine;

public class TaskExample : MonoBehaviour {

    private Task spam;

    private IEnumerator MyCoroutine() {
        yield return this.StartCoroutine(this.MyCoroutineCascadeOne());
    }

    private IEnumerator MyCoroutineCascadeOne() {
        this.spam = new Task(this.MyCoroutineCascadeTwo(), false);
        yield return this.StartCoroutine(this.spam.YieldStart());
    }

    private IEnumerator MyCoroutineCascadeTwo() {
        Task spam = new Task(this.MyCoroutineCascadeThree(), false, this.spam);
        yield return this.StartCoroutine(spam.YieldStart());

        //yield return this.StartCoroutine (this.MyCoroutineCascadeThree ());
    }

    private IEnumerator MyCoroutineCascadeThree() {
        while (true) {
            Debug.Log("Spam test logger ");
            yield return null;
        }
    }

    private void Start() {
        this.StartCoroutine(this.MyCoroutine());
    }

    private void Update() {
        if (Time.timeSinceLevelLoad > 1 && this.spam.Running) {
            Debug.Log("stopping ");
            this.spam.Stop();
        }
    }
}
