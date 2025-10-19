using UnityEngine;
using System.Collections;

public class Necromancer : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] private float m_attackCooldown = 1.0f;
    [SerializeField] private float m_spellCooldown = 2.0f; 

    private float attackTimer = 0f;
    private float spellTimer = 0f;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_PixelHeroes m_groundSensor;
    private bool m_grounded = false;
    private float m_delayToIdle = 0.0f;

    // === Новые переменные ===
    private bool m_canMove = true;
    private float m_attackLockTimer = 0f;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_PixelHeroes>();
    }

    void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
        if (spellTimer > 0)
            spellTimer -= Time.deltaTime;
        // === Таймер блокировки движения после атаки ===
        if (!m_canMove)
        {
            m_attackLockTimer -= Time.deltaTime;
            if (m_attackLockTimer <= 0f)
                m_canMove = true;
        }

        // === Проверка касания земли ===
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }
        else if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // === Обработка движения ===
        float inputX = 0f;

        // Можно двигаться, если не заблокирован или если в воздухе
        if (m_canMove || !m_grounded)
            inputX = Input.GetAxis("Horizontal");

        // Поворот спрайта
        if (inputX > 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else if (inputX < 0)
            GetComponent<SpriteRenderer>().flipX = true;

        // Движение
        m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);

        // Скорость по оси Y для анимации
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        // // === Обработка действий ===
        // if (Input.GetKeyDown("e"))
        //  {
        //      m_animator.SetBool("noBlood", m_noBlood);
        //      m_animator.SetTrigger("Death");
        //  }
        // else if (Input.GetKeyDown("q"))
        // {
        //     m_animator.SetTrigger("Hurt");
        // }
        if (Input.GetMouseButtonDown(0) && attackTimer <= 0f)
        {
            m_animator.SetTrigger("Attack");

            if (m_grounded)
            {
                m_canMove = false;
                m_attackLockTimer = 0.8f;
            }

            attackTimer = m_attackCooldown;
        }
        else if (Input.GetMouseButtonDown(1) && spellTimer <= 0f)
        {
            if (m_grounded)
            {
                m_canMove = false;
                m_attackLockTimer = 1.4f;
                m_animator.SetTrigger("Spellcast");
            }
            spellTimer = m_spellCooldown;
        }
        else if (Input.GetKeyDown("space") && m_grounded && (m_canMove || !m_grounded))
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }
} 