using System.Collections;
using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{
    public enum ElevatorState { AtTop, Moving, AtBottom }

    [Header("이동 지점 설정")]
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;

    [Header("동작 설정")]
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float startDelay = 0.5f;

    private ElevatorState currentState = ElevatorState.AtTop;

    private Transform playerTransform;

    void Start()
    {
        if (topPoint != null)
        {
            transform.position = topPoint.position;
            currentState = ElevatorState.AtTop;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contact = collision.GetContact(0);
            if (contact.normal.y < -0.5f)
            {
                playerTransform = collision.transform;
                playerTransform.SetParent(transform);

                if (currentState == ElevatorState.AtTop)
                {
                    StartCoroutine(MoveRoutine(bottomPoint.position, ElevatorState.AtBottom));
                }
                else if (currentState == ElevatorState.AtBottom)
                {
                    StartCoroutine(MoveRoutine(topPoint.position, ElevatorState.AtTop));
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerTransform != null)
            {
                playerTransform.SetParent(null);
                playerTransform = null;
            }
        }
    }

    private IEnumerator MoveRoutine(Vector3 targetPos, ElevatorState arrivalState)
    {
        currentState = ElevatorState.Moving;

        yield return new WaitForSeconds(startDelay);

        while (Vector3.Distance(transform.position, targetPos) > 0.02f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        currentState = arrivalState;
    }

    void Update()
    {

    }
}