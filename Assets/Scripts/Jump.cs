using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{

  public LayerMask GroundLayer = 1; // 1 == "Default"
  private CapsuleCollider _collider;//player colader
  public float jmp=0;
    public UIScript uis;

    private Rigidbody _rb;//rigidbody игрока
    private bool _isGrounded;//земля под игороком
    public float jumForse;//сила прижка
    // Start is called before the first frame update
    void Start()
    {
      _rb = GetComponent<Rigidbody>();//получаем ригид боди игрока
      _collider = GetComponent<CapsuleCollider>();//получаем колайдер игрока
      //т.к. нам не нужно что бы персонаж мог падать сам по-себе без нашего на то указания.
      //то нужно заблочить поворот по осях X и Z

      _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;//блокируем поворот игрока по осям Х и z

      //  Защита от дурака
      if (GroundLayer == gameObject.layer)
          Debug.LogError("Player SortingLayer must be different from Ground  SourtingLayer!");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
JumpL();
    }
    private void JumpL(){
        if (Input.GetAxis("Jump")> 0 || jmp>0f){
            if (_isGrounded) {
                //uis.jumpVelocity = true; uis.jumpForse = jumForse;
                uis.jumpForse=9f;
                _rb.AddForce(Vector3.up*jumForse);
            } else {
               //uis.jumpVelocity = false; uis.jumpForse = 0;
               uis.jumpForse=0f;
            }
        }
    }//прижок_rb.AddForce(new Vector3(0,1,0) * jumForse);
    private void OnCollisionEnter(Collision collision) { IsGroundedUpdate(collision, true); }
    private void OnCollisionExit(Collision collision) { IsGroundedUpdate(collision, false); }
    private void IsGroundedUpdate(Collision collision,bool value){if(collision.gameObject.tag ==("Ground")){ _isGrounded = value; } }//проверка земли и ограничение на один прижок
    public void Jmpona(float sa){jmp=sa;}
    private bool _isGrounded1
      {
          get {
              var bottomCenterPoint = new Vector3(_collider.bounds.center.x, _collider.bounds.min.y, _collider.bounds.center.z);
              //создаем невидимую физическую капсулу и проверяем не пересекает ли она обьект который относится к полу
              //_collider.bounds.size.x / 2 * 0.9f -- эта странная конструкция берет радиус обьекта.
              // был бы обязательно сферой -- брался бы радиус напрямую, а так пишем по-универсальнее
              return Physics.CheckCapsule(_collider.bounds.center, bottomCenterPoint, _collider.bounds.size.x / 2 * 0.5f, GroundLayer);
              // если можно будет прыгать в воздухе, то нужно будет изменить коэфициент 0.9 на меньший.
          }
      }
      public void asd(float s)
      {
        jmp=s;
      }
}
