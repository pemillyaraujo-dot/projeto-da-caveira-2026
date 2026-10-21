using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform alvo; // arraste o esqueleto aqui
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -4f);
    [SerializeField] private float suavidade = 5f;

    void LateUpdate()
    {
        if (alvo == null) return;

        Vector3 posicaoDesejada = alvo.position + alvo.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, posicaoDesejada, suavidade * Time.deltaTime);
        transform.LookAt(alvo.position + Vector3.up * 1.5f); // olha um pouco acima dos "pés"
    }
}
