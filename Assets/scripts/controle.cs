
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SkeletonPlayer : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidadeAndar = 2f;
    [SerializeField] private float velocidadeCorrer = 4f;
    [SerializeField] private float velocidadeAndarTras = 1.5f;
    [SerializeField] private float velocidadeRotacao = 120f;

    [Header("Pulo e Gravidade")]
    [SerializeField] private float forcaPulo = 5f;
    [SerializeField] private float gravidade = -9.81f;

    [Header("Ataque")]
    [SerializeField] private KeyCode teclaAtaque = KeyCode.Mouse0;

    private Animator animator;
    private CharacterController controller;

    private Vector3 velocidadeVertical;
    private Vector3 movimento;

    private bool estaIndoFrente;
    private bool estaIndoTras;
    private bool estaMovendo;
    private bool estaCorrendo;
    private bool estaNoChao;

    private float velocidadeAtual;
    private float sentido = 1f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError(
                "Animator não encontrado na caveira ou em seus filhos."
            );
            enabled = false;
            return;
        }
    }

    void Update()
    {
        Movimento();
        Rotacao();
        MovimentoHorizontal();
        PuloEGravidade();
        Animacoes();
        Ataque();
    }

    void Movimento()
    {
        estaIndoFrente = Input.GetKey(KeyCode.W);

        estaIndoTras = Input.GetKey(KeyCode.S)
                       && !estaIndoFrente;

        estaMovendo = estaIndoFrente || estaIndoTras;

        estaCorrendo = estaIndoFrente &&
            (Input.GetKey(KeyCode.LeftShift) ||
             Input.GetKey(KeyCode.RightShift));
    }

    void Rotacao()
    {
        float direcaoRotacao = 0f;

        if (Input.GetKey(KeyCode.A))
            direcaoRotacao -= 1f;

        if (Input.GetKey(KeyCode.D))
            direcaoRotacao += 1f;

        transform.Rotate(
            0f,
            direcaoRotacao * velocidadeRotacao * Time.deltaTime,
            0f
        );
    }

    void MovimentoHorizontal()
    {
        velocidadeAtual = 0f;
        sentido = 1f;

        if (estaIndoFrente)
        {
            velocidadeAtual = estaCorrendo
                ? velocidadeCorrer
                : velocidadeAndar;
        }
        else if (estaIndoTras)
        {
            velocidadeAtual = velocidadeAndarTras;
            sentido = -1f;
        }

        movimento = transform.forward * velocidadeAtual * sentido;
    }

    void PuloEGravidade()
    {
        estaNoChao = controller.isGrounded;

        if (estaNoChao && velocidadeVertical.y < 0f)
        {
            velocidadeVertical.y = -2f;
        }

        if (estaNoChao && Input.GetKeyDown(KeyCode.Space))
        {
            velocidadeVertical.y = Mathf.Sqrt(
                forcaPulo * -2f * gravidade
            );

            animator.SetTrigger("jump");
        }

        velocidadeVertical.y += gravidade * Time.deltaTime;

        Vector3 movimentoFinal = movimento;
        movimentoFinal.y = velocidadeVertical.y;

        controller.Move(movimentoFinal * Time.deltaTime);

        // Atualiza o estado do chão após o movimento.
        estaNoChao = controller.isGrounded;
    }

    void Ataque()
    {
        if (Input.GetKeyDown(teclaAtaque))
        {
            animator.SetTrigger("Attack");
        }
    }

    void Animacoes()
    {
        animator.SetBool("isMoving", estaMovendo);
        animator.SetBool("isRunning", estaCorrendo);
        animator.SetFloat(
            "direcaoMovimento",
            estaMovendo ? sentido : 1f
        );
        animator.SetBool("isGrounded", estaNoChao);
    }
}
