using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MystGameManager : SingletonMonoBehaviour<MystGameManager>
{

  public MystObjectManager objects;
  public Vector2 WINDOW_TOP_LEFT;
  public Vector2 WINDOW_BOTTOM_RIGHT;

  public int manager_state = 0;
  public bool is_title_exit = false;

  public GameObject GOBJ_MAIN_CAMERA;
  public Bloom BLOOM;

  public AudioSource AUDIO_BGM;
  public AudioClip CLIP_BGM_TITLE;
  public AudioClip CLIP_BGM_MAIN;

  public GameObject GOBJ_CANVAS;
  public Animator ANIM_TITLE;
  public GameObject GOBJ_TITLE;

  public int retry_count;
  public GameObject GOBJ_RETRY_COUNT_OVER;
  public GameObject GOBJ_RETRY_COUNT_CLEAR;

  public Vector2 PLAY_AREA;
  public GameObject GOBJ_PLAYER;
  public GameObject GOBJ_PLAYER_CHARGE;
  public bool player_enable_controlle;
  public float PLAYER_MOVE_SPEED;
  public int PLAYER_LAYER;
  public int player_life;
  public int player_state;
  private int PLAYER_LIFE_MAX;
  private float player_damage_start_time;
  private float PLAYER_DAMAGE_PERIOD;
  public bool player_is_nodamage;
  private Vector2 player_prev_pos;

  // public GameObject PRFB_PLAYER_SHOT;

  public GameObject GOBJ_OPTIONS;
  public OptionAnimation OPTIONS_ANIMATION;
  public float OPTIONS_MARGIN;
  public Vector2 options_direction;
  public float OPTIONS_DIRECTION_SPD;
  public int option_state; // 0:追従　10:射出 20:戻る
  public bool isLockOptionPosition;
  public Vector3 lockedRelativePosition;
  public float CHARGE_DURATION;
  public float charge_start_time;
  public bool is_charging;
  public float OPTIONS_SPD;
  public float OPTIONS_BACK_SPD;
  public int OPTIONS_LAYER;

  public int ENEMYS_LAYER;

  public GameObject PRFB_LOCK_OPTION;
  public GameObject PRFB_RELEASE_OPTION;
  public GameObject PRFB_SHOT_OPTION;

  public GameObject GOBJ_BOSS;
  public BossAction BOSS_ACTION;
  public int boss_state; // 0:標準 1:被ダメ
  public int boss_life;
  public int BOSS_LIFE_MAX;
  public GameObject GOBJ_BOSS_LIFE;
  private float BOSS_INITIAL_LIFE_HEIGHT;
  private float BOSS_LIFE_HEIGHT_POINT;
  private float boss_damage_start_time;
  private float BOSS_DAMAGE_PERIOD;
  public bool boss_is_nodamage;

  public GameObject PRFB_DAMAGE;
  public GameObject PRFB_DESTROY;

  public AudioHighPassFilter _HIGH_PASS_FILTER;

  void Start()
  {
    this.objects = MystObjectManager.Instance;
    // Debug.Log("called");

    WINDOW_TOP_LEFT = new Vector2(0, 24);
    WINDOW_BOTTOM_RIGHT = new Vector2(32, 0);
    AudioListener.volume = 0.05f;

    manager_state = 10;
    GOBJ_MAIN_CAMERA = GameObject.Find("Main Camera");
    BLOOM = GOBJ_MAIN_CAMERA.transform.FindChild("EffectCam").GetComponent<Bloom>();
    is_title_exit = false;

    GOBJ_CANVAS = GameObject.Find("Canvas");
    ANIM_TITLE = GameObject.Find("Acters").GetComponent<Animator>();
    ANIM_TITLE.enabled = false;
    GOBJ_TITLE = GOBJ_CANVAS.transform.FindChild("Title").gameObject;

    retry_count = 0;
    GOBJ_RETRY_COUNT_OVER = GOBJ_CANVAS.transform.FindChild("GameOver/RetryCount").gameObject;
    GOBJ_RETRY_COUNT_CLEAR = GOBJ_CANVAS.transform.FindChild("GameClear/RetryCount").gameObject;

    AUDIO_BGM = GetComponent<AudioSource>();
    CLIP_BGM_TITLE = Resources.Load("BGM/そよぐ帳と夢の夢") as AudioClip;
    CLIP_BGM_MAIN = Resources.Load("BGM/New_Gear") as AudioClip;
    AUDIO_BGM.clip = CLIP_BGM_TITLE;
    AUDIO_BGM.Play();

    PLAY_AREA = new Vector2(32, 24);
    GOBJ_PLAYER = GameObject.Find("MystPlayer");
    GOBJ_PLAYER_CHARGE = GOBJ_PLAYER.transform.FindChild("Charge").gameObject;
    GOBJ_PLAYER_CHARGE.SetActive(false);
    player_enable_controlle = true;
    PLAYER_MOVE_SPEED = 8f;
    PLAYER_LAYER = LayerMask.GetMask(new string[] { "Player" });
    PLAYER_LIFE_MAX = 1;
    player_life = PLAYER_LIFE_MAX;
    PLAYER_DAMAGE_PERIOD = 1f;

    GOBJ_OPTIONS = GameObject.Find("Options");
    GOBJ_OPTIONS.SetActive(false);
    OPTIONS_ANIMATION = GOBJ_OPTIONS.transform.FindChild("OptionImage/myst").GetComponent<OptionAnimation>();
    OPTIONS_MARGIN = 2f;
    options_direction = Vector2.right;
    OPTIONS_DIRECTION_SPD = 10f;
    option_state = 0;
    isLockOptionPosition = false;
    CHARGE_DURATION = 1f;
    OPTIONS_SPD = 70f;
    OPTIONS_BACK_SPD = 60f;
    OPTIONS_LAYER = LayerMask.GetMask(new string[] { "Bullet(Player)" });

    PRFB_LOCK_OPTION = Resources.Load("Prefabs/Effects/LockEffect") as GameObject;
    PRFB_RELEASE_OPTION = Resources.Load("Prefabs/Effects/ReleaseEffect") as GameObject;
    PRFB_SHOT_OPTION = Resources.Load("Prefabs/Effects/ChargeShotEffect") as GameObject;

    ENEMYS_LAYER = LayerMask.GetMask(new string[] { "Enemy", "EnemyFly", "EnemyBullet" });

    GOBJ_BOSS = GameObject.Find("MystBoss");
    BOSS_ACTION = new BossAction();
    BOSS_LIFE_MAX = 15;
    boss_life = BOSS_LIFE_MAX;
    GOBJ_BOSS_LIFE = GameObject.Find("BossLife");
    BOSS_INITIAL_LIFE_HEIGHT = 1;
    BOSS_LIFE_HEIGHT_POINT = BOSS_INITIAL_LIFE_HEIGHT / BOSS_LIFE_MAX;
    BOSS_DAMAGE_PERIOD = 1f;

    PRFB_DAMAGE = Resources.Load("Prefabs/Effects/Damage") as GameObject;
    PRFB_DESTROY = Resources.Load("Prefabs/Effects/Destroy") as GameObject;

    _HIGH_PASS_FILTER = GetComponent<AudioHighPassFilter>();

    #region Debug
    // StartCoroutine(InitializeOpenning());
    // GOBJ_CANVAS.SetActive(false);
    #endregion

    // ゲーム状態初期化用にGameStopを呼びたいが、タイトル画面にオプションが表示されちゃう
    this.GameStop();
    GOBJ_BOSS.transform.position = new Vector2(33f, 5.25f);
    GOBJ_BOSS_LIFE.SetActive(false);

    // GOBJ_OPTIONS.SetActive(false);

  }

  private void GameStop()
  {
    // 座標の初期化、当たり判定や、動作中のボスなどを止める
    BOSS_ACTION.Stop();
    objects.DestroyAll();
    player_state = 0;
    player_is_nodamage = true;
    option_state = 0;
    boss_is_nodamage = true;

    GOBJ_PLAYER_CHARGE.SetActive(false);

    GOBJ_PLAYER.transform.localPosition = new Vector2(4.25f, 12f);
    GOBJ_BOSS.transform.localPosition = new Vector2(29f, 11f);
    GOBJ_OPTIONS.SetActive(true);
    options_direction = new Vector2(1, 0.1f);
    iTween.Stop(GOBJ_OPTIONS);
    GOBJ_OPTIONS.transform.position = (Vector2)GOBJ_PLAYER.transform.position + options_direction.normalized * OPTIONS_MARGIN;
    GOBJ_OPTIONS.transform.rotation = Quaternion.identity;

    this.GOBJ_BOSS_LIFE.SetActive(true);

    player_life = PLAYER_LIFE_MAX;
    if(this.BOSS_ACTION.boss_state == 1){
      boss_life = BOSS_LIFE_MAX / 3 * 2;
    }else if(this.BOSS_ACTION.boss_state == 2){
      boss_life = BOSS_LIFE_MAX / 3;
    }else{
      boss_life = BOSS_LIFE_MAX;
    }
  }
  private void GameStart()
  {
    // 当たり判定の有効化や、ボスの動作開始を行う
    player_is_nodamage = false;
    boss_is_nodamage = false;

    player_enable_controlle = true;
    is_charging = false;
    charge_start_time = 0f;
    isLockOptionPosition = false;

    BOSS_ACTION.Start();
  }
  void Update()
  {
    if (manager_state == 0)
    {
      // Ready
      UpdatePlayer();
      UpdateOptions();
    }
    else if (manager_state == 1)
    {
      // GameMain
      UpdatePlayer();
      UpdateOptions();
      BOSS_ACTION.Update();
      UpdateBoss();
      UpdateLife();
    }
    else if (manager_state == 2)
    {

    }
    else if (manager_state == 3)
    {
      // GameOver
      InputManager im = InputManager.Instance;
      if (im.GetKeyDown(MBLDefine.Key.Shot))
      {
        manager_state = 0;
        retry_count++;
        StartCoroutine(InitializeOpenning());
      }
    }
    else if (manager_state == 4)
    {
      // GameClear
      InputManager im = InputManager.Instance;
      if (im.GetKeyDown(MBLDefine.Key.Shot))
      {
        Destroy(this.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      }
    }
    else if (manager_state >= 10 && manager_state <= 19)
    {
      // Title
      UpdateTitle();
      UpdatePlayer();
      UpdateOptions();
    }
  }
  void FixedUpdate()
  {
    FixedUpdateOption();
  }

  void LateUpdate()
  {
    BOSS_ACTION.LateUpdate();
  }


  private void UpdateTitle()
  {
    // タイトル系画面メインループ
    InputManager im = InputManager.Instance;

    if (manager_state == 10)
    {
      player_prev_pos = GOBJ_PLAYER.transform.position;
      var noiseX = Mathf.PerlinNoise(100, Time.time * 0.5f);
      var noiseY = Mathf.PerlinNoise(500, Time.time * 0.5f);
      var newPos = new Vector2(
        noiseX * (WINDOW_BOTTOM_RIGHT.x - 6) + 3,
        noiseY * (WINDOW_TOP_LEFT.y - 6) + 3
      );
      GOBJ_PLAYER.transform.position = newPos;
      if(Input.anyKeyDown){
        manager_state = 11;
      }
    }
    else if (manager_state == 11)
    {
      if (option_state == 20)
      {
        manager_state = 11;
        player_enable_controlle = false;
        StartCoroutine(StartStaging());
      }
    }
    else if (manager_state == 12){
      // 演出中は何も出来ない
    }
  }

  private IEnumerator StartStaging()
  {
    // ゲーム開始演出(タイトル画面脱出演出)
    iTween.ScaleTo(GOBJ_TITLE, iTween.Hash(
      "x", 0, "time", 0.4f, "easeType", iTween.EaseType.easeInCirc
    ));
    yield return new WaitForSeconds(1f);

    var moveTo = new Vector2(4.25f, 6.25f);
    var distance = moveTo - (Vector2)GOBJ_PLAYER.transform.position;
    var moveTime = distance.magnitude * 0.04f;
    // Debug.Log(moveTime);
    iTween.MoveTo(GOBJ_PLAYER, iTween.Hash(
      "x", moveTo.x, "y", moveTo.y, "time", moveTime, "easeType", iTween.EaseType.linear
    ));
    yield return new WaitForSeconds(moveTime + 0.1f);

    ANIM_TITLE.enabled = true;
    ANIM_TITLE.Play("Acters_Start");

    yield return new WaitForSeconds(4.1f);

    TweenBloom(0f, 10f, 1.5f);
    TweenBGMVolume(1f, 0f, 1f, iTween.EaseType.easeInCubic);

    yield return new WaitForSeconds(2f);

    AUDIO_BGM.Stop();
    AUDIO_BGM.volume = 1;
    AUDIO_BGM.clip = CLIP_BGM_MAIN;
    AUDIO_BGM.Play();
    _HIGH_PASS_FILTER.cutoffFrequency = 0f;

    StartCoroutine(InitializeOpenning());
  }

  // ---

  private IEnumerator InitializeOpenning()
  {
    // ゲーム開始演出（レディー表示）

    // 演出初期化
    if (ANIM_TITLE)
    {
      Destroy(ANIM_TITLE);
      Destroy(GOBJ_PLAYER.transform.FindChild("Ase").gameObject);
      Destroy(GOBJ_BOSS.transform.FindChild("Sana").gameObject);
      this.GameStop();
    }
    GOBJ_PLAYER.GetComponent<SpriteRenderer>().enabled = true;
    BLOOM.bloomIntensity = 10f;

    // 演出中
    float fade_time = 1.6f;

    if (this.is_title_exit)
    {
      fade_time = 0.4f;
      var gobj_gameover = GOBJ_CANVAS.transform.FindChild("GameOver").gameObject;
      iTween.ScaleTo(gobj_gameover, iTween.Hash(
        "x", 100f, "y", 0f, "time", 0.1f, "easetype", iTween.EaseType.linear));
      yield return new WaitForSeconds(0.2f);
    }
    GOBJ_CANVAS.transform.FindChild("Ready").gameObject.transform.localScale = Vector3.one;
    TweenBloom(10f, 0f, fade_time);
    if (this.is_title_exit) TweenHighPassFilter(3000f, 0f, fade_time);

    yield return new WaitForSeconds(fade_time * 1.5f);

    var gobj_ready = GOBJ_CANVAS.transform.FindChild("Ready").gameObject;
    iTween.ScaleTo(gobj_ready, iTween.Hash(
      "x", 100, "y", 0, "time", 0.1f, "easetype", iTween.EaseType.linear));

    yield return new WaitForSeconds(0.2f);

    // 演出終了
    this.is_title_exit = true;
    this.GameStart();
    this.manager_state = 1;
  }

  // ---

  private void UpdatePlayer()
  {
    if (player_state == 0)
    {
      var hit = Physics2D.OverlapCircle(GOBJ_PLAYER.transform.position, 0.25f, ENEMYS_LAYER);
      if (hit && !player_is_nodamage)
      {
        player_life -= 10;
        if (player_life <= 0)
        {
          BOSS_ACTION.Stop();
          StartCoroutine(GameOver());
        }
        else
        {
          StartCoroutine(DamageForPlayer());
          // player_state = 1;
          // player_damage_start_time = Time.time;
        }
      }
    }
    else if (player_state == 1)
    {
      if (player_damage_start_time + PLAYER_DAMAGE_PERIOD < Time.time)
      {
        // player_state = 0;	
      }
    }

    if (player_enable_controlle)
    {
      InputManager im = InputManager.Instance;

      // Move
      float x = im.GetAxesRaw(MBLDefine.Axes.Horizontal);
      float y = im.GetAxesRaw(MBLDefine.Axes.Vertical);

      var nextPosition = GOBJ_PLAYER.transform.position;
      if (x != 0) nextPosition.x += PLAYER_MOVE_SPEED * x * Time.deltaTime;
      if (y != 0) nextPosition.y += PLAYER_MOVE_SPEED * y * Time.deltaTime;

      if (nextPosition.x < 0 || nextPosition.x > PLAY_AREA.x) nextPosition.x = GOBJ_PLAYER.transform.position.x;
      if (nextPosition.y < 0 || nextPosition.y > PLAY_AREA.y) nextPosition.y = GOBJ_PLAYER.transform.position.y;
      GOBJ_PLAYER.transform.position = nextPosition;
    }
  }

  // ---

  private void UpdateOptions()
  {
    InputManager im = InputManager.Instance;

    if (option_state == 0)
    {
      // Follow
      if (player_enable_controlle)
      {
        if (!isLockOptionPosition)
        {
          // unlock
          float x = im.GetAxesRaw(MBLDefine.Axes.Horizontal);
          float y = im.GetAxesRaw(MBLDefine.Axes.Vertical);

          if(manager_state == 10){
            // title
            var delta = (Vector2)GOBJ_PLAYER.transform.position - player_prev_pos;
            if(delta.x < 0) x = -0.5f;
            if(delta.x > 0) x = 0.5f;
            if(delta.y < 0) y = -0.5f;
            if(delta.y > 0) y = 0.5f;
          }

          options_direction.x -= x * OPTIONS_DIRECTION_SPD * Time.deltaTime;
          options_direction.y -= y * OPTIONS_DIRECTION_SPD * Time.deltaTime;

          options_direction = options_direction.normalized;
          var newPos = (Vector2)GOBJ_PLAYER.transform.position + (options_direction * OPTIONS_MARGIN);
          GOBJ_OPTIONS.transform.position = newPos;
        }
        else
        {
          // locked
          GOBJ_OPTIONS.transform.position = GOBJ_PLAYER.transform.position + lockedRelativePosition;
        }

        if (im.GetKeyDown(MBLDefine.Key.Shot))
        {
          // チャージ関連
          charge_start_time = Time.time;
          GOBJ_PLAYER_CHARGE.SetActive(true);
          is_charging = true;

          // ロック関連
          isLockOptionPosition = true;
          lockedRelativePosition = GOBJ_OPTIONS.transform.position - GOBJ_PLAYER.transform.position;
          OPTIONS_ANIMATION.enabled = false;
          objects.InstantiateTiny(PRFB_LOCK_OPTION, GOBJ_OPTIONS.transform.position);
        }
        if (im.GetKeyUp(MBLDefine.Key.Shot))
        {
          // チャージ関連
          if (this.is_charging && Time.time >= charge_start_time + CHARGE_DURATION)
          {
            option_state = 10;
            isLockOptionPosition = false;
          }
          GOBJ_PLAYER_CHARGE.SetActive(false);
          is_charging = false;
          charge_start_time = 0;

          // ロック関連
          isLockOptionPosition = false;
          OPTIONS_ANIMATION.enabled = true;
          objects.InstantiateTiny(PRFB_RELEASE_OPTION, GOBJ_OPTIONS.transform.position);
        }
      }else{
        var newPos = (Vector2)GOBJ_PLAYER.transform.position + (options_direction * OPTIONS_MARGIN);
        GOBJ_OPTIONS.transform.position = newPos;
      }
    }
    else if (option_state == 10)
    {
      // Shot
      var newPos = GOBJ_OPTIONS.transform.position;
      newPos.x += OPTIONS_SPD * Time.deltaTime;
      if (newPos.x >= WINDOW_BOTTOM_RIGHT.x - 1)
      {
        option_state = 20;
      }
      else
      {
        GOBJ_OPTIONS.transform.position = newPos;
      }
    }
    else if (option_state == 20)
    {
      // Return

      // ココの処理はFixedUdateで
    }
  }
  private void FixedUpdateOption()
  {
    InputManager im = InputManager.Instance;
    if (option_state == 20)
    {
      // Return
      var distance = GOBJ_PLAYER.transform.position - GOBJ_OPTIONS.transform.position;
      if (Mathf.Abs(distance.sqrMagnitude) < (OPTIONS_MARGIN * OPTIONS_MARGIN))
      {
        // hit
        options_direction = distance.normalized * -1;
        option_state = 0;

        if (im.GetKey(MBLDefine.Key.Shot) && is_title_exit)
        {
          // 衝突した時、ボタン押しっぱならチャージに遷移
          // チャージ関連
          charge_start_time = Time.time;
          GOBJ_PLAYER_CHARGE.SetActive(true);
          is_charging = true;

          // ロック関連
          isLockOptionPosition = true;
          lockedRelativePosition = GOBJ_OPTIONS.transform.position - GOBJ_PLAYER.transform.position;
          OPTIONS_ANIMATION.enabled = false;
          objects.InstantiateTiny(PRFB_LOCK_OPTION, GOBJ_OPTIONS.transform.position);
        }
      }
      else
      {
        var direction = distance.normalized;
        GOBJ_OPTIONS.transform.position += direction * OPTIONS_BACK_SPD * Time.fixedDeltaTime;
      }
    }

  }

  private void UpdateLife()
  {
    var boss_current_height = boss_life * BOSS_LIFE_HEIGHT_POINT;
    var boss_life_image = GOBJ_BOSS_LIFE.GetComponent<Image>();
    boss_life_image.fillAmount = boss_current_height;
  }

  private void UpdateBoss()
  {
    if (boss_state == 0)
    {
      if (boss_life <= 0 && player_life > 0)
      {
        StartCoroutine(GameClear());
      }
    }
    else if (boss_state == 1)
    {
      if (boss_damage_start_time + BOSS_DAMAGE_PERIOD < Time.time)
      {
        // boss_state = 0;	
      }
    }
  }

  //---

  public IEnumerator GameOver()
  {
    // ゲームオーバー演出

    // 演出初期化
    player_enable_controlle = false;
    GOBJ_PLAYER.GetComponent<SpriteRenderer>().enabled = false;
    player_is_nodamage = true;
    boss_is_nodamage = true;
    GOBJ_RETRY_COUNT_OVER.GetComponent<Text>().text = "Retry:" + retry_count;

    this.objects.InstantiateTiny(PRFB_DESTROY, GOBJ_PLAYER.transform.position);
    TweenHighPassFilter(10f, 3000f, 0.5f);
    TweenBloom(0f, 10f, 0.1f);
    yield return new WaitForSeconds(0.2f);
    this.GameStop();

    var gobj_game_over = GOBJ_CANVAS.transform.FindChild("GameOver").gameObject;
    iTween.ScaleTo(gobj_game_over, iTween.Hash(
      "x", 1, "y", 1, "time", 0.1f, "easetype", iTween.EaseType.linear));
    yield return new WaitForSeconds(0.2f);
    GOBJ_PLAYER.GetComponent<SpriteRenderer>().enabled = true;
    manager_state = 3;
  }

  public IEnumerator GameClear()
  {
    // 出した弾を消す
    var bullets = GameObject.FindGameObjectsWithTag("Bullet(Enemy)");
    var enemies = GameObject.FindGameObjectsWithTag("Enemy");
    foreach(var bullet in bullets){
      var showEffect = Random.Range(0,2);
      if(showEffect == 0) objects.InstantiateTiny(PRFB_DESTROY, bullet.transform.position);
      MystGameManager.Destroy(bullet);
    }
    foreach(var enemy in enemies){
      MystGameManager.Destroy(enemy);
    }
    player_is_nodamage = true;
    player_enable_controlle = false;
    BOSS_ACTION.Stop();
    GOBJ_OPTIONS.SetActive(false);
    GOBJ_BOSS_LIFE.SetActive(false);

    manager_state = 4;
    GOBJ_BOSS.GetComponent<SpriteRenderer>().enabled = false;
    var gobj_boss_pos = GOBJ_BOSS.transform.position;
    gobj_boss_pos.y += 1;
    this.objects.InstantiateTiny(PRFB_DESTROY, gobj_boss_pos);

    yield return new WaitForSeconds(0.1f);

    TweenHighPassFilter(10f, 22000f, 5f, iTween.EaseType.easeInCubic);
    TweenBloom(0f, 10, 5f, iTween.EaseType.easeInCubic);

    // ハイパスフィルターで音を飛ばす、一呼吸おいて、おだやかめなBGM

    GOBJ_RETRY_COUNT_CLEAR.GetComponent<Text>().text = "Retry:" + retry_count;
    yield return new WaitForSeconds(5f);

    var gobj_ready = GOBJ_CANVAS.transform.FindChild("GameClear").gameObject;
    iTween.ScaleTo(gobj_ready, iTween.Hash(
      "x", 1, "y", 1, "time", 0.1f, "easetype", iTween.EaseType.linear));
    yield return new WaitForSeconds(3f);

    // Destroy(this.gameObject);
    // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  // private void ReStart(){
  //   Debug.Log("ReStart");
  //   AudioListener.volume = 0.05f;

  //   manager_state = 10;
  //   is_title_exit = false;

  //   AUDIO_BGM.clip = CLIP_BGM_TITLE;
  //   AUDIO_BGM.Play();

  //   GOBJ_PLAYER_CHARGE.SetActive(false);
  //   player_enable_controlle = false;
  //   player_life = PLAYER_LIFE_MAX;
  //   is_charging = false;
  //   charge_start_time = 0;

  //   GOBJ_OPTIONS.SetActive(false);
  //   options_direction = Vector2.right;
  //   option_state = 0;
  //   isLockOptionPosition = false;

  //   BOSS_ACTION = new BossAction();
  //   boss_life = BOSS_LIFE_MAX;

  //   this.GameStop();
  //   GOBJ_OPTIONS.SetActive(false);
  //   GOBJ_BOSS_LIFE.SetActive(false);
  // }

  //---

  private IEnumerator DamageForPlayer()
  {
    this.objects.InstantiateTiny(PRFB_DAMAGE, GOBJ_PLAYER.transform.position);
    player_is_nodamage = true;
    var sprite = GOBJ_PLAYER.GetComponent<SpriteRenderer>();
    int count = 30;
    bool forRed = true;
    for (int i = 0; i < count; i++)
    {
      if (forRed)
      {
        sprite.color = Color.red;
      }
      else
      {
        sprite.color = Color.white;
      }
      forRed = !forRed;
      yield return new WaitForSeconds(0.05f);
    }
    player_is_nodamage = false;
  }

  // ---
  private void TweenBase(string update, float from, float to, float time, iTween.EaseType easeType = iTween.EaseType.linear)
  {
    Hashtable hash = new Hashtable(){
      {"from", from}, {"to", to}, {"time", time},
      {"easeType", easeType},
      {"onupdate", update}, {"onupdatetarget", gameObject}
    };
    iTween.ValueTo(gameObject, hash);
  }

  private void TweenHighPassFilter(float from, float to, float time, iTween.EaseType easeType = iTween.EaseType.linear)
  {
    TweenBase("OnUpdateHighPassFilter", from, to, time, easeType);
  }
  public void OnUpdateHighPassFilter(float val)
  {
    _HIGH_PASS_FILTER.cutoffFrequency = val;
  }

  private void TweenBloom(float from, float to, float time, iTween.EaseType easeType = iTween.EaseType.linear)
  {
    TweenBase("OnUpdateBloom", from, to, time, easeType);
  }
  public void OnUpdateBloom(float val)
  {
    BLOOM.bloomIntensity = val;
  }

  private void TweenBGMVolume(float from, float to, float time, iTween.EaseType easeType = iTween.EaseType.linear)
  {
    TweenBase("OnUpdateBGMVolume", from, to, time, easeType);
  }
  public void OnUpdateBGMVolume(float val)
  {
    AUDIO_BGM.volume = val;
  }

  // ---

  public Vector3 FixedBossPosition()
  {
    var pos = GOBJ_BOSS.transform.position;
    pos.y += 1;
    return pos;
  }
}
