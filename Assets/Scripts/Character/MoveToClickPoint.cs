using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveToClickPoint : NetworkBehaviour {
    [SerializeField] private Animator animator;

    private NavMeshAgent navMeshAgent;

    private void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
    }

    private void Update() {
        if (IsLocalPlayer) {
            if (Input.GetMouseButton(1)) {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100)) {
                    SetDestinationServerRpc(hit.point);
                }
            }
        }

        if (animator) {
            animator.SetBool("IsRunning", navMeshAgent.velocity != Vector3.zero);
        }
    }

    private void LateUpdate() {
        if (GetComponent<MeleeHit>().isHitting == false && navMeshAgent.velocity != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
        }
    }

    [ServerRpc]
    private void SetDestinationServerRpc(Vector3 dest) {
        SetDestinationClientRpc(dest);
    }

    [ClientRpc]
    private void SetDestinationClientRpc(Vector3 dest) {
        navMeshAgent.destination = dest;
    }
}
