using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; //Работа с аудио
using UnityEngine.UI;//работа с ui

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class UIScript : MonoBehaviour{


    public bool isFullscreen = false; //Полноэкранный режим

    public bool survivalMode = true;
    public bool sandboxMode = false;

    [SerializeField]
    public bool MobileD;//деваис андроид vg
    [SerializeField]
    public bool PcD;//девайс пк
    [SerializeField]
    private bool isServer;

    [SerializeField]
    private bool menuTrans = false;//меню транспорта
    [SerializeField]
    private bool pauseStatus = false;//меню пауза
    [SerializeField]
    private bool EnventarOpen = false;//менб инвентаря
    [SerializeField]
    private bool inTransport = false;//находиться ли игрок в транспорте
    [SerializeField]
    private bool seting = false;//меню настроек
    [SerializeField]
    public bool nonemenu = true;//
    [SerializeField]
    public bool padOpen = false;
    [SerializeField]
    private bool menuMultyplayer = false;
    [SerializeField]
    private bool worldMenu = false;
    [SerializeField]
    private bool avariableEnebledMenus = false;//флаг когда изменяется активное меню
    [SerializeField]
    private bool avariableEnebledSubMenusSettings = false;//флаг когда изменяется активное меню sttings

    [SerializeField]
    private bool flye = false;

    [SerializeField]
    private Canvas Glav;//главний канвас
    [SerializeField]
    private Canvas AndroC;//андроид канвас
    [SerializeField]
    private Canvas Setting;//канвас настроек
    [SerializeField]
    private Canvas PauseS;//канвас настроек
    [SerializeField]
    private Canvas AudioC;//канвас настроек аудио
    [SerializeField]
    private Canvas GraficC;//канвас настроек графики
    [SerializeField]
    private Canvas GameC;//канвас настроек игры
    [SerializeField]
    private Canvas DebugbC;//канвас настроек дебага игры
    [SerializeField]
    private Canvas Enventar;//канвас инвентаря
    [SerializeField]
    private Canvas PadC;

    [SerializeField]
    public Camera manCam;//камера игрока

    private CapsuleCollider _collider;//player colader

    [SerializeField]
    public GameObject player;//игрок обект

    [SerializeField]
    private float speedPlayer;//скорость ходьби игрока
    [SerializeField]
    private float speedRunPlayer;
    [SerializeField]
    private float speedObj;//переменая для ограничения скорости
    [SerializeField]
    public float chustvitelnost=60f;
    private float horizontal=0f;//значение от -1 до 1
    private float vertical=0f;//значение от -1 до 1
    private float horV;//для передачи
    private float verV;//для пердачи
    [SerializeField]
    private float jumpForse = 0;
    private float yVecSpeed;
    float normalSpeedPlayer;

    private int currResolutionIndex = 0; //Текущее разрешение
    public int sit=1;


    [SerializeField]
    private Texture2D centerScreen;//текстура по центру екрана

    public Text TextSettingChustvittelnost;//получаем обект текста настоек чуствительности

    private Rigidbody _rb;//rigidbody игрока
    private RigidbodyConstraints rbc;

    public int hp = 100;
    public GameObject menuAddServerg;

    private float sprotcam ;





    // Start is called before the first frame update
    private void Start()
    {
        //проверка на каком устроистве
        #if UNITY_STANDALONE || UNITY_EDITOR
            PcD = true;
#elif UNITY_ANDROID || UNITY_IOS || UNITY_REMOTE
            MobileD = true;
#endif

sprotcam = (chustvitelnost / 180) * 16;


        _rb = GetComponent<Rigidbody>();//получаем ригид боди игрока
        _collider = GetComponent<CapsuleCollider>();//получаем колайдер игрока
        //т.к. нам не нужно что бы персонаж мог падать сам по-себе без нашего на то указания.
        //то нужно заблочить поворот по осях X и Z
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        rbc = _rb.constraints;
        normalSpeedPlayer = speedPlayer;
    }

    private void FixedUpdate()
    {
        if(nonemenu) _rb.velocity = _movementVector;//ограничиваем скорость

    }
    // Update is called once per frame
    private void Update()
    {
        levoe();//функция для мелких скриптов постояного действия
        if(true)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Pause();//кнопка для паузи
            if (Input.GetKeyDown(KeyCode.E)) EnventarBo();//кнопка для инвинтаря
            if (Input.GetKeyDown(KeyCode.P)) PadOpen();
            if (Input.GetKeyDown(KeyCode.G)) Cursor.lockState=CursorLockMode.Locked;//блокируем курсор
            if (Input.GetKeyDown(KeyCode.Y)) Cursor.lockState=CursorLockMode.None;//включаем курсор
        }
        if (nonemenu)
        {
            //if (Input.GetKeyDown(KeyCode.T)) MenuTransF();//кнопка для меню транспорта
            if(PcD){RotCAmera();}
            bool w, leftctrl;
            w = Input.GetKey(KeyCode.W) ? true : false;
            leftctrl = Input.GetKey(KeyCode.LeftControl) ? true : false;
            if (w && leftctrl) speedPlayer = speedRunPlayer;
            else speedPlayer = normalSpeedPlayer;

        if (Input.GetKeyDown(KeyCode.F)) flye = !flye;
        }
  	}
    private void levoe()
    {
        if (avariableEnebledSubMenusSettings)
        {
            avariableEnebledSubMenusSettings = false;
            if (seting && sit == 1) AudioC.gameObject.SetActive(true);//если меню настроек и подменю настроек 1 включаем канвас настроек аудио
            else AudioC.gameObject.SetActive(false);//если нет выключаем
            if (seting && sit == 2) GraficC.gameObject.SetActive(true);//если меню настроек и подменю настроек 2
            else GraficC.gameObject.SetActive(false);//если нет выключаем
            if (seting && sit == 3) GameC.gameObject.SetActive(true);//если меню настроек и подменю настроек 3
            else GameC.gameObject.SetActive(false);//если нет выключаем
            if (seting && sit == 4) DebugbC.gameObject.SetActive(true);//если меню настроек и подменю настроек 4
            else DebugbC.gameObject.SetActive(false);//если нет выключаем
        }
        if (nonemenu && MobileD)
        {
            AndroC.gameObject.SetActive(true);
        }
    		else
    		{
            AndroC.gameObject.SetActive(false);
        }
        if (!pauseStatus && !menuTrans && !EnventarOpen && !padOpen && !seting && !menuMultyplayer && !worldMenu) nonemenu = true;
        if (nonemenu) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
        horizontal = horV;//передаем значения
        vertical = verV;//передает значения
        if (Input.GetKey(KeyCode.Space) && flye) gameObject.transform.position += new Vector3(0,3,0) * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftShift) && flye) gameObject.transform.position += new Vector3(0,-3,0) * Time.deltaTime;
        if (flye) _rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        else _rb.constraints = rbc;

    }
    private void OnGUI()
    {
        if(nonemenu) GUI.DrawTexture(new Rect(Screen.width/2,Screen.height/2, 5, 5), centerScreen);//отрисовуем по центру екрана точку
    }
    //вектор направления

    public Vector3 _movementVector//вектор направления движения
    {
      get
      {
          float horsp = 0.0f;
          float versp = 0.0f;
          horizontal = Input.GetAxis("Horizontal");//получаем направлении вперед назад
          vertical = Input.GetAxis("Vertical");//получаем направлении лево право
          horsp = horizontal * speedPlayer;
          versp = vertical * speedPlayer;
          if(flye) return transform.rotation * new Vector3(horsp, _rb.velocity.y + yVecSpeed, versp);//возращаем направление
          else return transform.rotation * new Vector3(horsp, _rb.velocity.y, versp);//возращаем направление
      }
    }
    private void RotCAmera()
    {
       var yr=Input.GetAxis("Mouse X")*sprotcam*10*Time.fixedDeltaTime;//получаем 1 или -1 когда двигаться мишь по оси x
       var xr=-Input.GetAxis("Mouse Y")*sprotcam*10*Time.fixedDeltaTime;//получаем 1 или -1 когда двигаться мишь по оси y
       manCam.transform.rotation *= Quaternion.Euler(xr,0.0f,0.0f);//меняем поворот камери по оси х
       player.transform.rotation *= Quaternion.Euler(0.0f,yr,0.0f);//меняем поворот всего игрока по оси у
    }

    public void OnGo(float asd){horV = asd;}//функция визова из вне
    public void OnBok(float bsd){verV = bsd;}//функция вызова из вне
    public void OnGG(){horizontal = 0.98f;}//функция вызова из вне
    public void OnUpBtn(){horV = 0.0f;verV = 0.0f; }//функция вызова из вне
    public void ChangeResolution(int index){currResolutionIndex = index;}

    public void SetChustvitelnost(Slider a)//получаем значение из слайдера для регулирования чуствительности
    {
        chustvitelnost = a.value;//чуствительность равна значению слайдера
        TextSettingChustvittelnost.text = "Chustvitelnost: " + chustvitelnost.ToString();
        sprotcam = (chustvitelnost / 180) * 16;
    }
    public void QuitGame()
    {
        Application.Quit(); //Закрытие игры. В редакторе, кончено, она закрыта не будет, поэтому для проверки можно использовать Debug.Log();
    }
    public void SitF(int qaw){sit = qaw;avariableEnebledSubMenusSettings = true; }//получаем номер откритого подменю настроек

    public bool gsMenuMultiplayer
	{
        get
		{
            return menuMultyplayer;
		}
	}
    public bool gsMenuWorld
	{
		get
		{
            return worldMenu;
		}
	}

    public void BackBtn()
    {
        pauseStatus = true;
        EnventarOpen = false;
        menuTrans = false;
        seting = false;
        nonemenu = false;
        worldMenu = false;
        menuMultyplayer = false;
        PauseS.gameObject.SetActive(pauseStatus);
    }//включаем меню паузи
    public void BackToGame()
    {
        nonemenu = true;
        menuTrans = false;
        pauseStatus = false;
        EnventarOpen = false;
        seting = false;
        worldMenu = false;
        menuMultyplayer = false;
        PauseS.gameObject.SetActive(pauseStatus);
        Setting.gameObject.SetActive(seting);//активируем канвас настроек
        Glav.gameObject.SetActive(nonemenu);
        Enventar.gameObject.SetActive(EnventarOpen);
        PadC.gameObject.SetActive(padOpen);

    }//ключаем все меню
	//public void MenuTransF()
	//{
	//	avariableEnebledMenus = true;
	//	menuTrans = !menuTrans;
	//	EnventarOpen = false;
	//	pauseStatus = false;
	//	seting = false;
	//	nonemenu = false;
	//	worldMenu = false;
	//	menuMultyplayer = false;
	//}//вкдючаем меню транспорта
	public void EnventarBo()
    {
        EnventarOpen = !EnventarOpen;
        menuTrans = false;
        pauseStatus = false;
        seting = false;
        nonemenu = !EnventarOpen;
        worldMenu = false;
        menuMultyplayer = false;
        Enventar.gameObject.SetActive(EnventarOpen);
    }//включаем менюинвентаря
    public void Pause()
    {
        pauseStatus = !pauseStatus;
        EnventarOpen = false;
        menuTrans = false;
        seting = false;
        nonemenu = !pauseStatus;
        worldMenu = false;
        menuMultyplayer = false;
        PauseS.gameObject.SetActive(pauseStatus);
    }//включаем меню паузи
    public void Setinds()
    {
        seting = !seting;
        pauseStatus = false;
        EnventarOpen = false;
        menuTrans = false;
        nonemenu = !seting;
        worldMenu = false;
        menuMultyplayer = false;
        Setting.gameObject.SetActive(seting);//активируем канвас настроек
        avariableEnebledSubMenusSettings = true;
    }//включаем меню настроек
    public void PadOpen()
    {
        padOpen = !padOpen;
        pauseStatus = false;
        EnventarOpen = false;
        nonemenu = !padOpen;
        seting = false;
        menuTrans = false;
        worldMenu = false;
        menuMultyplayer = false;
        PadC.gameObject.SetActive(padOpen);
    }
    public void WorldMenu()
	{
        worldMenu = !worldMenu;
        padOpen = false;
        pauseStatus = false;
        EnventarOpen = false;
        nonemenu = !worldMenu;
        seting = false;
        menuTrans = false;
        menuMultyplayer = false;
	}
    public void MultyplayerMenu()
    {
        menuMultyplayer = !menuMultyplayer;
        worldMenu = false;
        padOpen = false;
        pauseStatus = false;
        EnventarOpen = false;
        nonemenu = !menuMultyplayer;
        seting = false;
        menuTrans = false;

    }
}
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
