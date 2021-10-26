using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; //Работа с аудио
using UnityEngine.UI;//работа с ui

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class UIScript : MonoBehaviour{
    public static float volume = 0; //Громкость
    public int quality = 0; //Качество
    public bool isFullscreen = false; //Полноэкранный режим
    public float chustvitelnost=60f;

    private int currResolutionIndex = 0; //Текущее разрешение
    public int sit=1;

    public bool menuTrans = false;//меню транспорта
    public bool pauseStatus = false;//меню пауза
    public bool EnventarOpen = false;//менб инвентаря
    public bool inTransport = false;//находиться ли игрок в транспорте
    public bool seting = false;//меню настроек
    public bool nonemenu = true;//
    public bool padOpen = false;
    public bool inoe = false;

    public Canvas Glav;//главний канвас
    public Canvas AndroC;//андроид канвас
    public Canvas Setting;//канвас настроек
    public Canvas PauseS;//канвас настроек
    public Canvas AudioC;//канвас настроек аудио
    public Canvas GraficC;//канвас настроек графики
    public Canvas GameC;//канвас настроек игры
    public Canvas DebugbC;//канвас настроек дебага игры
    public Canvas Enventar;//канвас инвентаря
    public Canvas PadC;

    public bool MobileD;//деваис андроид vg
    public bool PcD;//девайс пк
    public bool isServer;

    public Texture2D centerScreen;//текстура по центру екрана

    public float speedPlayer;//скорость ходьби игрока
    public float speedRunPlayer;
    public float speedObj;//переменая для ограничения скорости
    public GameObject player;//игрок обект
    private Rigidbody _rb;//rigidbody игрока
    private RigidbodyConstraints rbc;
    public Camera manCam;//камера игрока
    private CapsuleCollider _collider;//player colader
    private float horizontal=0f;//значение от -1 до 1
    private float vertical=0f;//значение от -1 до 1
    private float horV;//для передачи
    private float verV;//для пердачи
    public bool tsb = false;//если любая кнопка нажата но не отпущена на андроед
    public bool jumpVelocity = false;
    public float jumpForse = 0;
    float nsp;
    public bool flye = false;
    private float yVecSpeed;


    public bool survivalMode = true;
    public bool sandboxMode = false;


    public Text TextSettingChustvittelnost;//получаем обект текста настоек чуствительности


    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();//получаем ригид боди игрока
        _collider = GetComponent<CapsuleCollider>();//получаем колайдер игрока
        //т.к. нам не нужно что бы персонаж мог падать сам по-себе без нашего на то указания.
        //то нужно заблочить поворот по осях X и Z
        nsp=speedPlayer;

        //проверка на каком устроистве
        #if UNITY_STANDALONE || UNITY_EDITOR
            PcD=true;
        #elif UNITY_ANDROID || UNITY_IOS || UNITY_REMOTE
            MobileD=true;

        #endif
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        rbc = _rb.constraints;
    }

    private void FixedUpdate()
    {
        if(nonemenu){//если не отрито никакое меню
            MoveLogic();//визиваем логику перемещения
        }
        levoe();//функция для мелких скриптов постояного действия
        BugFix();//функция для предотвращения багов
    }
    // Update is called once per frame
    private void Update()
    {

            if (Input.GetKeyDown(KeyCode.T)) MenuTransF();//кнопка для меню транспорта
            if (Input.GetKeyDown(KeyCode.Escape)) Pause();//кнопка для паузи
            if (Input.GetKeyDown(KeyCode.E)) EnventarBo();//кнопка для инвинтаря
            if (Input.GetKeyDown(KeyCode.P)) PadOpen();
            if(nonemenu){//если не включено любое меню

            }
            if(Input.GetKeyDown(KeyCode.G)){//если кнопка нажалась
              Cursor.lockState=CursorLockMode.Locked;//блокируем курсор
            }
            if(Input.GetKeyDown(KeyCode.Y)){//если кнопка нажалась
              Cursor.lockState=CursorLockMode.None;//включаем курсор
            }
            bool w,leftctrl;

            w=(Input.GetKey(KeyCode.W) ? true : false);
            leftctrl=(Input.GetKey(KeyCode.LeftControl) ? true : false);
            if(w&&leftctrl)
            {
                speedPlayer = speedRunPlayer;
            }
            else
            {
              speedPlayer = nsp;
            }
            if(Input.GetKeyDown(KeyCode.F))
            {
                flye=!flye;
            }

    	}
      private void levoe(){

            if(seting==true){//если переменая настроек истина
                Setting.gameObject.SetActive(true);//активируем канвас настроек
                if(seting==true&&sit==1) AudioC.gameObject.SetActive(true);//если меню настроек и подменю настроек 1 включаем канвас настроек аудио
                    else AudioC.gameObject.SetActive(false);//если нет выключаем
                if(seting==true&&sit==2) GraficC.gameObject.SetActive(true);//если меню настроек и подменю настроек 2
                    else GraficC.gameObject.SetActive(false);//если нет выключаем
                if (seting==true&&sit==3) GameC.gameObject.SetActive(true);//если меню настроек и подменю настроек 3
                    else GameC.gameObject.SetActive(false);//если нет выключаем
                if (seting==true&&sit==4) DebugbC.gameObject.SetActive(true);//если меню настроек и подменю настроек 4
                    else DebugbC.gameObject.SetActive(false);//если нет выключаем
            }
            else
            {
                Setting.gameObject.SetActive(false);//если нет выключаем канвас настроек
            }
            if(pauseStatus==true)
            {
                PauseS.gameObject.SetActive(true);
            }
            else
            {
                PauseS.gameObject.SetActive(false);
            }
            if(nonemenu==true)
            {
                Glav.gameObject.SetActive(true);
            }
            else
            {
                Glav.gameObject.SetActive(false);
            }
            if(nonemenu==true&&MobileD==true)
            {
                AndroC.gameObject.SetActive(true);
            }
            else
            {
                AndroC.gameObject.SetActive(false);
            }
            if(EnventarOpen==true)
            {
                Enventar.gameObject.SetActive(true);
            }
            else
            {
                Enventar.gameObject.SetActive(false);
            }
            if(padOpen==true)
		        {
                PadC.gameObject.SetActive(true);
		        }
            else
		        {
                PadC.gameObject.SetActive(false);
            }

            horizontal=horV;//передаем значения
            vertical=verV;//передает значения

            if(inoe)
            {
                nonemenu = false;
                padOpen= false;
                EnventarOpen = false;
                pauseStatus = false;
                menuTrans = false;
                seting = false;
            }

            if (nonemenu)
            {
                Cursor.lockState=CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState=CursorLockMode.None;
            }

            if (Input.GetKey(KeyCode.Space) && flye)
		        {
                gameObject.transform.position += new Vector3(0,3,0) * Time.deltaTime;
            }
            if(Input.GetKey(KeyCode.LeftShift) && flye)
            {
              gameObject.transform.position += new Vector3(0,-3,0) * Time.deltaTime;
            }

            if(flye)
            {
                _rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                _rb.constraints = rbc;
            }



      }
      void OnGUI()
      {
        if(nonemenu){
         GUI.DrawTexture(new Rect(Screen.width/2,Screen.height/2, 5, 5), centerScreen);//отрисовуем по центру екрана точку
       }
      }
      private void MoveLogic()//логика передвижения
      {
        _rb.velocity = _movementVector;//ограничиваем скорость
      }
    //вектор направления

     public Vector3 _movementVector//вектор направления движения
     {
        get
        {
            float horsp = 0.0f;
            float versp = 0.0f;
            if(!tsb)//если игра на пк
            {
                horizontal = Input.GetAxis("Horizontal");//получаем направлении вперед назад
                vertical = Input.GetAxis("Vertical");//получаем направлении лево право
            }
            horsp = horizontal * speedPlayer;
            versp = vertical * speedPlayer;

            if(flye)
			      {
                return transform.rotation * new Vector3(horsp, _rb.velocity.y + yVecSpeed, versp);//возращаем направление
            }
            else
      			{
                return transform.rotation * new Vector3(horsp, _rb.velocity.y, versp);//возращаем направление
            }

        }
     }
    private void BugFix(){//функция баг фиксов
        if(!pauseStatus&&!menuTrans&&!EnventarOpen&&!seting&&!padOpen&&!inoe) nonemenu=true;//если не какие меню не открити
    }
    public void OnGo(float asd){horV=asd; }//функция визова из вне
    public void OnBok(float bsd){verV=bsd; }//функция вызова из вне
    public void OnGG(){horizontal=0.98f;}//функция вызова из вне
    public void OnUpBtn(){horV=0.0f;verV=0.0f; }//функция вызова из вне



    public void ChangeVolume(float val) //Изменение звука
    {
        volume = val;
    }

    public void ChangeResolution(int index) //Изменение разрешения
    {
        currResolutionIndex = index;
    }
    public void SetChustvitelnost(Slider a)//получаем значение из слайдера для регулирования чуствительности
    {
      chustvitelnost=a.value;//чуствительность равна значению слайдера
        TextSettingChustvittelnost.text = "Chustvitelnost: " + a.value.ToString();
    }
    public void QuitGame()
    {
        Application.Quit(); //Закрытие игры. В редакторе, кончено, она закрыта не будет, поэтому для проверки можно использовать Debug.Log();
    }
    public void SitF(int qaw){sit=qaw;}//получаем номер откритого подменю настроек
    public void BackBtn(){pauseStatus=true;EnventarOpen=false;menuTrans=false;seting=false;nonemenu=false;inoe = false;}//включаем меню паузи
    public void BackToGame(){nonemenu=true;menuTrans=false;pauseStatus=false;EnventarOpen=false;seting=false;inoe = false;}//ключаем все меню
    public void MenuTransF() {menuTrans=!menuTrans;EnventarOpen=false;pauseStatus=false;seting=false;nonemenu=false;inoe = false;}//вкдючаем меню транспорта
    public void EnventarBo() {EnventarOpen=!EnventarOpen;menuTrans=false;pauseStatus=false;seting=false;nonemenu=false;inoe = false;}//включаем менюинвентаря
    public void Pause() { pauseStatus=!pauseStatus;EnventarOpen=false;menuTrans=false;seting=false;nonemenu=false;inoe = false;}//включаем меню паузи
    public void Setinds(){seting=!seting;pauseStatus=false;EnventarOpen=false;menuTrans=false;nonemenu=false;inoe = false;}//включаем меню настроек
    public void PadOpen() { padOpen = !padOpen; pauseStatus = false; EnventarOpen = false; nonemenu = false; seting = false; menuTrans = false;inoe = false; }
}
//
//
//
//
//
//
///
//
//
//
//
//
//
//
//
//
