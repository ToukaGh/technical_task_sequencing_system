using System.Collections;
using UnityEngine;

[System.Serializable]
public class Action
{
    public AnimationClip animationClip;
    public AudioClip audioClip;
    public GameObject targetObject;
    public bool waitForAnimation;
    public bool waitForAudio;
    public bool enable;

}

public class SequencingSystem : MonoBehaviour
{
    public Action[] actions;
    public Animator playerAnimator;
    public AudioSource audioSource;
    public GameObject mainCamera;
    private int currentIndex;
    public Vector3 cameraPointA;
    public Vector3 cameraPointB;
    void Start()
    {
        ExecuteSequence();
    }

    public void ExecuteSequence()
    {
        StartCoroutine(MoveCamera(cameraPointA, cameraPointB));
        Action currentAction = actions[currentIndex];
        StartCoroutine(ExecuteAction(currentAction));
    }

    private IEnumerator ExecuteAction(Action action)
    {
        Coroutine animationCoroutine = null;
        Coroutine audioCoroutine = null;

        if (action.animationClip != null)
        {
            animationCoroutine = StartCoroutine(PlayAnimation(action.animationClip));
        }

        if (action.audioClip != null)
        {
            audioCoroutine = StartCoroutine(PlayAudio(action.audioClip));
        }

        if (action.waitForAnimation && animationCoroutine != null)
        {
            yield return animationCoroutine;
        }

        if (action.waitForAudio && audioCoroutine != null)
        {
            yield return audioCoroutine;
        }

        if (action.targetObject != null && action.enable)
        {
            action.targetObject.SetActive(true);
        }

        
        currentIndex++;
        ExecuteSequence();
    }

    private IEnumerator PlayAnimation(AnimationClip animationClip)
    {
        playerAnimator.Play(animationClip.name);
        yield return new WaitForSeconds(animationClip.length);
    }

    private IEnumerator PlayAudio(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        yield return new WaitForSeconds(audioClip.length);
    }

    private IEnumerator MoveCamera(Vector3 cameraPointA, Vector3 cameraPointB)
    {
        float duration = 5.0f; 
        float elapsed = 0f;
        while (true)
        {
            while (elapsed < duration)
            {
                mainCamera.transform.position = Vector3.Lerp(cameraPointA, cameraPointB, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCamera.transform.position = cameraPointB;

            Vector3 temp = cameraPointA;
            cameraPointA = cameraPointB;
            cameraPointB = temp;

            elapsed = 0f;
        }

    }

    void Update()
    {
        // Your update logic here
    }
}
