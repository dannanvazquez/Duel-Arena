using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeHit : NetworkBehaviour {
    [SerializeField] private Animator animator;

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
    }

    private void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if (!IsLocalPlayer) return;

        if (canHit && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.A))) {
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
        //transform.LookAt(new Vector3(dir.x, transform.position.y, dir.z));
        if (animator) {
            animator.SetTrigger("MeleeHit");
        }
    }

    private IEnumerator HitTiming() {
        isHitting = true;
        canHit = false;

        yield return new WaitForSeconds(hitAnimationTime);

        isHitting = false;

        yield return new WaitForSeconds(hitCooldown);

        canHit = true;
    }

    private void HitHittableColliders(Vector3 dir) {
        LayerMask hittableMask = 1 << LayerMask.NameToLayer("Hittable");
        Collider[] hittableColliders = Physics.OverlapSphere(transform.position, hitRadius, hittableMask);
        foreach (var hittableCollider in hittableColliders) {
            if (hittableCollider.CompareTag("Ball")) {
                hittableCollider.GetComponent<Rigidbody>().velocity = dir * hitForce;
            }
        }
    }
}
