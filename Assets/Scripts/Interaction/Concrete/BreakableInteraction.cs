using Dioinecail.ButtonUtil;
using System.Collections;
using UnityEngine;

namespace LinkedSquad.Interactions
{
    public class BreakableInteraction : MonoBehaviour, IBreakable
    {
        [SerializeField] private Transform[] m_PiecesRoots;
        [SerializeField] private float m_BreakThreshold;
        [SerializeField] private float m_HidePiecesTimer;

        [SerializeField] AudioClip m_BreakSoundClip;



        public void TryBreak(float mass, Vector3 direction)
        {
            if (mass < m_BreakThreshold)
                return;

            if(TryGetComponent<Collider>(out var thisCollider))
            {
                thisCollider.enabled = false;
            }

            if(TryGetComponent<MeshRenderer>(out var meshRenderer))
            {
                meshRenderer.enabled = false;
            }

            foreach (var pRoot in m_PiecesRoots)
            {
                pRoot.gameObject.SetActive(true);

                for (int i = 0; i < pRoot.childCount; i++)
                {
                    if (pRoot.GetChild(i).TryGetComponent<Rigidbody>(out var pieceRB))
                    {
                        var rndValue = UnityEngine.Random.value * 45f;

                        pieceRB.velocity = direction;
                        pieceRB.angularVelocity = new Vector3(rndValue, rndValue, rndValue);
                        pieceRB.isKinematic = false;
                    }
                }
            }

            if (m_HidePiecesTimer > 0f)
                StartCoroutine(CoroutineHidePieces());
        }

        private void OnCollisionEnter(Collision collision)
        {
            var mass = collision.rigidbody.mass;
            var relativeVelocity = collision.relativeVelocity;
            var impulse = collision.impulse;
            var normal = collision.GetContact(0).normal;
            var point = collision.GetContact(0).point;

            // Both bodies see the same impulse. Flip it for one of the bodies.
            if (Vector3.Dot(normal, relativeVelocity) < 0f)
                relativeVelocity *= -1f;

            TryBreak(mass, relativeVelocity);

            collision.rigidbody.velocity = relativeVelocity;

            DebugInfo(mass, relativeVelocity, impulse, point);
        }

        private IEnumerator CoroutineHidePieces()
        {
            yield return new WaitForSeconds(m_HidePiecesTimer);

            foreach (var pRoot in m_PiecesRoots)
            {
                for (int i = 0; i < pRoot.childCount; i++)
                {
                    pRoot.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        private void DebugInfo(float mass, Vector3 relativeVelocity, Vector3 impulse, Vector3 impactPoint)
        {
            var infoString = new System.Text.StringBuilder();

            infoString.AppendLine($"[mass:'{mass.ToString("0.00")}']");
            infoString.AppendLine($"[relativeVelocity:'{relativeVelocity.ToString()}']");
            infoString.AppendLine($"[impulse:'{impulse.ToString()}']");

            Debug.Log(infoString.ToString());
            Debug.DrawRay(impactPoint, relativeVelocity, Color.red, 9999f);
            Debug.DrawRay(impactPoint, impulse, Color.yellow, 9999f);
        }

#if UNITY_EDITOR

        private System.Collections.Generic.List<Transform> m_PiecesTransforms;
        private System.Collections.Generic.List<Vector3> m_PiecesPositions;
        private System.Collections.Generic.List<Quaternion> m_PiecesRotations;



        private void Awake()
        {
            m_PiecesTransforms = new System.Collections.Generic.List<Transform>();
            m_PiecesRotations = new System.Collections.Generic.List<Quaternion>();
            m_PiecesPositions = new System.Collections.Generic.List<Vector3>();

            foreach (var pRoot in m_PiecesRoots)
            {
                for (int i = 0; i < pRoot.childCount; i++)
                {
                    var index = i;

                    m_PiecesTransforms.Add(pRoot.GetChild(index));
                    m_PiecesPositions.Add(pRoot.GetChild(index).position);
                    m_PiecesRotations.Add(pRoot.GetChild(index).rotation);
                }
            }
        }

        [Button]
        private void ResetBreakable()
        {
            for (int i = 0; i < m_PiecesTransforms.Count; i++)
            {
                var piece = m_PiecesTransforms[i];

                piece.position = m_PiecesPositions[i];
                piece.rotation = m_PiecesRotations[i];

                if(piece.TryGetComponent<Rigidbody>(out var pieceRB))
                {
                    pieceRB.velocity = Vector3.zero;
                    pieceRB.angularVelocity = Vector3.zero;
                    pieceRB.isKinematic = true;
                }
            }

            if (TryGetComponent<Collider>(out var thisCollider))
            {
                thisCollider.enabled = true;
            }
        }

#endif
    }
}