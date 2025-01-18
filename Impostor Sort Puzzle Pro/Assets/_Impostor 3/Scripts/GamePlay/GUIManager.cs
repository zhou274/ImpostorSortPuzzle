using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;


public class GUIManager : SingletonMonoDontDestroy<GUIManager>
{
    public GameObject gamePlayPanel;
    public GameObject menuPanel;
    public GameObject winningCanvasPanel;
    public GameObject videoGO;
    public GameObject priceGO;
    public GameObject MainMenuPanel;
    public TextMeshProUGUI levelText;
    public Text rewindText;
    public AudioSource sound;
    public AudioClip[] soundList;

    public string clickid;
    private StarkAdManager starkAdManager;

    public List<AudioSource> allSound;

    public Image soundImg;
    public Sprite[] soundImgSprite;
    
    public Image vibrateImg;
    public Sprite[] vibrateImgSprite;

    public bool isSoundTurnOn = true;
    public bool canVibrate = true;

    public GameObject AddRewindPanel;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        MainMenuPanel.SetActive(true);
        gamePlayPanel.SetActive(false);
    }
    public void Play()
    {
        MainMenuPanel.SetActive(false);
        gamePlayPanel.SetActive(true);
    }
    public void SetDefaultGUI()
    {
        levelText.text = "关卡 " + (GameManager.Instance.currentLevel  + 1);
        rewindText.text = GameManager.Instance.numberRewinds.ToString();
        if (isSoundTurnOn)
        {
            soundImg.sprite = soundImgSprite[1];
            foreach (AudioSource _audio in allSound)
            {
                _audio.volume = 1;
            }
        }
        else
        {
            soundImg.sprite = soundImgSprite[0];
            foreach (AudioSource _audio in allSound)
            {
                _audio.volume = 0;
            }
        }
        if (canVibrate)
        {
            vibrateImg.sprite = vibrateImgSprite[1];
        }
        else
        {
            vibrateImg.sprite = vibrateImgSprite[0];
        }
        
        if (GameManager.Instance.numberRewinds==0)
        {
            EnableWatchingVideo();
        }
    }

    public void ActivatingMenu()
    {
        PlayClickBoxSound();
        // IAPManager.Instance().Init(null);
        // var localPrice = IAPManager.Instance().GetPriceStringById(ProductId.PackageNoAds);
        // if (localPrice.Length == 0 || localPrice == "0")
        // {
        //     priceGO.GetComponent<Text>().text = $"$2.99";
        // }
        // else
        // {
        //     priceGO.GetComponent<Text>().text = localPrice;
        // }
        menuPanel.SetActive(true);
    }
    
    public void HideMenu()
    {
        menuPanel.SetActive(false);
    }
    
    public void EnableWatchingVideo()
    {
        //videoGO.SetActive(true);
    }
    
    public void WatchVideo1()
    {
        if (!GameManager.Instance.isRemoveAds)
        {
            // todo txy
//            AdsManager.Instance.ShowInterstitialAds();
            AdManager.Instance.ShowInterstitialAd();
        }
    }
    
    public void WatchVideo2()
    {
        
        // todo txy
        AdManager.Instance.RewardAction = () =>
        {
            GameManager.Instance.numberRewinds = 5;
            SetDefaultGUI();
            videoGO.SetActive(false);
        };
        AdManager.Instance.ShowRewardAd();
        
//        AdsManager.Instance.ShowAds(AdsPlacement.ADS_MAIN, "rewind_bonus", () =>
//        {
//            GameManager.Instance.numberRewinds = 5;
//            SetDefaultGUI();
//            videoGO.SetActive(false);
//        });
    }
    
    public void WatchVideo3()
    {
        PlayClickBoxSound();
        if (!GameManager.Instance.isAddBox)
        {
            // todo txy
            AdManager.Instance.RewardAction = () =>
            {
                AddOneBox();
            };
            AdManager.Instance.ShowRewardAd();
            
//            AdsManager.Instance.ShowAds(AdsPlacement.ADS_MAIN, "add_box", () =>
//            {
//                AddOneBox();
//            });
        }
    }

    public void Replay()
    {
        PlayClickBoxSound();
        videoGO.SetActive(false);
        GameController.Ins.gamePlayController.Replay();
    }
    
    private bool isEnbleToRewind()
    {
        return GameManager.Instance.numberRewinds > 0;
    }
    
    public void Rewind()
    {
        PlayClickBoxSound();
        //if (GameController.Ins.gamePlayController.movementSaveStack.Count > 0)
        //{
        //    GameController.Ins.gamePlayController.RewindPlay();
        //}
        if (isEnbleToRewind())
        {
            if (GameController.Ins.gamePlayController.movementSaveStack.Count > 0)
            {
                GameController.Ins.gamePlayController.RewindPlay();

            }
        }
        else
        {
            AddRewindPanel.SetActive(true);
            //if (GameManager.Instance.numberRewinds == 0)
            //{
            //    EnableWatchingVideo();
            //    WatchVideo2();
            //}
        }

    }
    public void AddRewind()
    {
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {

                    GameManager.Instance.numberRewinds = GameManager.Instance.numberRewinds + 5;
                    GUIManager.Instance.SetDefaultGUI();
                    AddRewindPanel.SetActive(false);


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
    }
    public void HideAddPanel()
    {
        AddRewindPanel.SetActive(false);
    }
    public void AddOneBox()
    {
        GameController.Ins.gamePlayController.AddOneBox();
    }

    public void Shopping()
    {
        PlayClickBoxSound();
    }

    public void RemoveAds()
    {
        PlayClickBoxSound();
        // IAPManager.Instance().Buy(ProductId.PackageNoAds, (success, str) =>
        // {
        //     if (success)
        //     {
        //         GameManager.Instance.isRemoveAds = true;
        //         ObscuredPrefs.SetBool("RemoveAds",true);
        //     }
        // });
    }
    
    public void TurnOnOffSound()
    {
        PlayClickBoxSound();
        if (isSoundTurnOn)
        {
            soundImg.sprite = soundImgSprite[0];
            foreach (AudioSource _audio in allSound)
            {
                _audio.volume = 0;
            }
        }
        else
        {
            soundImg.sprite = soundImgSprite[1];
            foreach (AudioSource _audio in allSound)
            {
                _audio.volume = 1;
            }
        }
        isSoundTurnOn = !isSoundTurnOn;
        //Debug.Log(isSoundTurnOn);
        ObscuredPrefs.SetBool("isSoundOn",isSoundTurnOn);
    }

    public void Vibrating()
    {
        if (!canVibrate) return;
        //this.StartDelayMethod( 0.5f,()=>Handheld.Vibrate());
    }
    
    public void TurnOnOffVibrate()
    {
        PlayClickBoxSound();
        if (canVibrate)
        {
            vibrateImg.sprite = vibrateImgSprite[0];
        }
        else
        {
            vibrateImg.sprite = vibrateImgSprite[1];
        }
        canVibrate = !canVibrate;
        ObscuredPrefs.SetBool("canVibrate",canVibrate);
    }

    public void GivenGift()
    {
        PlayClickBoxSound();
    }
    
    public void PlayClickBoxSound()
    {
        sound.PlayOneShot(soundList[1]);
    }
    
    public void PlayFallingSound()
    {
        sound.PlayOneShot(soundList[2]);
    }
    
    public void PlaySolveSound()
    {
        sound.PlayOneShot(soundList[3]);
    }

    public void PlayWinningSound()
    {
        sound.PlayOneShot(soundList[4]);
    }

    public void PlayWrongPick()
    {
        sound.PlayOneShot(soundList[5]);
    }

    public void ActivatingWinningCanvas()
    {
        winningCanvasPanel.SetActive(true);
        ShowInterstitialAd("1lcaf5895d5l1293dc",
            () => {
                Debug.LogError("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
    }
    
    public void EndWinningCanvas()
    {
        winningCanvasPanel.SetActive(false);
    }

    public void NextLevel()
    {
        PlayClickBoxSound();
        SetDefaultGUI();
        EndWinningCanvas();
        GameController.Ins.gamePlayController.NextLevel();
    }

    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
}
