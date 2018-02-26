using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.IO;

public class playerController : MonoBehaviour
{
    Animator animator;
    public GameObject VideoPlayback;
    MediaPlayerCtrl mediaPlayerCtrl;
    Vector3 ts;
    public skeletonController sc;
    public BloodBox bloodBox;
    private float allHp = 2000f;
    public float currentHp;
    public float playerPower = 1000f;
    private GameController gameController;
    public Transform playerMoveTrans;
    public float playerMoveSpeed;
    public GameObject daoEffect;
    public BarbarianController barbarianController;
    public float playerAttackRange;
    public Transform daoEffectPosition;
    public CameraMove cm;
    public Transform FallDownPosition;
    public float FallDownWalkForwardTime;
    private bool setWalk = false;
    public float FallDownUpRotation;
    public float FallDownUpTime;
    public float FallDownRotation;
    public float FallDownTime;
    public GameObject ShakeCamera;
    public float FallDownShakeTime;
    public float DelayFallDownTime;
    public float DelayPlayFurionTime;
    public bool isPlayerAttack;
    private int attackTimes;
    public HttpManager httpManager;
    private int count;
    public int counter;

    public String host = "128.46.101.85";
    public Int32 port = 8888;

    internal Boolean socket_ready = false;
    internal String input_buffer = "";
    TcpClient tcp_socket;
    NetworkStream net_stream;

    StreamWriter socket_writer;
    StreamReader socket_reader;

    void Awake()
    {
        attackTimes = 0;
        currentHp = allHp;
        ts = this.transform.position;
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("Idling", true);//stop moving
        mediaPlayerCtrl = VideoPlayback.GetComponent<MediaPlayerCtrl>();
    }
    IEnumerator Example()
    {
        print(Time.time);
        yield return new WaitForSeconds(30);
        print(Time.time);
        print("\n 30 seconds finished");
        counter = 1;
    }
    void Start()
    {
        count = 0;
        counter = 0;
        gameController = GameController.GetInstance();
        //setupSocket();
        /*StartCoroutine("downloadfile");
        StartCoroutine(Example()); //included in the start to download the video completely*/
        if (!gameController.isPlayerBloodVisible)
        {
            bloodBox.gameObject.SetActive(false);
        }
        if (gameController.isRecordMode)
        {
            StartCoroutine("DelayPlayFallDown");
        }
    }
    /*void OnApplicationQuit()
    {
        closeSocket();
    }*/

    void Update()
    {
        //remember the On Computer Debug in inspector (GameController)
        if (gameController.isRecordMode)
        {
			if (setWalk)
			{
				animator.SetBool("Idling", false);
                Debug.Log("walking call play()\n");
                mediaPlayerCtrl.Play();
				
			}
			else
			{
				animator.SetBool("Idling", true);
                Debug.Log("call pause()\n");
                mediaPlayerCtrl.Pause();
			}
			if (isPlayerAttack)
			{
                Debug.Log("set trigger\n");
                animator.SetTrigger("Use");
			}
        }
        //we can also replace this section with just mediaPlyerctrl.setDirection(1) or any value.....

        if (GvrController.IsTouching)
        {
            Vector2 current_touchpos = GvrController.TouchPos;

            //Debug.Log("EasyMovieTextureCsharp, current_touchpos = "+ current_touchpos.x +" , " + current_touchpos.y);

            if ((current_touchpos.y < current_touchpos.x) && (current_touchpos.y < 1 - current_touchpos.x))
            {
                //Debug.Log("EasyMovieTextureCsharp, current_touchpos = forward");
                mediaPlayerCtrl.setDirection(1);
            }
            else if ((current_touchpos.y < current_touchpos.x) && (current_touchpos.y > 1 - current_touchpos.x))
            {
                //Debug.Log("EasyMovieTextureCsharp, current_touchpos = right");
                mediaPlayerCtrl.setDirection(161);
            }
            else if ((current_touchpos.y > current_touchpos.x) && (current_touchpos.y > 1 - current_touchpos.x))
            {
                //Debug.Log("EasyMovieTextureCsharp, current_touchpos = backward");
                mediaPlayerCtrl.setDirection(-1);
            }
            else if ((current_touchpos.y > current_touchpos.x) && (current_touchpos.y < 1 - current_touchpos.x))
            {
                //Debug.Log("EasyMovieTextureCsharp, current_touchpos = left");
                mediaPlayerCtrl.setDirection(-161);
            }
            print(Time.time);
            if (counter == 0)
            {
                StartCoroutine(Example());
                
            }
            print(Time.time);
        }

    }


    int frameID = 0;
    int direction = 1;

    int getFrameID()
    {
        if(direction==1)
        { 
        frameID++;
        if(frameID>63)
        {
            frameID = 0;
        }
        }
        else
        {
            frameID--;
            if(frameID<0)
            {
                frameID = 63;
            }
        }
        return frameID;
    }

    void LateUpdate()
    {
        //视频录制用
        print("\n 30 seconds entering into lateupdate");
		if(gameController.isRecordMode)
        {
            return;
        }

		//=========================在Android手机VR播放时=========================
        if (!gameController.OnComputerDebug)
        {
			if (GvrController.IsTouching)
			{
				animator.SetBool("Idling", false);
				this.transform.position = new Vector3(ts.x, ts.y, ts.z += (Time.deltaTime * playerMoveSpeed));
                Debug.Log("\n app button is touching and the modified position is= " + this.transform.position);
                mediaPlayerCtrl.Play();
                Debug.Log("GvrController.IsTouching");

                //mediaPlayerCtrl.Play();
                //mediaPlayerCtrl.setDirection(getFrameID());
                //Debug.Log("call play()\n");

                /*if (gameController.IsInternetMode)
                {
					count++;
					//Debug.Log("count:" + count);
					if (count % 60 == 0)
					{
						//Debug.Log("DownLoadViking");
						httpManager.DownLoadViking();
					}
                }*/
			}
			else
			{
				animator.SetBool("Idling", true);
				mediaPlayerCtrl.Pause();
			}
			if (GvrController.AppButtonDown)
			{
				animator.SetTrigger("Use");
                //direction = -direction;
                //mediaPlayerCtrl.setDirection(direction);
                //Debug.Log("GvrController.AppButtonDown");
            }

           /*Vector2 current_touchpos = GvrController.TouchPos;

            //Debug.Log("EasyMovieTextureCsharp, current_touchpos = "+ current_touchpos.x +" , " + current_touchpos.y);

            if( (current_touchpos.y< current_touchpos.x) && (current_touchpos.y < 1- current_touchpos.x))
            {
                //Debug.Log("EasyMovieTextureCsharp, current_touchpos = forward");
                mediaPlayerCtrl.setDirection(1);
            }
            else if((current_touchpos.y < current_touchpos.x) && (current_touchpos.y > 1 - current_touchpos.x))
            {
                //Debug.Log("EasyMovieTextureCsharp, current_touchpos = right");
                mediaPlayerCtrl.setDirection(161);
            }
            else if ((current_touchpos.y > current_touchpos.x) && (current_touchpos.y > 1 - current_touchpos.x))
            {
                //Debug.Log("EasyMovieTextureCsharp, current_touchpos = backward");
                mediaPlayerCtrl.setDirection(-1);
            }
            else if((current_touchpos.y > current_touchpos.x) && (current_touchpos.y < 1 - current_touchpos.x))
            {
                //Debug.Log("EasyMovieTextureCsharp, current_touchpos = left");
                mediaPlayerCtrl.setDirection(-161);
            }
            print(Time.time);
            StartCoroutine(Example());
            print(Time.time);*/
            //mediaPlayerCtrl.Play();
        }
		//=========================在Android手机VR播放时=========================

		//=========================在Mac调试播放时=========================
		if(gameController.OnComputerDebug)
        {
			if (Input.GetKey(KeyCode.W))
			{
				animator.SetBool("Idling", false);
                this.transform.position = new Vector3(ts.x, ts.y, ts.z+=(Time.deltaTime * playerMoveSpeed));
				mediaPlayerCtrl.Play();
				if (gameController.IsInternetMode)
				{
					count++;
					Debug.Log("count:" + count);
					if (count % 60 == 0)
					{
						Debug.Log("DownLoadViking");
						httpManager.DownLoadViking();
					}
				}
            }else
            {
				animator.SetBool("Idling", true);
				mediaPlayerCtrl.Pause();
            }
			if (Input.GetKeyDown(KeyCode.E))
			{
				animator.SetTrigger("Use");
			}
        }
		//=========================在Mac调试播放时=========================
	}
    public void ScBeHitted()
    {
        if(sc!= null)
        {
           sc.BeHitted(); 
        }
    }

    public void BarnarianBeHitted()
    {
        setWalk = false;
        attackTimes++;
        //击打次数
		if (attackTimes  > 0)
		{
			isPlayerAttack = false;
		}
        if(barbarianController != null)
        {
            float distance = Vector3.Distance(this.transform.position, barbarianController.transform.position);
            //Debug.Log(distance);
            if(distance <playerAttackRange)
            {
                //Debug.Log("BarnarianBeHitted");
                Instantiate(daoEffect,daoEffectPosition);
                barbarianController.BeHitted();
                cm.ShakeCamera(0.1f);
            }
        }
    }



    public void SetBoold(float changeBlood)
    {
        currentHp = currentHp - changeBlood;
        if (currentHp <= 0.0f){
            //死亡
            currentHp = 0.0f;
            animator.SetInteger("Death", 2);
			if (sc != null)
			{
				sc.isPlayerDie = true;
			}
        }
        if(gameController.isPlayerBloodVisible)
        {
            bloodBox.OnBloodChange(allHp, currentHp);
        }
    }

    /// <summary>
    /// 向前
    /// </summary>
    private void FallDownAction_1()
    {
        setWalk = true;
		//键值对儿的形式保存iTween所用到的参数
		Hashtable args = new Hashtable();
		//这里是设置类型，iTween的类型又很多种，在源码中的枚举EaseType中
		//例如移动的特效，先震动在移动、先后退在移动、先加速在变速、等等
        args.Add("easeType", iTween.EaseType.linear);
		// x y z 标示移动的位置。
        args.Add("x", FallDownPosition.position.x);
		args.Add("y", FallDownPosition.position.y);
		args.Add("z", FallDownPosition.position.z);

		//移动的时间
		args.Add("time", FallDownWalkForwardTime);
        if(gameController.isFallDownMode)
        {
            args.Add("oncomplete", "FallDownAction_2");
        }else{
            //args.Add("oncomplete", "LookRight");
        }
        iTween.MoveTo(this.gameObject, args);

    }

    /// <summary>
    /// 被绊倒头朝上
    /// </summary>
    private void FallDownAction_2()
	{
        Debug.Log("FallDownAction_2");
        setWalk = false;
		//键值对儿的形式保存iTween所用到的参数  
		Hashtable args = new Hashtable();
		//args.Add("delay", FallDownShakeTime);
		// x y z 旋转的角度  
		args.Add("x", FallDownUpRotation);
		args.Add("y", 0f);
		args.Add("z", 0f);
        //时间
        args.Add("time", FallDownUpTime);
		args.Add("oncompleteparams", FallDownTime);
		if (gameController.isFallDownMode)
		{
            args.Add("oncomplete", "FallDownAction_3");
			cm.ShakeCamera(FallDownShakeTime);
        }else
        {
            args.Add("oncomplete", "LookRight");
        }
		iTween.RotateTo(ShakeCamera, args);
	}


	/// <summary>
	/// 向右看
	/// </summary>
	private void LookRight()
	{
        //setWalk = false;
        //Debug.Log("LookRight");
		//键值对儿的形式保存iTween所用到的参数
          
		Hashtable args = new Hashtable();
        args.Add("easeType", iTween.EaseType.easeOutBack);
		args.Add("from", 0f);
		args.Add("to", 80f);
		//变化过程中（ValueTo必写参数）  
		args.Add("onupdate", "AnimationUpdata");
		args.Add("onupdatetarget", gameObject);
		args.Add("oncomplete", "LookLeft");
        args.Add("time", 0.7f);
		iTween.ValueTo(ShakeCamera, args);
	}

	public void AnimationUpdata(object obj)
	{
		float per = (float)obj;
        cm.transform.rotation = Quaternion.Euler(new Vector3(0f, per, 0f));
	}

    public IEnumerator DelayPlayFallDown()
    {
        yield return new WaitForSeconds(1f);
        FallDownAction_1();
        yield return new WaitForSeconds(1f);
        LookRight();
        yield return null;
    }

    
    // Update is called once per frame
void downloadfile()
    {
        string received_data = readSocket();
        string key_stroke = Input.inputString;

        // Collects keystrokes into a buffer
        if (key_stroke != "")
        {
            input_buffer += key_stroke;

            if (key_stroke == "\n")
            {
                // Send the buffer, clean it
                Debug.Log("Sending: " + input_buffer);
                writeSocket(input_buffer);
                input_buffer = "";
            }

        }


        if (received_data != "")
        {
            // Do something with the received data,
            // print it in the log for now
            Debug.Log(received_data);
        }
    }

    public void setupSocket()
    {
        try
        {
            tcp_socket = new TcpClient(host, port);

            net_stream = tcp_socket.GetStream();
            socket_writer = new StreamWriter(net_stream);
            socket_reader = new StreamReader(net_stream);

            socket_ready = true;
        }
        catch (Exception e)
        {
            // Something went wrong
            Debug.Log("Socket error: " + e);
        }
    }

    public void writeSocket(string line)
    {
        if (!socket_ready)
            return;

        line = line + "\r\n";
        socket_writer.Write(line);
        socket_writer.Flush();
    }

    public String readSocket()
    {
        if (!socket_ready)
            return "";

        if (net_stream.DataAvailable)
            return socket_reader.ReadLine();

        return "";
    }

    public void closeSocket()
    {
        if (!socket_ready)
            return;

        socket_writer.Close();
        socket_reader.Close();
        tcp_socket.Close();
        socket_ready = false;
    }

}
