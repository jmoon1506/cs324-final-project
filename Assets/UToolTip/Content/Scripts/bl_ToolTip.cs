using UnityEngine;
using UnityEngine.UI;


public class bl_ToolTip : MonoBehaviour
{
    /// <summary>
    /// Root ToolTip UI
    /// </summary>
    public RectTransform m_Rect = null;
    /// <summary>
    /// Custom position of tooltip to mouse point
    /// </summary>
    public Vector3 OffSet = Vector2.zero;
    public Text TooltipText = null;
    public Image ToolTipImage = null;
    [Space(5)]
    public AnimationClip ShowHideAnim;
    [Space(5)]
    public Image ToolTipBg = null;
    public Sprite LeftTopBg;
    public Sprite LeftBottonBg;
    public Sprite RightTopBg;
    public Sprite RightBottonBg;

    private bool m_Show = false;
    private UTTPlaceH H;
    private UTTPlaceV V;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (!m_Show)
            return;
       
        Vector2 mp = MousePosition;
        //Determine the correct position for show the tooltip
        //NOTE: if you have problem with the position, please read the readme.txt
        Vector3 v = new Vector3(((Screen.width * 0.5f) - (-mp.x)) - Screen.width, mp.y - (Screen.height * 0.5f), 0);
        //Lerp Movement for best smooth effect.
        m_Rect.anchoredPosition = Vector3.Lerp(m_Rect.anchoredPosition, v, Time.deltaTime * 20);
        AnchorPosition();
    }

    /// <summary>
    /// Show tooltip event
    /// </summary>
    /// <param name="b">show?</param>
    public void ShowToolTip(bool b,float wait = 0.0f, Sprite s = null,string t = "")
    {

        m_Show = b;
        CacheSprite = s;
        CacheText = t;
        if (b)
        {
            Invoke("ToolTip", wait);           
        }
        else
        {
            CancelInvoke("ToolTip");
            m_Show = b; AnimatedState(false);
        }

    }

    private Sprite CacheSprite;
    private string CacheText;
    void ToolTip()
    {
        //if this have a sprite
        if (CacheSprite != null)
        {
            ToolTipImage.gameObject.SetActive(true);
            ToolTipImage.sprite = CacheSprite;
        }
        else//if this dont have a sprite
        {
            ToolTipImage.gameObject.SetActive(false);
        }
        //refresh sprite
        if (!m_Show) { ToolTipImage.sprite = null; }
        //get values  
        AnimatedState(m_Show);
        TooltipText.text = CacheText;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void AnimatedState(bool b)
    {
        Animation a = m_Rect.GetComponent<Animation>();
        if (b)
        {
            //play animation forward
            a[ShowHideAnim.name].speed = 1.0f;
            a.CrossFade(ShowHideAnim.name, 0.2f);
        }
        else
        {
            //play animation in reverse
            if (a[ShowHideAnim.name].time == 0.0f)
            {
                a[ShowHideAnim.name].time = a[ShowHideAnim.name].length;
            }
            a[ShowHideAnim.name].speed = -2.0f;
            a.CrossFade(ShowHideAnim.name, 0.2f);
        }
    }
    /// <summary>
    /// avoid the tooltip exceed screen borders.
    /// </summary>
    private Vector2 MousePosition
    {
        get
        {
            Vector3 v = Input.mousePosition;
            if ((v.x + ( m_Rect.sizeDelta.x)) <= Screen.width)
            {
                v.x += OffSet.x;
                H = UTTPlaceH.Left;
            }
            else
            {
                v.x -= OffSet.x;
                H = UTTPlaceH.Right;
            }

            if ((v.y + (m_Rect.sizeDelta.y)) > Screen.height)
            {
                v.y += OffSet.y;
                V = UTTPlaceV.Top;
            }
            else
            {
                v.y -= OffSet.y;
                V = UTTPlaceV.Botton;
            }

            return v;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void AnchorPosition()
    {
        if (V == UTTPlaceV.Top && H == UTTPlaceH.Left)
        {
            ToolTipBg.sprite = LeftTopBg;
        }
        else if (V == UTTPlaceV.Top && H == UTTPlaceH.Right)
        {
            ToolTipBg.sprite = RightTopBg;
        }
        else if (V == UTTPlaceV.Botton && H == UTTPlaceH.Left)
        {
            ToolTipBg.sprite = LeftBottonBg;
        }
        else if (V == UTTPlaceV.Botton && H == UTTPlaceH.Right)
        {
            ToolTipBg.sprite = RightBottonBg;
        }
    }

     // Standard Singleton Access
        private static bl_ToolTip instance;
        public static bl_ToolTip Instance
        {
            get
            {
                if (instance == null)
                    instance = GameObject.FindObjectOfType<bl_ToolTip>();
                return instance;
            }
        }
}