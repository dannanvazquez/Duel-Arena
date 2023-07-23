using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeHit : NetworkBehaviour {
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject radiusIndicator;
    [SerializeField] private AudioClip[] kickBallClips;

    [Header("Settings")]
    [SerializeField] private float hitRadius = 1f;
    [SerializeField] private float hitForce = 5f;
    [SerializeField] private float hitAnimationTime = 0.5f;
    [SerializeField] private float hitCooldown = 1f;

    private NavMeshAgent navMeshAgent;

    public bool isHitting { get; private set; }
    public bool canHit { get; private set; }

    private void Awake() {
        isHitting = false;
        canHit = true;

        radiusIndicator.transform.localScale = new Vector3(hitRadius, hitRadius, 1f);
    }

    private void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if (!IsLocalPlayer) return;

        if (canHit && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q))) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100)) {
                Vector3 direction = hit.point - transform.position;
                direction.y = transform.position.y;
                HitDirectionServerRpc(direction.normalized);
            }
        }
    }

    [ServerRpc]
    private void HitDirectionServerRpc(Vector3 dir) {
        HitDirectionClientRpc(dir);
        HitHittableColliders(dir);
    }


    [ClientRpc]
    private void HitDirectionClientRpc(Vector3 dir) {
        StartCoroutine(HitTiming());

        transform.rotation = Quaternion.LookRotation(dir);

        if (animator) {
            animator.SetTrigger("MeleeHit");
        }
    }

    private IEnumerator HitTiming() {
        isHitting = true;
        canHit = false;

        if (IsLocalPlayer) {
            radiusIndicator.GetComponent<SpriteRenderer>().enabled = true;
        }

        yield return new WaitForSeconds(hitAnimationTime);

        isHitting = false;

        if (IsLocalPlayer) {
            radiusIndicator.GetComponent<SpriteRenderer>().enabled = false;
        }

        yield return new WaitForSeconds(hitCooldown);

        canHit = true;
    }

    private void HitHittableColliders(Vector3 dir) {
        LayerMask hittableMask = 1 << LayerMask.NameToLayer("Hittable");
        Collider[] hittableColliders = Physics.OverlapSphere(transform.position, hitRadius, hittableMask);
        foreach (var hittableCollider in hittableColliders) {
            if (hittableCollider.CompareTag("Ball")) {
                hittableCollider.GetComponent<Rigidbody>().velocity = dir * hitForce;
                PlayKickBallAudioClientRpc(Random.Range(0, kickBallClips.Length));
            }
        }
    }

    [ClientRpc]
    private void PlayKickBallAudioClientRpc(int index) {
        AudioSource.PlayClipAtPoint(kickBallClips[index], Camera.main.transform.position, 1f);
    }
}
