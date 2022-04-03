using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour {

    private static TaskManager instance;
    public static TaskManager Instance {
        get {
            if (TaskManager.instance == null) {
                GameObject gameObject = new GameObject("Task Manager");
                TaskManager.Instance = gameObject.AddComponent<TaskManager>();
            }

            return TaskManager.instance;
        }

        set { TaskManager.instance = value; }
    }
}

public class Task {
    private List<Task> children;

    public bool Paused { get; private set; }
    public bool Running { get; private set; }
    public bool Aborted { get; private set; }

    private IEnumerator coroutine;

    public delegate void FinishedHandler();
    public event FinishedHandler Finished;

    public Task(IEnumerator coroutine, bool autoStart = true, Task parent = null) {
        this.coroutine = coroutine;
        this.children = new List<Task>();

        this.Paused = false;
        this.Running = false;
        this.Aborted = false;

        if (parent != null) {
            parent.AddChild(this);
        }

        if (autoStart) {
            this.Start();
        }
    }

    public void AddChild(Task task) {
        this.children.Add(task);
    }

    private void OnFinished() {
        if (this.Finished != null) {
            this.Finished();
        }
    }

    private IEnumerator ProcessCoroutine() {
        yield return null;

        IEnumerator coroutine = this.coroutine;

        while (this.Running) {
            if (this.Paused) {
                yield return null;
            } else {
                if (coroutine != null && coroutine.MoveNext()) {
                    yield return coroutine.Current;
                } else {
                    //Debug.Log ("ProcessRoutine finished");
                    this.Running = false;
                }
            }
        }

        this.OnFinished();
    }

    public void Start() {
        this.Running = true;
        TaskManager.Instance.StartCoroutine(this.ProcessCoroutine());
    }

    public IEnumerator YieldStart() {
        this.Running = true;
        yield return TaskManager.Instance.StartCoroutine(this.ProcessCoroutine());
    }

    public void Stop() {
        foreach (Task child in this.children) {
            child.Stop();
        }

        this.Running = false;
        this.Aborted = true;
    }
}
