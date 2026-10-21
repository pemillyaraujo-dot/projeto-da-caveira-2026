using UnityEngine;

public class SkeletonPlayer : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidadeAndar = 2f;
    [SerializeField] private float velocidadeCorrer = 4f;
    [SerializeField] private float velocidadeAndarTras = 1.5f;
    [SerializeField] private float velocidadeRotacao = 120f;

    [Header("Pulo")]
    [SerializeField] private float forcaPulo = 5f;
    [SerializeField] private float gravidade = -9.81f;

    private Animator animator;
    private CharacterController controller;
    private Vector3 velocidadeVertical;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Verifica teclas
        bool estaIndoFrente = Input.GetKey(KeyCode.W);
        bool estaIndoTras = Input.GetKey(KeyCode.S) && !estaIndoFrente;
        bool estaMovendo = estaIndoFrente || estaIndoTras;
        bool estaCorrendo =
            estaIndoFrente &&
            (Input.GetKey(KeyCode.LeftShift) ||
             Input.GetKey(KeyCode.RightShift));

        // -----------------------------
        // ROTAÇÃO
        // -----------------------------
        float direcaoRotacao = 0f;
        if (Input.GetKey(KeyCode.A))
            direcaoRotacao = -1f;
        if (Input.GetKey(KeyCode.D))
            direcaoRotacao = 1f;

        transform.Rotate(
            0f,
            direcaoRotacao * velocidadeRotacao * Time.deltaTime,
            0f
        );

        // -----------------------------
        // MOVIMENTO HORIZONTAL
        // -----------------------------
        float velocidadeAtual = 0f;
        float sentido = 1f;

        if (estaIndoFrente)
        {
            velocidadeAtual = estaCorrendo ? velocidadeCorrer : velocidadeAndar;
            sentido = 1f;
        }
        else if (estaIndoTras)
        {
            velocidadeAtual = velocidadeAndarTras;
            sentido = -1f;
        }

        Vector3 movimento = transform.forward * velocidadeAtual * sentido;

        // -----------------------------
        // PULO E GRAVIDADE
        // -----------------------------
        bool estaNoChao = controller.isGrounded;

        if (estaNoChao && velocidadeVertical.y < 0)
        {
            velocidadeVertical.y = -2f; // pequena força pra manter grudado no chão
        }

        if (estaNoChao && Input.GetKeyDown(KeyCode.Space))
        {
            velocidadeVertical.y = Mathf.Sqrt(forcaPulo * -2f * gravidade);
            animator.SetTrigger("jump"); // crie esse trigger no Animator
        }

        velocidadeVertical.y += gravidade * Time.deltaTime;

        // Move horizontal + vertical juntos
        Vector3 movimentoFinal = (movimento + velocidadeVertical);
        controller.Move(movimentoFinal * Time.deltaTime);

        // -----------------------------
        // ANIMAÇÕES
        // -----------------------------
        animator.SetBool("isMoving", estaMovendo);
        animator.SetBool("isRunning", estaCorrendo);
        animator.SetFloat("direcaoMovimento", estaMovendo ? sentido : 1f);
        animator.SetBool("isGrounded", estaNoChao); // útil pra transição de volta ao chão
    }
}