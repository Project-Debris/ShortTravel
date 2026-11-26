using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private Vector3 lastElevatorPos;

    [Header("이동 지점 설정")]
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;

    [Header("동작 설정")]
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float startDelay = 0.5f;

    public enum ElevatorState { AtTop, Moving, AtBottom }
    private ElevatorState currentState = ElevatorState.AtTop;
    // 전원 단자 연결 확인 -> 기본값 false
    private bool power = true;

    private Transform playerTransform;

    void Start()
    {
        if (topPoint != null)
        {
            transform.position = topPoint.position;
            currentState = ElevatorState.AtTop;
        }
        lastElevatorPos = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 elevatorDelta = transform.position - lastElevatorPos;

        if (playerRb != null)
        {
            playerRb.position += new Vector2(elevatorDelta.x, elevatorDelta.y);
        }

        lastElevatorPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 접촉면이 위인지 확인
            ContactPoint2D contact = collision.GetContact(0);
            if (contact.normal.y < -0.5f)
            {
                playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

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
            playerRb = null;
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
}